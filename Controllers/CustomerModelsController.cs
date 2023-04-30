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

        // GET: CustomerModels
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CustomerModel.Include(c => c.Hotel);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CustomerModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CustomerModel == null)
            {
                return NotFound();
            }

            var customerModel = await _context.CustomerModel
                .Include(c => c.Hotel)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customerModel == null)
            {
                return NotFound();
            }

            return View(customerModel);
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
        public async Task<IActionResult> Create(int id,[Bind("CustomerId,HotelId,Rooms,CheckIn,CheckOut,Rating,Comments")] CustomerModel customerModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            customerModel.User = user;
            customerModel.HotelId = id;
            Console.WriteLine();
            Console.WriteLine(customerModel.HotelId+" "+customerModel.User.Id+" "+customerModel.CheckIn);
            Console.WriteLine();
                _context.Add(customerModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }

        // GET: CustomerModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CustomerModel == null)
            {
                return NotFound();
            }

            var customerModel = await _context.CustomerModel.FindAsync(id);
            if (customerModel == null)
            {
                return NotFound();
            }
            ViewData["HotelId"] = new SelectList(_context.HotelModel, "HotelId", "Address", customerModel.HotelId);
            return View(customerModel);
        }

        // POST: CustomerModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,HotelId,Rooms,CheckIn,CheckOut,Rating,Comments")] CustomerModel customerModel)
        {
            if (id != customerModel.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerModelExists(customerModel.CustomerId))
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
            ViewData["HotelId"] = new SelectList(_context.HotelModel, "HotelId", "Address", customerModel.HotelId);
            return View(customerModel);
        }

        // GET: CustomerModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CustomerModel == null)
            {
                return NotFound();
            }

            var customerModel = await _context.CustomerModel
                .Include(c => c.Hotel)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customerModel == null)
            {
                return NotFound();
            }

            return View(customerModel);
        }

        // POST: CustomerModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CustomerModel == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CustomerModel'  is null.");
            }
            var customerModel = await _context.CustomerModel.FindAsync(id);
            if (customerModel != null)
            {
                _context.CustomerModel.Remove(customerModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Reviews(int id)
        {
            var reviews = await _context.CustomerModel
                .Include(c => c.User)
                .Where(c => c.HotelId == id && c.Comments != null && c.Comments != "")
                .Select(c => new { UserEmail = c.User.Email, Reviews = c.Comments, Ratings=c.Rating })
                .ToListAsync();
           
            ViewBag.reviews = reviews;

            return View();
        }

        //Profile Section
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.CustomerModel
                .Include(c => c.Hotel)
                .FirstOrDefaultAsync(c => c.User == user);

            if (customer == null)
            {
                return RedirectToAction("EmptyProfile");
            }

            return View(customer);
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var customer = await _context.CustomerModel.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.CustomerModel.Remove(customer);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "HotelModels");
        }

        public IActionResult EmptyProfile()
        {
            return View();
        }

        public IActionResult addReview()
        {
            return View();
        }
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
        private bool CustomerModelExists(int id)
        {
          return (_context.CustomerModel?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
