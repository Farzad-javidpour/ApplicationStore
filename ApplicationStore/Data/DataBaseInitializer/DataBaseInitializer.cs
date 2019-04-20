using ApplicationStore.Models;
using ApplicationStore.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationStore.Data.DataBaseInitializer
{
    public class DataBaseInitializer : IDataBaseInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataBaseInitializer(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async void Initialize()
        {
            _context.Database.Migrate();

            //  اگر نقش سوپر ادمین وجود دارد ریترن کن
            if (_context.Roles.Any(r => r.Name == StaticInfo.SuperAdminUser)) return;


            // ساختن نقش های مورد نیاز
            _roleManager.CreateAsync(new IdentityRole(StaticInfo.AdminUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticInfo.SuperAdminUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticInfo.MemberUser)).GetAwaiter().GetResult();

            // ساختن یک یوزر جدید
            _userManager.CreateAsync(new ApplicationStoreUser
            {
                FirstName = StaticInfo.SuperAdminFirstName,
                LastName = StaticInfo.SuperAdminLastName,
                UserName = StaticInfo.SuperAdminUsername,
                Email = StaticInfo.SuperAdminEmail,
                PhoneNumber = StaticInfo.SuperAdminPhoneNumber,
                EmailConfirmed = true
            },
            password: StaticInfo.SuperAdminPassword
            ).GetAwaiter().GetResult();


            // دادن رول سوپر ادمین به یوزر ساخته شده
            await _userManager.AddToRoleAsync(await _userManager.FindByEmailAsync(StaticInfo.SuperAdminEmail), StaticInfo.SuperAdminUser);
        }
    }
}
