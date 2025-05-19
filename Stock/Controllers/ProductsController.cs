using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stock.Data;
using Stock.Models;

namespace Stock.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDBContext _context;

        public ProductsController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stocks = await _context.Stocks.AsNoTracking().ToListAsync();
            Console.WriteLine($"Loaded {stocks.Count} stocks from DB");

            ViewBag.Stocks = stocks;

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Stock)
                .Include(p => p.Supplier)
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Stock)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id");
            ViewData["StockId"] = new SelectList(_context.Stocks, "Id", "Id");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,CategoryId,SupplierId,StockId")] Product product)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", product.CategoryId);
            ViewData["StockId"] = new SelectList(_context.Stocks, "Id", "Id", product.StockId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Id", product.SupplierId);
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", product.CategoryId);
            ViewData["StockId"] = new SelectList(_context.Stocks, "Id", "Id", product.StockId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Id", product.SupplierId);
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,CategoryId,SupplierId,StockId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", product.CategoryId);
            ViewData["StockId"] = new SelectList(_context.Stocks, "Id", "Id", product.StockId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Id", product.SupplierId);
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Stock)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'AppDBContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> GeneratePdfReport(int? stockId)
        {
            var productsQuery = _context.Products
                .Include(e => e.Stock)
                .Include(e => e.Supplier)
                .AsQueryable();

            if (stockId.HasValue)
            {
                productsQuery = productsQuery.Where(e => e.StockId == stockId.Value);
            }

            var products = await productsQuery.ToListAsync();

            using var stream = new MemoryStream();
            var pdfWriter = new PdfWriter(stream);
            var pdfDocument = new PdfDocument(pdfWriter);
            var document = new Document(pdfDocument, PageSize.A4.Rotate()); // Альбомная ориентация

            // Настройка шрифта с поддержкой кириллицы
            var fontPath = "C:\\Windows\\Fonts\\arial.ttf";
            var font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H, pdfDocument);
            document.SetFont(font);

            // Стилизация заголовка
            var titleStyle = new Style()
                .SetFontSize(18)
                .SetBold()
                .SetFontColor(DeviceRgb.BLACK)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(22);

            string reportTitle = stockId.HasValue
                ? $"Отчет по товарам склада: {products.FirstOrDefault()?.Stock?.Name ?? "Неизвестный склад"}"
                : "Отчет по всем товарам";

            document.Add(new Paragraph(reportTitle).AddStyle(titleStyle));

            // Создаем таблицу с товарами
            var table = new Table(UnitValue.CreatePercentArray(new float[] { 40, 30, 30 }), true)
                .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                .SetWidth(UnitValue.CreatePercentValue(90));

            // Стилизация заголовков таблицы
            var headerStyle = new Style()
                .SetBackgroundColor(new DeviceRgb(0, 102, 51)) // SteelBlue
                .SetFontColor(DeviceRgb.WHITE)
                .SetBold()
                .SetPadding(5)
                .SetTextAlignment(TextAlignment.CENTER);

            table.AddHeaderCell(new Cell().Add(new Paragraph("Поставщик")).AddStyle(headerStyle));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Наименование")).AddStyle(headerStyle));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Цена")).AddStyle(headerStyle));

            // Стиль для ячеек с данными
            var cellStyle = new Style()
                .SetPadding(5)
                .SetBorder(new SolidBorder(DeviceRgb.BLACK, 0.5f));

            // Рассчитываем общую сумму
            int totalPrice = 0;

            foreach (var product in products)
            {
                table.AddCell(new Cell().Add(new Paragraph(product.Supplier?.Name ?? "Н/Д")).AddStyle(cellStyle));
                table.AddCell(new Cell().Add(new Paragraph(product.Name)).AddStyle(cellStyle));
                table.AddCell(new Cell().Add(new Paragraph(product.Price.ToString("N0") + " BYN")).AddStyle(cellStyle));
                totalPrice += product.Price;
            }

            document.Add(table);

            // Стиль для итоговой информации
            var totalStyle = new Style()
                .SetFontSize(14)
                .SetBold()
                .SetMarginTop(15)
                .SetTextAlignment(TextAlignment.RIGHT);

            // Добавляем итоговую информацию
            document.Add(new Paragraph($"Общее количество товаров: {products.Count}")
                .AddStyle(totalStyle));

            document.Add(new Paragraph($"Общая стоимость: {totalPrice:N0} BYN")
                .AddStyle(totalStyle));

            // Добавляем дату генерации отчета
            document.Add(new Paragraph($"Отчет сформирован: {DateTime.Now:dd.MM.yyyy HH:mm}")
                .AddStyle(totalStyle.SetFontSize(12).SetItalic()));

            document.Close();

            string fileName = stockId.HasValue
                ? $"Отчет_по_товарам_склад_{stockId.Value}.pdf"
                : "Полный_отчет_по_товарам.pdf";

            return File(stream.ToArray(), "application/pdf", fileName);
        }
    }
}
    