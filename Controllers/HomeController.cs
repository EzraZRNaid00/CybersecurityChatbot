using EventEase1.Data;
using EventEase1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EventEase1.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Displays the home/dashboard page
        public IActionResult Index()
        {
            return View();
        }

        // API endpoint: Returns JSON with venue, event, and booking counts for the dashboard
        public async Task<IActionResult> GetCounts()
        {
            var venueCount = await _context.Venues.CountAsync();
            var eventCount = await _context.Events.CountAsync();
            var bookingCount = await _context.Bookings.CountAsync();

            return Json(new { venueCount, eventCount, bookingCount });
        }

        public async Task<IActionResult> GetRecentBookings()
        {
            var recentBookings = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .Select(b => new {
                    customerName = b.CustomerName,
                    eventName = b.Event.Name,
                    venueName = b.Venue.Name,
                    bookingDate = b.BookingDate.ToString("yyyy-MM-dd"),
                    tickets = b.NumberOfTickets
                })
                .ToListAsync();

            return Json(recentBookings);
        }

        // GET: Displays the privacy policy page
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Displays error page when an unhandled exception occurs
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}