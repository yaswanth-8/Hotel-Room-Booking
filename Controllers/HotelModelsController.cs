using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Room_Booking.Data;
using Hotel_Room_Booking.Models;
using Newtonsoft.Json;

namespace Hotel_Room_Booking.Controllers
{
    public class HotelModelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        Uri baseAddress = new Uri("https://localhost:7258/api");
        HttpClient client;

        public HotelModelsController(ApplicationDbContext context)
        {
            client = new HttpClient();
            client.BaseAddress = baseAddress;
            _context = context;
        }

        // GET: HotelModels
        public async Task<IActionResult> Index(string searchCity)
        {
            List<HotelModel> hotels = new List<HotelModel>();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/HotelApi").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                hotels = JsonConvert.DeserializeObject<List<HotelModel>>(data);
            }
           
            var cities = new SelectList(hotels.Select(h => h.City).Distinct().OrderBy(c => c));

            ViewBag.Cities = cities;
            if (!string.IsNullOrEmpty(searchCity))
            {
                hotels = hotels.Where(h => h.City == searchCity).OrderBy(h => h.Price).ToList();
                return View(hotels);
            }

            return hotels != null ? 
                          View( hotels) :
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
            string data = JsonConvert.SerializeObject(hotelModel);
            StringContent content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(client.BaseAddress + "/HotelApi", content).Result;

            if (response.IsSuccessStatusCode)
            {
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
