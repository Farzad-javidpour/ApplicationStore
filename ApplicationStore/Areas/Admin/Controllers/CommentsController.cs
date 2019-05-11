using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationStore.Data;
using ApplicationStore.Models;
using Microsoft.AspNetCore.Authorization;
using ApplicationStore.Models.ViewModels;

namespace ApplicationStore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Super Admin")]
    [Area("Admin")]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        //________________________________________________________________________________
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Comments.Include(c => c.ApplicationPublish).Include(c => c.ApplicationStoreUser);
            return View(await applicationDbContext.ToListAsync());
        }
        //________________________________________________________________________________
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.ApplicationPublish)
                .Include(c => c.ApplicationStoreUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }
        //________________________________________________________________________________
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c=>c.ApplicationPublish)
                .Include(c=>c.ApplicationStoreUser)
                .SingleOrDefaultAsync(c=>c.Id == id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewBag.ApplicationPublishId = new SelectList(_context.ApplicationPublishs, "Id", "Version", comment.ApplicationPublishId);
            ViewBag.UserId = new SelectList(_context.ApplicationStoreUsers, "Id", "UserName", comment.ApplicationStoreUserId);
            return View(comment);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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
            ViewData["ApplicationPublishId"] = new SelectList(_context.ApplicationPublishs, "Id", "Version", comment.ApplicationPublishId);
            ViewData["UserId"] = new SelectList(_context.ApplicationStoreUsers, "Id", "Username", comment.ApplicationStoreUserId);
            return View(comment);
        }
        //________________________________________________________________________________
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.ApplicationPublish)
                .Include(c => c.ApplicationStoreUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            comment.CommentState = CommentStateEnum.Deleted;
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //________________________________________________________________________________
        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
