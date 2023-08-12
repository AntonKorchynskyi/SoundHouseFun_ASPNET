using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoundHouseFun.Data;
using SoundHouseFun.Models;
using System.Security.Claims;

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

        public async Task<IActionResult> SongDetails(int? id)
        {
            var song = await _context.Songs
                .FirstOrDefaultAsync(song => song.Id == id);

            return View(song);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int songId, int quantity)
        {
            // Get our logged in user
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Attempt to get a cart
            var cart = await _context.Carts
                .FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);

            // Check that we have don't have an active cart
            if (cart == null)
            {
                cart = new Models.Cart { UserId = userId };
                await _context.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            var song = await _context.Songs
                .FirstOrDefaultAsync(song => song.Id == songId);

            if (song == null)
            {
                return NotFound();
            }

            // Create a new cart item
            var cartItem = new CartItem
            {
                Cart = cart,
                Song = song,
                Quantity = quantity,
                Price = song.Price,
            };

            // If valid, do all the goodness
            if (ModelState.IsValid)
            {
                await _context.AddAsync(cartItem);
                await _context.SaveChangesAsync();

                return RedirectToAction("ViewMyCart");
            }

            // Otherwise, GTFO
            return NotFound();
        }

        [Authorize]
        public async Task<IActionResult> ViewMyCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = await _context.Carts
                .Include(cart => cart.User)
                .Include(cart => cart.CartItems)
                .ThenInclude(cartItem => cartItem.Song)
                .FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);

            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteCartItem(int cartItemId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = await _context.Carts
                .FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);

            if (cart == null) return NotFound();

            var cartItem = await _context.CartItems
                .Include(cartItem => cartItem.Song)
                .FirstOrDefaultAsync(cartItem => cartItem.Cart == cart && cartItem.Id == cartItemId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                return RedirectToAction("ViewMyCart");
            }

            return NotFound();
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = await _context.Carts
                .Include(cart => cart.User)
                .Include(cart => cart.CartItems)
                .ThenInclude(cartItem => cartItem.Song)
                .FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);

            var order = new Order
            {
                UserId = userId,
                Cart = cart,
                Total = ((decimal)(cart.CartItems.Sum(cartItem => (cartItem.Price * cartItem.Quantity)))),
                PaymentMethod = PaymentMethods.VISA,
            };

            ViewData["PaymentMethods"] = new SelectList(Enum.GetValues(typeof(PaymentMethods)));

            return View(order);
        }

    }
}
