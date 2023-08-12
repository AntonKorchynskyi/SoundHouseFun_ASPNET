using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundHouseFun.Data;

namespace SoundHouseFun.Controllers
{
    public class ShopController : Controller
    {
        // Property for our database connection
        private ApplicationDbContext _context;

        // Configuration variable for reading appsettings
        private IConfiguration _configuration;

        // Constructor
        public ShopController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration; // make configuration available to controller
        }

        public async Task<IActionResult> Index()
        {
            var albums = await _context.Albums
                .OrderBy(album => album.Name)
                .ToListAsync();

            return View(albums);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var albumWithSongs = await _context.Albums
                .Include(album => album.Songs)
                .FirstOrDefaultAsync(album => album.Id == id);

            return View(albumWithSongs);
        }

    }
}
