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
    public class EmployersController : BaseController
    {
        private readonly AppDBContext _context;

        public EmployersController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var appDBContext = _context.Employers.Include(e => e.Stock);
            return View(await appDBContext.ToListAsync());
        }

        public async Task<IActionResult> GetEmployeesByNumber (int eNumber)
        {
            var employee = await _context.Employers
                .Where(employee => employee.Number == eNumber)
                .Include(employees => employees.Stock)
                .ToListAsync();

            ViewBag.Employers = employee;

            return View("Index", await _context.Employers.ToListAsync());
        }

        public async Task<IActionResult> GetEmployeesByName(string name)
        {
            var employees = await _context.Employers
                .Where(employee => employee.Name.Contains(name))
                .Include(employees => employees.Stock)
                .ToListAsync();

            ViewBag.Employers = employees;

            return View("Index", await _context.Employers.ToListAsync()); 
        }

        public async Task<IActionResult> GetEmployeesBySurename(string surename)
        {
            var employees = await _context.Employers
                .Where(employee => employee.Surename.Contains(surename))
                .Include(employees => employees.Stock)
                .ToListAsync();

            ViewBag.Employers = employees;

            return View("Index", await _context.Employers.ToListAsync());
        }

        public async Task<IActionResult> GetEmployeesByDateConnection(DateTime dateConnection)
        {
            var employees = await _context.Employers
                .Where(employee => employee.DateConnection == dateConnection)
                .Include(employees => employees.Stock)
                .ToListAsync();

            ViewBag.Employers = employees;

            return View("Index", await _context.Employers.ToListAsync());
        }

        public async Task<IActionResult> GetEmployeesByStockId(int stockId)
        {
            var employees = await _context.Employers
                .Where(employee => employee.StockId == stockId)
                .Include(employees => employees.Stock)
                .ToListAsync();

            ViewBag.Employers = employees;

            return View("Index", await _context.Employers.ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employers == null)
            {
                return NotFound();
            }

            var employer = await _context.Employers
                .Include(e => e.Stock)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employer == null)
            {
                return NotFound();
            }

            return View(employer);
        }

        public IActionResult Create()
        {
            ViewData["StockId"] = new SelectList(_context.Stocks, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surename,Number,DateConnection,StockId")] Employer employer)
        {
            _context.Add(employer);
            await _context.SaveChangesAsync();
            ViewData["StockId"] = new SelectList(_context.Stocks, "Id", "Id", employer.StockId);
            return View(employer);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employers == null)
            {
                return NotFound();
            }

            var employer = await _context.Employers.FindAsync(id);
            if (employer == null)
            {
                return NotFound();
            }
            ViewData["StockId"] = new SelectList(_context.Stocks, "Id", "Id", employer.StockId);
            return View(employer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surename,Number,DateConnection,StockId")] Employer employer)
        {
            if (id != employer.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(employer);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployerExists(employer.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            ViewData["StockId"] = new SelectList(_context.Stocks, "Id", "Id", employer.StockId);
            return View(employer);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employers == null)
            {
                return NotFound();
            }

            var employer = await _context.Employers
                .Include(e => e.Stock)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employer == null)
            {
                return NotFound();
            }

            return View(employer);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employers == null)
            {
                return Problem("Entity set 'AppDBContext.Employers'  is null.");
            }
            var employer = await _context.Employers.FindAsync(id);
            if (employer != null)
            {
                _context.Employers.Remove(employer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployerExists(int id)
        {
            return (_context.Employers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
