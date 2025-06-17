using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SampleMvcApp.Models;  // Adjust namespace as needed
using SampleMvcApp.Services;
public class DashboardController : Controller
{
    private readonly TicketService _ticketService;

    public DashboardController(TicketService ticketService)
    {
        _ticketService = ticketService;
    }

    public async Task<IActionResult> Index()
    {
        var activeCount = await _ticketService.CountByStatusAsync("Active");
        var completedCount = await _ticketService.CountByStatusAsync("Completed");

        var model = new DashboardViewModel
        {
            AvailableTicketsCount = activeCount,
            CompletedTicketsCount = completedCount
        };

        return View(model);
    }
}


public class DashboardViewModel
{
    public int AvailableTicketsCount { get; set; }
    public int CompletedTicketsCount { get; set; }
}
