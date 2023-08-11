using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoundHouseFun.Data;
using SoundHouseFun.Models;

namespace SoundHouseFun.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SongsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SongsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Songs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Songs.Include(s => s.Album);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Songs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Songs == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .Include(s => s.Album)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // GET: Songs/Create
        public IActionResult Create()
        {
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Name");
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AlbumId,Name,Genre,Singer,ReleaseDate,Audio")] Song song)
        {


            if (ModelState.IsValid)
            {
                /*
                // Call the UploadAudio method to handle the uploaded file
                var uploadedAudioFileName = await UploadAudio(song.Audio);

                // Handle the result from UploadAudio (e.g., error messages or success)
                if (uploadedAudioFileName == "Invalid audio file format")
                {
                    ModelState.AddModelError("Audio", "Invalid audio file format");
                    // Handle the error, perhaps redisplay the form with the error message
                    return View(song);
                }
                else if (uploadedAudioFileName != null)
                {
                    // The file was successfully uploaded, you can save the file name or perform further actions
                    model.AudioFileName = uploadedAudioFileName;
                    // Save the model to your database or perform other actions
                    // ...
                }*/

                _context.Add(song);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If we got this far, something failed. Log the ModelState errors
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            // You can put a breakpoint here to inspect 'errors' in the debugger
            System.Diagnostics.Debug.WriteLine(errors);

            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Name", song.AlbumId);
            return View(song);
        }

        // GET: Songs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Songs == null)
            {
                return NotFound();
            }

            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Name", song.AlbumId);
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AlbumId,Name,Genre,Singer,ReleaseDate,Audio")] Song song)
        {
            if (id != song.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(song);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(song.Id))
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

            // If we got this far, something failed. Log the ModelState errors
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            // You can put a breakpoint here to inspect 'errors' in the debugger
            System.Diagnostics.Debug.WriteLine(errors);

            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Name", song.AlbumId);
            return View(song);
        }

        // GET: Songs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Songs == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .Include(s => s.Album)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Songs == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Songs'  is null.");
            }
            var song = await _context.Songs.FindAsync(id);
            if (song != null)
            {
                _context.Songs.Remove(song);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SongExists(int id)
        {
          return (_context.Songs?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task<string> UploadAudio(IFormFile Audio)
        {
            if (Audio != null)
            {
                // Check the file extension and content type
                if (!IsAudioFile(Audio.FileName, Audio.ContentType))
                {
                    // Return an error message or throw an exception
                    return "Invalid audio file format";
                }

                // Get temp location
                var filePath = Path.GetTempFileName();

                // Create a unique name so we don't overwrite any existing photos
                // eg: Audio => abcdefg123456890-Audio (Using the Globally Unique Identifier (GUID))
                var fileName = Guid.NewGuid() + "-" + Audio.FileName;

                // Set destination path dynamically so it works on any system (double slashes escape the characters)
                var uploadPath = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\audio\\" + fileName;

                // Execute the file copy
                using var stream = new FileStream(uploadPath, FileMode.Create);
                await Audio.CopyToAsync(stream);

                // Set the Photo property name of the new Song object
                return fileName;
            }

            return null;
        }

        // Used ChatGPT
        // Helper function to check if the file is a valid audio file based on extension and content type
        private bool IsAudioFile(string fileName, string contentType)
        {
            var validExtensions = new[] { ".mp3", ".wav", ".ogg", ".flac", ".aac" };
            var validContentTypes = new[] { "audio/mpeg", "audio/wav", "audio/ogg", "audio/flac", "audio/aac" };

            var extension = Path.GetExtension(fileName);
            return validExtensions.Contains(extension.ToLower()) &&
                   validContentTypes.Contains(contentType.ToLower());
        }
    }
}
