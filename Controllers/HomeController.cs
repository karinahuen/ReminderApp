using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ReminderApp.Models;
using Microsoft.AspNetCore.Identity; 
using System.Security.Claims; 
using ReminderApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ReminderApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

    public async Task<IActionResult> Index()
    {
        //Check if user is logged in
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            // Get the current User ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch reminders for THIS user only
            var reminders = await _context.Reminders
                .Where(r => r.OwnerId == userId)
                .ToListAsync();

            // Send the list to the View
            return View(reminders);
        }

        // If not logged in, send an empty list (Prevents NullReferenceException)
        return View(new List<Reminder>());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Description,Platform")] Reminder reminder)
    {
        if (ModelState.IsValid)
        {
            // Assign current user as owner
            reminder.OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _context.Add(reminder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(reminder);
    }

    public async Task<IActionResult> Edit(int? id)
{
    if (id == null) return NotFound();

    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var reminder = await _context.Reminders
        .FirstOrDefaultAsync(m => m.Id == id && m.OwnerId == userId);

    if (reminder == null) return NotFound();
    
    return View(reminder);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Platform")] Reminder reminder)
{
    if (id != reminder.Id) return NotFound();

    if (ModelState.IsValid)
    {
        try
        {
            // Re-assign OwnerId to ensure security
            reminder.OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _context.Update(reminder);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Reminders.Any(e => e.Id == reminder.Id)) return NotFound();
            else throw;
        }
        return RedirectToAction(nameof(Index));
    }
    return View(reminder);
}

public async Task<IActionResult> Delete(int? id)
{
    if (id == null) return NotFound();

    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var reminder = await _context.Reminders
        .FirstOrDefaultAsync(m => m.Id == id && m.OwnerId == userId);

    if (reminder == null) return NotFound();

    return View(reminder);
}

[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var reminder = await _context.Reminders
        .FirstOrDefaultAsync(m => m.Id == id && m.OwnerId == userId);

    if (reminder != null)
    {
        _context.Reminders.Remove(reminder);
        await _context.SaveChangesAsync();
    }
    return RedirectToAction(nameof(Index));
}


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
