using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stock.Data;
using Stock.Models;

namespace Stock.Controllers
{
    public class DeliveriesController : BaseController
    {
        private readonly AppDBContext _context;

        public DeliveriesController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return _context.Delivery != null ?
                        View(await _context.Delivery.ToListAsync()) :
                        Problem("Entity set 'AppDBContext.Delivery'  is null.");
        }

        public async Task<IActionResult> GetOrderByStatus (string statusName)
        {
            var delivereis = await _context.Delivery
                .Where(delivereis => delivereis.Status.Contains(statusName))
                .ToListAsync();

            ViewBag.Delivereis = delivereis;

            return View("Index", await _context.Delivery.ToListAsync()); 
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Delivery == null)
            {
                return NotFound();
            }

            var delivery = await _context.Delivery
                .FirstOrDefaultAsync(m => m.Id == id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderDate,DeliveryDate,Status")] Delivery delivery)
        {
            _context.Add(delivery);
            await _context.SaveChangesAsync();
            return View(delivery);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Delivery == null)
            {
                return NotFound();
            }

            var delivery = await _context.Delivery.FindAsync(id);
            if (delivery == null)
            {
                return NotFound();
            }
            return View(delivery);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,DeliveryDate,Status")] Delivery delivery)
        {
            if (id != delivery.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(delivery);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeliveryExists(delivery.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return View(delivery);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Delivery == null)
            {
                return NotFound();
            }

            var delivery = await _context.Delivery
                .FirstOrDefaultAsync(m => m.Id == id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Delivery == null)
            {
                return Problem("Entity set 'AppDBContext.Delivery'  is null.");
            }
            var delivery = await _context.Delivery.FindAsync(id);
            if (delivery != null)
            {
                _context.Delivery.Remove(delivery);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeliveryExists(int id)
        {
            return (_context.Delivery?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
