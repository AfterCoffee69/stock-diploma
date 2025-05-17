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
    public class StockModelsController : Controller
    {
        private readonly AppDBContext _context;

        public StockModelsController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;

            var suppliers = await _context.Suppliers.ToListAsync();
            ViewBag.Suppliers = suppliers;

            return View(await _context.Stocks.ToListAsync());
        }

        public async Task<IActionResult> GetProductsByPrice(decimal price)
        {
            var products = await _context.Stocks
                .Include(stock => stock.Products)
                .SelectMany(stock => stock.Products)
                .Where(product => product.Price > price)
                .ToListAsync();

            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            var suppliers = await _context.Suppliers.ToListAsync();
            ViewBag.Suppliers = suppliers;

            ViewBag.Products = products;

            return View("Index", await _context.Stocks.ToListAsync());
        }

        public async Task<IActionResult> GetProductsByStockName(string stockName)
        {
            var products = await _context.Products
                .Where(product => product.Stock.Name.Contains(stockName))
                .ToListAsync();

            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            var suppliers = await _context.Suppliers.ToListAsync();
            ViewBag.Suppliers = suppliers;


            ViewBag.Products = products;

            return View("Index", await _context.Stocks.ToListAsync());
        }

        public async Task<IActionResult> GetProductsByName(string name)
        {
            var products = await _context.Products
                .Where(product => product.Name.Contains(name))
                .ToListAsync();

            ViewBag.Products = products;

            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            var suppliers = await _context.Suppliers.ToListAsync();
            ViewBag.Suppliers = suppliers;

            return View("Index", await _context.Stocks.ToListAsync());
        }

        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await _context.Products
                .Where(product => product.CategoryId == categoryId)
                .ToListAsync();

            ViewBag.Products = products;

            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            var suppliers = await _context.Suppliers.ToListAsync();
            ViewBag.Suppliers = suppliers;

            return View("Index", await _context.Stocks.ToListAsync());
        }
        public async Task<IActionResult> GetProductsBySupplier(int supplierId)
        {
            var products = await _context.Products
                .Where(product => product.SupplierId == supplierId)
                .ToListAsync();

            ViewBag.Products = products;

            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;

            var suppliers = await _context.Suppliers.ToListAsync();
            ViewBag.Suppliers = suppliers;

            return View("Index", await _context.Stocks.ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Stocks == null)
            {
                return NotFound();
            }

            var stockModel = await _context.Stocks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockModel == null)
            {
                return NotFound();
            }

            return View(stockModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Addres")] StockModel stockModel)
        {
            _context.Add(stockModel);
            await _context.SaveChangesAsync();
            return View(stockModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Stocks == null)
            {
                return NotFound();
            }

            var stockModel = await _context.Stocks.FindAsync(id);
            if (stockModel == null)
            {
                return NotFound();
            }
            return View(stockModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Addres")] StockModel stockModel)
        {
            if (id != stockModel.Id)
            {
                return NotFound();
            }
            try
            {
                _context.Update(stockModel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockModelExists(stockModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return View(stockModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Stocks == null)
            {
                return NotFound();
            }

            var stockModel = await _context.Stocks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockModel == null)
            {
                return NotFound();
            }

            return View(stockModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Stocks == null)
            {
                return Problem("Entity set 'AppDBContext.Stocks'  is null.");
            }
            var stockModel = await _context.Stocks.FindAsync(id);
            if (stockModel != null)
            {
                _context.Stocks.Remove(stockModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockModelExists(int id)
        {
            return (_context.Stocks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
