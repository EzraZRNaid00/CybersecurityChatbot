using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase1.Data;
using EventEase1.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace EventEase1.Controllers
{
    public class VenuesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public VenuesController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Displays all venues
        public async Task<IActionResult> Index()
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"]?.ToString();
            }
            return View(await _context.Venues.ToListAsync());
        }

        // GET: Shows detailed information for a single venue
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // GET: Displays the venue creation form
        public IActionResult Create()
        {
            return View();
        }

        // POST: Saves a new venue to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload to Azurite
                if (venue.ImageUpload != null && venue.ImageUpload.Length > 0)
                {
                    string? connectionString = _configuration.GetConnectionString("StorageConnection");

                    if (string.IsNullOrEmpty(connectionString))
                    {
                        ModelState.AddModelError("", "Storage connection string is missing.");
                        return View(venue);
                    }

                    try
                    {
                        string containerName = "venue-images";
                        var blobServiceClient = new BlobServiceClient(connectionString);
                        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                        await containerClient.CreateIfNotExistsAsync();
                        await containerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

                        string fileExtension = Path.GetExtension(venue.ImageUpload.FileName);
                        string fileName = Guid.NewGuid().ToString() + fileExtension;
                        var blobClient = containerClient.GetBlobClient(fileName);

                        using (var stream = venue.ImageUpload.OpenReadStream())
                        {
                            await blobClient.UploadAsync(stream, new BlobHttpHeaders
                            {
                                ContentType = venue.ImageUpload.ContentType
                            });
                        }

                        venue.ImageUrl = blobClient.Uri.ToString();
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Image upload failed: {ex.Message}. Make sure Azurite is running.");
                        return View(venue);
                    }
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        // GET: Displays the edit form for a specific venue
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // POST: Updates an existing venue in the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue)
        {
            if (id != venue.VenueId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle new image upload if provided
                    if (venue.ImageUpload != null && venue.ImageUpload.Length > 0)
                    {
                        string? connectionString = _configuration.GetConnectionString("StorageConnection");

                        if (string.IsNullOrEmpty(connectionString))
                        {
                            ModelState.AddModelError("", "Storage connection string is missing.");
                            return View(venue);
                        }

                        try
                        {
                            string containerName = "venue-images";
                            var blobServiceClient = new BlobServiceClient(connectionString);
                            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                            await containerClient.CreateIfNotExistsAsync();

                            string fileExtension = Path.GetExtension(venue.ImageUpload.FileName);
                            string fileName = Guid.NewGuid().ToString() + fileExtension;
                            var blobClient = containerClient.GetBlobClient(fileName);

                            using (var stream = venue.ImageUpload.OpenReadStream())
                            {
                                await blobClient.UploadAsync(stream, new BlobHttpHeaders
                                {
                                    ContentType = venue.ImageUpload.ContentType
                                });
                            }

                            venue.ImageUrl = blobClient.Uri.ToString();
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", $"Image upload failed: {ex.Message}. Make sure Azurite is running.");
                            return View(venue);
                        }
                    }
                    else
                    {
                        // No new image uploaded — keep the existing ImageUrl
                        var existingVenue = await _context.Venues.AsNoTracking()
                            .FirstOrDefaultAsync(v => v.VenueId == venue.VenueId);
                        if (existingVenue != null)
                        {
                            venue.ImageUrl = existingVenue.ImageUrl;
                        }
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        // GET: Displays the delete confirmation page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // POST: Permanently removes a venue from the database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);

            bool hasBookings = await _context.Bookings.AnyAsync(b => b.VenueId == id);

            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this venue because it has existing bookings. Please delete the bookings first.";
                return RedirectToAction(nameof(Index));
            }

            if (venue != null)
            {
                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
        }
    }
}