using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Diagnostics;

[Authorize]
public class TicketController : Controller
{
    private readonly ApplicationDbContext _context;

    public TicketController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var tickets = _context.Tickets
            .Include(t => t.Messages)
            .ToList();
        return View(tickets);
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpGet]
public JsonResult GetTicketTrends()
{
    // Step 1: Group by date without formatting and bring into memory
    var groupedData = _context.Tickets
        .GroupBy(t => t.CreatedAt.Date)
        .Select(g => new
        {
            Date = g.Key,
            Available = g.Count(e => e.Status == "Active"),
            Completed = g.Count(e => e.Status == "Completed")
        })
        .OrderBy(g => g.Date)
        .AsEnumerable() // switch to LINQ to Objects
        // Step 2: format date string in memory
        .Select(g => new
        {
            date = g.Date.ToString("yyyy-MM-dd"),
            available = g.Available,
            completed = g.Completed
        })
        .ToList();

    return Json(groupedData);
}


    [HttpPost]
    public IActionResult Create(Ticket ticket)
    {
        ticket.CreatedBy = User.Identity?.Name ?? "Anonymous";
        ticket.CreatedAt = DateTime.Now;
        ticket.Status = "Active";

        _context.Tickets.Add(ticket);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult Details(int id)
    {
        var ticket = _context.Tickets
            .Include(t => t.Messages)
            .FirstOrDefault(t => t.Id == id);

        if (ticket == null)
            return NotFound();

        return View(ticket);
    }

    // Updated to return JSON for AJAX
    [HttpPost]
    public async Task<IActionResult> AddMessage(int ticketId, string msgText, IFormFile? file)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Messages)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
            return NotFound();

        var message = new Message
        {
            TicketId = ticketId,
            Sender = User.Identity?.Name ?? "Anonymous",
            Text = msgText ?? string.Empty,
            Timestamp = DateTime.UtcNow
        };

        if (file != null && file.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            message.FileData = memoryStream.ToArray();
            message.FileName = file.FileName;
            message.ContentType = file.ContentType;
        }

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Return JSON with message data
        return Json(new
        {
            id = message.Id,
            sender = message.Sender,
            text = message.Text,
            timestamp = message.Timestamp,
            fileName = message.FileName,
            fileUrl = message.FileName != null
                ? Url.Action("DownloadFile", "Ticket", new { id = message.Id })
                : null
        });
    }

    public async Task<IActionResult> DownloadFile(int id)
    {
        var message = await _context.Messages.FindAsync(id);
        if (message == null || message.FileData == null)
            return NotFound();

        var contentType = message.ContentType ?? "application/octet-stream";
        var fileName = message.FileName ?? "attachment";

        return File(message.FileData, contentType, fileName);
    }

    [HttpPost]
    public async Task<IActionResult> CloseTicket(int ticketId)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);

        if (ticket == null)
            return NotFound();

        ticket.Status = "Completed";
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = ticketId });
    }
    [HttpGet]
public IActionResult Summarize(int ticketId)
{
    var ticket = _context.Tickets
        .Include(t => t.Messages)
        .FirstOrDefault(t => t.Id == ticketId);

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


}
