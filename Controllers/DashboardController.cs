using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var activeCount = await _context.Tickets.CountAsync(t => t.Status == "Active");
        var completedCount = await _context.Tickets.CountAsync(t => t.Status == "Completed");

        var model = new DashboardViewModel
        {
            AvailableTicketsCount = activeCount,
            CompletedTicketsCount = completedCount
        };

        return View(model);
    }
}

// ViewModel with property names matching controller usage
public class DashboardViewModel
{
    public int AvailableTicketsCount { get; set; }
    public int CompletedTicketsCount { get; set; }
}
