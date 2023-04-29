using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Room_Booking.Data;
using Hotel_Room_Booking.Models;

namespace Hotel_Room_Booking.Controllers
{
    public class HotelModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HotelModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HotelModels
        public async Task<IActionResult> Index(string searchCity)
        {
            var hotels = await _context.HotelModel.ToListAsync();
            var cities = new SelectList(hotels.Select(h => h.City).Distinct());
            ViewBag.Cities = cities;
            if (!string.IsNullOrEmpty(searchCity))
            {
                hotels = hotels.Where(h => h.City == searchCity).OrderBy(h => h.Price).ToList();
                return View(hotels);
            }

            return _context.HotelModel != null ? 
                          View(await _context.HotelModel.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.HotelModel'  is null.");
        }

        // GET: HotelModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.HotelModel == null)
            {
                return NotFound();
            }

            var hotelModel = await _context.HotelModel
                .FirstOrDefaultAsync(m => m.HotelId == id);
            if (hotelModel == null)
            {
                return NotFound();
            }

            return View(hotelModel);
        }

        // GET: HotelModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HotelModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HotelId,City,Address,HotelName,Rooms,Image,Description,Price")] HotelModel hotelModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hotelModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hotelModel);
        }

        // GET: HotelModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.HotelModel == null)
            {
                return NotFound();
            }

            var hotelModel = await _context.HotelModel.FindAsync(id);
            if (hotelModel == null)
            {
                return NotFound();
            }
            return View(hotelModel);
        }

        // POST: HotelModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HotelId,City,Address,HotelName,Rooms,Image,Description,Price")] HotelModel hotelModel)
        {
            if (id != hotelModel.HotelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hotelModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelModelExists(hotelModel.HotelId))
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
            return View(hotelModel);
        }

        // GET: HotelModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.HotelModel == null)
            {
                return NotFound();
            }

            var hotelModel = await _context.HotelModel
                .FirstOrDefaultAsync(m => m.HotelId == id);
            if (hotelModel == null)
            {
                return NotFound();
            }

            return View(hotelModel);
        }

        // POST: HotelModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.HotelModel == null)
            {
                return Problem("Entity set 'ApplicationDbContext.HotelModel'  is null.");
            }
            var hotelModel = await _context.HotelModel.FindAsync(id);
            if (hotelModel != null)
            {
                _context.HotelModel.Remove(hotelModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HotelModelExists(int id)
        {
          return (_context.HotelModel?.Any(e => e.HotelId == id)).GetValueOrDefault();
        }
    }
}
