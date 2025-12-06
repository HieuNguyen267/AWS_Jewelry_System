using Jewelry_Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace Jewelry_Model.Utils;

public static class JewelryAwsContextExtensions
{
    public static async Task SeedDefaultData(this JewelryAwsContext context)
    {
        // Đảm bảo DB đã tạo
        await context.Database.MigrateAsync();

        // ========== SEED ACCOUNT ==========
        if (!context.Accounts.Any())
        {
            context.Accounts.Add(new Account
            {
                Id = Guid.NewGuid(),
                Email = "admin@example.com",
                Password = "123456", // nhớ hash khi làm thật
                Role = "Admin",
                FullName = "Admin User",
                Address = "N/A",
                Phone = "0000000000",
                IsActive = true,
                CreateAt = DateTime.UtcNow
            });
        }

        // ========== SEED SIZE ==========
        if (!context.Sizes.Any())
        {
            var sizes = new List<Size>
            {
                new Size { Id = Guid.NewGuid(), Label = "6", Type = "Ring", IsActive = true, CreateAt = DateTime.UtcNow },
                new Size { Id = Guid.NewGuid(), Label = "7", Type = "Ring", IsActive = true, CreateAt = DateTime.UtcNow },
                new Size { Id = Guid.NewGuid(), Label = "18cm", Type = "Bracelet", IsActive = true, CreateAt = DateTime.UtcNow },
                new Size { Id = Guid.NewGuid(), Label = "45cm", Type = "Chain", IsActive = true, CreateAt = DateTime.UtcNow }
            };
            context.Sizes.AddRange(sizes);
        }

        // ========== SEED PRODUCT + PRODUCT SIZE ==========
        if (!context.Products.Any())
        {
            var product1 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Diamond Ring",
                Description = "A beautiful diamond ring.",
                Image = "diamond_ring.jpg",
                IsNew = true,
                IsActive = true,
                CreateAt = DateTime.UtcNow
            };

            var product2 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Gold Necklace",
                Description = "24K gold necklace.",
                Image = "gold_necklace.jpg",
                IsNew = false,
                IsActive = true,
                CreateAt = DateTime.UtcNow
            };

            context.Products.AddRange(product1, product2);
            await context.SaveChangesAsync();

            // Lấy size random để gán
            var sizes = context.Sizes.Take(3).ToList();

            context.ProductSizes.AddRange(new List<ProductSize>
            {
                new ProductSize {
                    Id = Guid.NewGuid(),
                    ProductId = product1.Id,
                    SizeId = sizes[0].Id,
                    Price = 199.99,
                    Quantity = 10,
                    IsActive = true
                },
                new ProductSize {
                    Id = Guid.NewGuid(),
                    ProductId = product2.Id,
                    SizeId = sizes[1].Id,
                    Price = 299.99,
                    Quantity = 5,
                    IsActive = true
                }
            });
        }

        // ========== SEED REVIEW ==========
        if (!context.Reviews.Any())
        {
            var product = context.Products.FirstOrDefault();
            var account = context.Accounts.FirstOrDefault();

            if (product != null && account != null)
            {
                context.Reviews.Add(new Review
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    AccountId = account.Id,
                    Rating = 4.5,
                    Content = "Great product!",
                    IsActive = true,
                    CreateAt = DateTime.UtcNow
                });
            }
        }

        await context.SaveChangesAsync();
    }
}
