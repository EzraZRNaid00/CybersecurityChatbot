using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase1.Data;
using EventEase1.Models;

namespace EventEase1.Controllers
{
    public class BookingsController : Controller
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Displays all bookings with search functionality
        public async Task<IActionResult> Index(string searchString)
        {
            ViewBag.CurrentFilter = searchString;

            var bookings = _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b =>
                    b.CustomerName.Contains(searchString) ||
                    b.Event.Name.Contains(searchString));
            }

            return View(await bookings.ToListAsync());
        }

        // GET: Shows detailed information for a single booking
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Displays the booking creation form
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Name");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name");

            return View();
        }

        // POST: Saves a new booking to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,VenueId,EventId,CustomerName,NumberOfTickets,BookingDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                // Prevent double bookings - check by DATE only
                bool bookingExists = await _context.Bookings
                    .AnyAsync(b => b.VenueId == booking.VenueId
                                && b.BookingDate.Date == booking.BookingDate.Date);

                if (bookingExists)
                {
                    ModelState.AddModelError("VenueId", "This venue is already booked for the selected date.");
                }
                else
                {
                    _context.Add(booking);
                    await _context.SaveChangesAsync();

                    // Triggers confetti!
                    TempData["SuccessMessage"] = "Booking created successfully! 🎉";

                    return RedirectToAction(nameof(Index));
                }
            }

            // Repopulate dropdowns if validation fails
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Name", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name", booking.VenueId);

            return View(booking);
        }

        // GET: Displays the edit form for a specific booking
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Name", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name", booking.VenueId);

            return View(booking);
        }

        // POST: Updates an existing booking in the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,VenueId,EventId,CustomerName,NumberOfTickets,BookingDate")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Prevent double booking when editing - check by DATE only
                bool bookingExists = await _context.Bookings
                    .AnyAsync(b => b.VenueId == booking.VenueId
                                && b.BookingDate.Date == booking.BookingDate.Date
                                && b.BookingId != booking.BookingId);

                if (bookingExists)
                {
                    ModelState.AddModelError("VenueId", "This venue is already booked for the selected date.");
                }
                else
                {
                    try
                    {
                        _context.Update(booking);
                        await _context.SaveChangesAsync();

                        // Triggers confetti on edit too!
                        TempData["SuccessMessage"] = "Booking updated successfully! 🎉";
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BookingExists(booking.BookingId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "Name", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name", booking.VenueId);

            return View(booking);
        }

        // GET: Displays the delete confirmation page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Permanently removes a booking from the database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Booking deleted successfully!";

            return RedirectToAction(nameof(Index));
        }

        // Helper method to check if a booking exists
        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}