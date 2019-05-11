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
using System.Globalization;

namespace ApplicationStore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Super Admin")]
    [Area("Admin")]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public ApplicationViewModel ApplicationVM { get; set; }

        public ApplicationsController(ApplicationDbContext context)
        {
            _context = context;

            ApplicationVM = new ApplicationViewModel()
            {
                Application = new Application(),
                ApplicationCategories = _context.ApplicationCategories.ToList()
            };
        }
        //________________________________________________________________________________
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Applications
                .Include(a => a.ApplicationCategory)
                .Include(a => a.ApplicationStoreUser);

            var applicationVMList = new List<ApplicationViewModel>();
            foreach (var item in await applicationDbContext.ToListAsync())
            {
                applicationVMList.Add(
                    new ApplicationViewModel()
                    {
                        Application = item,
                        RegisterDateShamsi = Persia.Calendar.ConvertToPersian(item.RegisterDate).ToString()
                    });
            }

            return View(applicationVMList);
        }
        //________________________________________________________________________________
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Applications
                .Include(a => a.ApplicationCategory)
                .Include(a => a.ApplicationStoreUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (application == null)
            {
                return NotFound();
            }

            ApplicationVM.Application = application;
            ApplicationVM.RegisterDateShamsi = Persia.Calendar.ConvertToPersian(application.RegisterDate).ToString();

            return View(ApplicationVM);
        }
        //________________________________________________________________________________
        public IActionResult Create()
        {
            ViewBag.UserId = Tools.GetCurrenyUserId(User);
            return View(ApplicationVM);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            if (ModelState.IsValid)
            {
                var application = new Application()
                {
                    Title = ApplicationVM.Application.Title.Trim(),
                    Code = ApplicationVM.Application.Code.Trim(),
                    RegisterDate = DateTime.Now.Date,
                    Description = ApplicationVM.Application.Description.Trim(),
                    State = RowStateEnum.Active,
                    ApplicationCategoryId = ApplicationVM.Application.ApplicationCategoryId,
                    ApplicationStoreUserId = ApplicationVM.Application.ApplicationStoreUserId

                };

                _context.Add(application);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ApplicationVM.ApplicationCategories = _context.ApplicationCategories.ToList();
            return View(ApplicationVM);
        }
        //________________________________________________________________________________
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.UserId = Tools.GetCurrenyUserId(User);
            var application = await _context.Applications
                .Include(a => a.ApplicationCategory)
                .Include(a => a.ApplicationStoreUser)
                .SingleOrDefaultAsync(a => a.Id == id);
            if (application == null)
            {
                return NotFound();
            }

            ApplicationVM.Application = application;
            ApplicationVM.ApplicationCategories = _context.ApplicationCategories.ToList();

            return View(ApplicationVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind] Application application)
        {
            if (id != application.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(application);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationExists(application.Id))
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
            ViewData["ApplicationCategoryId"] = new SelectList(_context.ApplicationCategories, "Id", "Title", application.ApplicationCategoryId);
            return View(application);
        }
        //________________________________________________________________________________
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Applications
                .Include(a => a.ApplicationCategory)
                .Include(a => a.ApplicationStoreUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (application == null)
            {
                return NotFound();
            }

            ApplicationVM.Application = application;
            ApplicationVM.RegisterDateShamsi = Persia.Calendar.ConvertToPersian(application.RegisterDate).ToString();

            return View(ApplicationVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            application.State = RowStateEnum.Deleted;
            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //________________________________________________________________________________
        private bool ApplicationExists(int id)
        {
            return _context.Applications.Any(e => e.Id == id);
        }
    }
}
