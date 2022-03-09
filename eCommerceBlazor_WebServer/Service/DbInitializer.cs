﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using eCommerceBlazor_Common;
using eCommerceBlazor_DataAccess.Data;
using eCommerceBlazor_WebServer.Service.IService;

namespace eCommerceBlazor_WebServer.Service
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
                if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                }
                else
                {
                    return;
                }

                IdentityUser user = new()
                {
                    UserName = "jimmy@purnellsoftwaredevelopment.com",
                    Email = "jimmy@purnellsoftwaredevelopment.com",
                    EmailConfirmed = true
                };

                _userManager.CreateAsync(user, "Admin123").GetAwaiter().GetResult();

                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
