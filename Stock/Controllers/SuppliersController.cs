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
    public class SuppliersController : BaseController
    {
        private readonly AppDBContext _context;

        public SuppliersController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return _context.Suppliers != null ?
                        View(await _context.Suppliers.ToListAsync()) :
                        Problem("Entity set 'AppDBContext.Suppliers'  is null.");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supplier == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Where(o => o.SupplierId == id)
                .ToListAsync();


            if (products.Count > 0)
            {
                ViewBag.Products = products;
            }
            else
            {
                ViewBag.Products = null;
            }

            return View(supplier);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email")] Supplier supplier)
        {
            _context.Add(supplier);
            await _context.SaveChangesAsync();
            return View(supplier);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email")] Supplier supplier)
        {
            if (id != supplier.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(supplier);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(supplier.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return View(supplier);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Suppliers == null)
            {
                return Problem("Entity set 'AppDBContext.Suppliers'  is null.");
            }
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(int id)
        {
            return (_context.Suppliers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
