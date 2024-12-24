// Data/DbInitializer.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductCatalogApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogApp.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure the database is created
            await context.Database.MigrateAsync();

            // 1. Seed Roles
            string[] roles = new string[] { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Seed Admin User
            var adminEmail = "admin@productcatalog.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                };
                var result = await userManager.CreateAsync(adminUser, "Admin@123"); // Use a strong password in production
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    // Handle errors
                    throw new Exception("Failed to create Admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // 3. Seed Categories
            if (!context.Categories.Any())
            {
                var categories = new Category[]
                {
                    new Category { Name = "Electronics" },
                    new Category { Name = "Books" },
                    new Category { Name = "Clothing" },
                    new Category { Name = "Home & Kitchen" },
                    new Category { Name = "Sports" }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // Retrieve Categories from Database
            var electronics = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Electronics");
            var books = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Books");
            var clothing = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Clothing");
            var homeKitchen = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Home & Kitchen");
            var sports = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Sports");

            if (electronics == null || books == null || clothing == null || homeKitchen == null || sports == null)
            {
                throw new Exception("One or more categories are missing. Ensure that categories are seeded correctly.");
            }

            // 4. Seed Products
            if (!context.Products.Any())
            {
                var currentTime = DateTime.Now;
                var products = new Product[]
                {
                    new Product
                    {
                        Name = "Laptop",
                        Price = 999.99m,
                        StartDate = currentTime.AddDays(-10), // Started 10 days ago
                        Duration = 30,
                        CategoryId = electronics.Id,
                        CreationDate = currentTime.AddDays(-15), // Created 15 days ago
                        EndDate = currentTime.AddDays(20), // Ends in 20 days
                        CreatedByUserId = adminUser.Id
                    },
                    new Product
                    {
                        Name = "Book",
                        Price = 19.99m,
                        StartDate = currentTime.AddDays(-5),
                        Duration = 20,
                        CategoryId = books.Id,
                        CreationDate = currentTime.AddDays(-10),
                        EndDate = currentTime.AddDays(10),
                        CreatedByUserId = adminUser.Id
                    },
                    new Product
                    {
                        Name = "T-Shirt",
                        Price = 14.99m,
                        StartDate = currentTime.AddDays(-2),
                        Duration = 10,
                        CategoryId = clothing.Id,
                        CreationDate = currentTime.AddDays(-7),
                        EndDate = currentTime.AddDays(8),
                        CreatedByUserId = adminUser.Id
                    }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
