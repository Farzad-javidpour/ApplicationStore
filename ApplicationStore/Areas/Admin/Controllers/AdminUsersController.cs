using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationStore.Data;
using ApplicationStore.Models;
using ApplicationStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApplicationStore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Super Admin")]
    [Area("Admin")]
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await (from user in _context.ApplicationStoreUsers
                               join userRole in _context.UserRoles
                               on user.Id equals userRole.UserId
                               join role in _context.Roles
                               on userRole.RoleId equals role.Id
                               select new ApplicationStoreUser
                               {
                                   Id = user.Id,
                                   FirstName = user.FirstName,
                                   LastName = user.LastName,
                                   Email = user.Email,
                                   PhoneNumber = user.PhoneNumber,
                                   MemberType = (role.Name == "Member") ? MemberTypeEnum.Member : (role.Name == "Admin") ? MemberTypeEnum.Admin : (role.Name == "Super Admin") ? MemberTypeEnum.SuperAdmin : 0
                               }
                               ).ToListAsync();

            return View(users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }

            var adminUser = await _context.ApplicationStoreUsers.FindAsync(id);
            if (adminUser == null)
            {
                return NotFound();
            }
            return View(adminUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationStoreUser applicationStoreUser)
        {
            if (id != applicationStoreUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.ApplicationStoreUsers.FirstOrDefaultAsync(i => i.Id == applicationStoreUser.Id);
                    user.FirstName = applicationStoreUser.FirstName;
                    user.LastName = applicationStoreUser.LastName;
                    user.PhoneNumber = applicationStoreUser.PhoneNumber;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationStoreUserExists(applicationStoreUser.Id))
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
            return View(applicationStoreUser);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }

            var adminUser = await _context.ApplicationStoreUsers.FindAsync(id);
            if (adminUser == null)
            {
                return NotFound();
            }
            return View(adminUser);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(string id)
        {
            try
            {
                var user = await _context.ApplicationStoreUsers.FirstOrDefaultAsync(i => i.Id == id);
                user.LockoutEnd = DateTime.Now.AddYears(1000);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return View(id);
            }
        }


        private bool ApplicationStoreUserExists(string id)
        {
            return _context.ApplicationStoreUsers.Any(e => e.Id == id);
        }
    }
}