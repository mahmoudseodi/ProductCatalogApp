// Controllers/ProductsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductCatalogApp.Data;
using ProductCatalogApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogApp.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Products/Index (Available to all users)
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? categoryId)
        {
            var currentTime = DateTime.Now;
            var productsQuery = _context.Products
                .Include(p => p.Category)
                .Where(p => p.StartDate <= currentTime && (p.EndDate == null || p.EndDate >= currentTime))
                .AsQueryable();

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            var products = await productsQuery.ToListAsync();
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

            var viewModel = new ProductCatalogViewModel
            {
                SelectedCategoryId = categoryId,
                Products = products,
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == categoryId
                })
            };

            return View(viewModel);
        }

        // GET: Products/AdminIndex (Admins Only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex(int? categoryId)
        {
            var productsQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.CreatedByUser)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            var products = await productsQuery.ToListAsync();
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

            var viewModel = new ProductCatalogViewModel
            {
                SelectedCategoryId = categoryId,
                Products = products,
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == categoryId
                })
            };

            return View(viewModel);
        }

        // GET: Products/Details/5 (Available to all users)
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET: Products/Create (Admins Only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create (Admins Only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,StartDate,Duration,Price,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreationDate = DateTime.Now;
                product.CreatedByUserId = _userManager.GetUserId(User);
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AdminIndex));
            }

            // If model state is invalid, repopulate categories
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5 (Admins Only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5 (Admins Only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,Duration,Price,CategoryId")] Product product)
        {
            if (id != product.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.FindAsync(id);
                    if (existingProduct == null)
                        return NotFound();

                    existingProduct.Name = product.Name;
                    existingProduct.StartDate = product.StartDate;
                    existingProduct.Duration = product.Duration;
                    existingProduct.Price = product.Price;
                    existingProduct.CategoryId = product.CategoryId;

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(AdminIndex));
            }

            // If model state is invalid, repopulate categories
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5 (Admins Only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5 (Admins Only)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(AdminIndex));
        }

        // Helper method to check if a product exists
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
