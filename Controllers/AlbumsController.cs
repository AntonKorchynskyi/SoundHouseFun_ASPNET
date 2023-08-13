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
using System.Diagnostics;
using Microsoft.Extensions.Hosting.Internal;


namespace SoundHouseFun.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AlbumsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AlbumsController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Albums
        public async Task<IActionResult> Index()
        {
              return _context.Albums != null ? 
                          View(await _context.Albums.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Albums'  is null.");
        }

        // GET: Albums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .FirstOrDefaultAsync(m => m.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // GET: Albums/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Photo")] Album album, IFormFile? Photo)
        {
            if (ModelState.IsValid)
            {
                // Upload the photo
                var photoFileName = await UploadPhoto(Photo);
                if (photoFileName != null)
                {
                    // Save the photo filename in the album object
                    album.Photo = photoFileName;
                }

                _context.Add(album);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            /* if (ModelState.IsValid)
            {
                _context.Add(album);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            } */

            // If we got this far, something failed. Log the ModelState errors
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            // You can put a breakpoint here to inspect 'errors' in the debugger
            System.Diagnostics.Debug.WriteLine(errors);

            return View(album);
        }

        // GET: Albums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }

            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Photo")] Album album)
        {
            if (id != album.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(album);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(album.Id))
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

            return View(album);
        }

        // GET: Albums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .FirstOrDefaultAsync(m => m.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Albums == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Albums'  is null.");
            }
            var album = await _context.Albums.FindAsync(id);
            if (album != null)
            {
                _context.Albums.Remove(album);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlbumExists(int id)
        {
          return (_context.Albums?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task<string> UploadPhoto(IFormFile Photo)
        {
            Debug.WriteLine("step 1");

            if (Photo != null)
            {
                Debug.WriteLine("step 2");
                // Get temp location
                var filePath = Path.GetTempFileName();
                Debug.WriteLine(filePath);
                // Create a unique name so we don't overwrite any existing photos
                // eg: photo.jpg => abcdefg123456890-photo.jpg (Using the Globally Unique Identifier (GUID))
                var fileName = Guid.NewGuid() + "-" + Photo.FileName;
                Debug.WriteLine(fileName);
                // Set destination path dynamically so it works on any system (double slashes escape the characters)
                var uploadPath = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\img\\albums\\" + fileName;
                Debug.WriteLine(uploadPath);
                // Execute the file copy
                using var stream = new FileStream(uploadPath, FileMode.Create);
                await Photo.CopyToAsync(stream);

                // Set the Photo property name of the new Song object
                return fileName;
            }

            return null;
        }
    }
}
