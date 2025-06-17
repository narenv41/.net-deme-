using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SampleMvcApp.Models;
using SampleMvcApp.Services;
using System.Diagnostics;
using MongoDB.Bson;
using System.Collections.Generic;

[Authorize]
public class TicketController : Controller
{
    private readonly TicketService _ticketService;
    private readonly MessageService _messageService;

    public TicketController(TicketService ticketService, MessageService messageService)
    {
        _ticketService = ticketService;
        _messageService = messageService;
    }

    public async Task<IActionResult> Index()
    {
        var tickets = await _ticketService.GetAllAsync();
        return View(tickets);
    }

    public IActionResult Create()
    {
        return View();
    }

[HttpPost]public async Task<IActionResult> Create(Ticket ticket)
{
    ticket.CreatedBy = User.Identity?.Name ?? "Anonymous";
    ticket.CreatedAt = DateTime.UtcNow;
    ticket.Status = "Active";

    if (string.IsNullOrEmpty(ticket.Priority))
    {
        var payload = new
        {
            Messages = new List<string> { ticket.Description ?? "" }
        };

        var inputJson = System.Text.Json.JsonSerializer.Serialize(payload);
        var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "AI", "t3init.py");

        var startInfo = new ProcessStartInfo
        {
            FileName = "python",
            Arguments = $"\"{scriptPath}\" \"{ticket.Description}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = Process.Start(startInfo))
        {
            string output = await process.StandardOutput.ReadToEndAsync();
            ticket.Priority = output.Trim();
        }
    }

    await _ticketService.CreateAsync(ticket);
    return RedirectToAction("Index");
}



    public async Task<IActionResult> Details(string id)
    {
        var ticket = await _ticketService.GetByIdWithMessagesAsync(id);
        if (ticket == null)
            return NotFound();

        return View(ticket);
    }

    [HttpPost]
    public async Task<IActionResult> AddMessage(string ticketId, string msgText, IFormFile? file)
    {
        var message = new Message
        {
            TicketId = ticketId,
            Sender = User.Identity?.Name ?? "Anonymous",
            Text = msgText ?? string.Empty,
            Timestamp = DateTime.UtcNow
        };
        

        if (file != null && file.Length > 0)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            message.FileData = ms.ToArray();
            message.FileName = file.FileName;
            message.ContentType = file.ContentType;
        }

        await _messageService.CreateAsync(message);

        return Json(new
        {
            id = message.Id.ToString(),
            sender = message.Sender,
            text = message.Text,
            timestamp = message.Timestamp,
            fileName = message.FileName,
            fileUrl = message.FileName != null
                ? Url.Action("DownloadFile", "Ticket", new { id = message.Id })
                : null
        });
    }

    public async Task<IActionResult> DownloadFile(string id)
    {
        var message = await _messageService.GetByIdAsync(id);
        if (message == null || message.FileData == null)
            return NotFound();

        return File(message.FileData, message.ContentType ?? "application/octet-stream", message.FileName ?? "attachment");
    }

    [HttpPost]
    public async Task<IActionResult> CloseTicket(string ticketId)
    {
        var ticket = await _ticketService.GetByIdAsync(ticketId);
        if (ticket == null)
            return NotFound();

        ticket.Status = "Completed";
        await _ticketService.UpdateAsync(ticketId, ticket);

        return RedirectToAction("Details", new { id = ticketId });
    }

    [HttpGet]
    public async Task<IActionResult> Summarize(string ticketId)
    {
        var ticket = await _ticketService.GetByIdWithMessagesAsync(ticketId);
        if (ticket == null)
            return NotFound();

        var payload = new
        {
            Title = ticket.Title,
            Description = ticket.Description,
            Messages = ticket.Messages.Select(m => $"{m.Sender}: {m.Text}")
        };

        var inputJson = System.Text.Json.JsonSerializer.Serialize(payload);
        var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "summary", "t1.py");

        var psi = new ProcessStartInfo
        {
            FileName = "python",
            Arguments = $"\"{scriptPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };  

        string result;
        using (var process = Process.Start(psi))
        {
            using (var sw = process.StandardInput)
            {
                sw.WriteLine(inputJson);
            }
            result = process.StandardOutput.ReadToEnd();
        }

        return Content(result);
    }

    [HttpGet]
    public async Task<JsonResult> GetTicketTrends()
    {
        var tickets = await _ticketService.GetAllAsync();

        var grouped = tickets
            .GroupBy(t => t.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                Available = g.Count(e => e.Status == "Active"),
                Completed = g.Count(e => e.Status == "Completed")
            })
            .OrderBy(g => g.Date)
            .Select(g => new
            {
                date = g.Date.ToString("yyyy-MM-dd"),
                available = g.Available,
                completed = g.Completed
            })
            .ToList();

        return Json(grouped);
    }
}