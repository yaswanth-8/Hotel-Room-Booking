using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Room_Booking.Data;
using Hotel_Room_Booking.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Hotel_Room_Booking.Controllers
{
    public class CustomerModelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;


        public CustomerModelsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Roles = "admin")]
        // GET: CustomerModels
        public async Task<IActionResult> Index(string searchHotel)
        {
            var hotels = await _context.HotelModel
                .Select(h => h.HotelName)
                .ToListAsync();

            ViewBag.Hotels = new SelectList(hotels.Distinct());

            if (!string.IsNullOrEmpty(searchHotel) && hotels.Contains(searchHotel))
            {
                var customers = await _context.CustomerModel
                    .Include(c => c.User)
                    .Include(c => c.Hotel)
                    .Where(c => c.Hotel.HotelName == searchHotel)
                    .ToListAsync();

                return View(customers);
            }
            else
            {
                var customers = await _context.CustomerModel
                    .Include(c => c.User)
                    .Include(c => c.Hotel)
                    .ToListAsync();

                return View(customers);
            }
        }

        // GET: CustomerModels/Create
        public IActionResult Create()
        {
            ViewData["HotelId"] = new SelectList(_context.HotelModel, "HotelId", "Address");
            return View();
        }

        // POST: CustomerModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("CustomerId,HotelId,Rooms,CheckIn,CheckOut,Rating,Comments")] CustomerModel customerModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            customerModel.User = user;
            customerModel.HotelId = id;
            var hotel = await _context.HotelModel.FindAsync(id);

            if (hotel != null)
            {
                hotel.Rooms -= customerModel.Rooms;
                _context.Update(hotel);
                await _context.SaveChangesAsync();
            }
            _context.Add(customerModel);
            await _context.SaveChangesAsync();

            return Redirect("https://buy.stripe.com/test_00gdRe6Oofdhe5OdQR");
        }

        public async Task<IActionResult> Reviews(int id)
        {
            var reviews = await _context.CustomerModel
                .Include(c => c.User)
                .Where(c => c.HotelId == id && c.Comments != null && c.Comments != "")
                .Select(c => new { UserEmail = c.User.Email, Reviews = c.Comments, Ratings=c.Rating })
                .ToListAsync();
           
            ViewBag.reviews = reviews;
            ViewBag.hotelId = id;
            return View();
        }

        //Profile Section
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            var customers = await _context.CustomerModel
                .Include(c => c.Hotel)
                .Where(c => c.User == user)
                .ToListAsync();

            if (customers == null || customers.Count == 0)
            {
                return RedirectToAction("EmptyProfile");
            }

            return View(customers);
        }

        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            var customer = await _context.CustomerModel.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            var hotel = await _context.HotelModel.FindAsync(customer.HotelId);

            if (hotel != null)
            {
                hotel.Rooms += customer.Rooms;
                _context.Update(hotel);
                await _context.SaveChangesAsync();
            }
            _context.CustomerModel.Remove(customer);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "HotelModels");
        }

        [Authorize]
        public IActionResult EmptyProfile()
        {
            return View();
        }

        [Authorize]
        public IActionResult addReview()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> addReview(int id, int rating, string comment)
        {
            // Find the customer record to update
            var customer = await _context.CustomerModel.FindAsync(id);

            // Update the ratings and comments
            customer.Rating = rating;
            customer.Comments = comment;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the profile page
            return RedirectToAction("Profile");
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> MakeAdmin(string email, string password)
        {
            // Create a new user with the given email and password
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            // If the user was successfully created, add the "admin" role to their account
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "admin");
                return RedirectToAction("Index", "Home");
            }

            // If there was an error creating the user, return an error message
            ModelState.AddModelError(string.Empty, "Error creating user.");
            return View();
        }


        private bool CustomerModelExists(int id)
        {
          return (_context.CustomerModel?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
