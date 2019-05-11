using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationStore.Data;
using ApplicationStore.Models;
using ApplicationStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using ApplicationStore.Utility;

namespace ApplicationStore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Super Admin")]
    [Area("Admin")]
    public class ApplicationCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        //________________________________________________________________________________
        public async Task<IActionResult> Index()
        {
            return View(await _context.ApplicationCategories.Include(m=>m.ApplicationStoreUser).ToListAsync());
        }
        //________________________________________________________________________________
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationCategory = await _context.ApplicationCategories
                .Include(m=>m.ApplicationStoreUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationCategory == null)
            {
                return NotFound();
            }

            return View(applicationCategory);
        }
        //________________________________________________________________________________
        public IActionResult Create()
        {
            ViewBag.UserId = Tools.GetCurrentUserId(User);
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] ApplicationCategory applicationCategory)
        {
            if (ModelState.IsValid)
            {
                applicationCategory.State = RowStateEnum.Active;
                _context.Add(applicationCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(applicationCategory);
        }
        //________________________________________________________________________________
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.UserId = Tools.GetCurrentUserId(User);
            var applicationCategory = await _context.ApplicationCategories
                .Include(m => m.ApplicationStoreUser)
                .SingleOrDefaultAsync(m=>m.Id == id);
            if (applicationCategory == null)
            {
                return NotFound();
            }
            return View(applicationCategory);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind] ApplicationCategory applicationCategory)
        {
            if (id != applicationCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicationCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationCategoryExists(applicationCategory.Id))
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
            return View(applicationCategory);
        }
        //________________________________________________________________________________
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationCategory = await _context.ApplicationCategories
                .Include(m=>m.ApplicationStoreUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationCategory == null)
            {
                return NotFound();
            }

            return View(applicationCategory);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var applicationCategory = await _context.ApplicationCategories.FindAsync(id);
            applicationCategory.State = RowStateEnum.Deleted;
            _context.ApplicationCategories.Update(applicationCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //________________________________________________________________________________
        private bool ApplicationCategoryExists(int id)
        {
            return _context.ApplicationCategories.Any(e => e.Id == id);
        }
    }
}
