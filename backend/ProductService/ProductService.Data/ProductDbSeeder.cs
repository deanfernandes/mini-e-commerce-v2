using ProductService.Data.Context;
using ProductService.Data.Models;

namespace ProductService.Data;

public static class ProductDbSeeder
{
    public static void SeedProducts(ProductDbContext context)
    {
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product
                {
                    Name = "Wireless Mouse",
                    Description = "Ergonomic wireless mouse with 2.4 GHz connectivity.",
                    Price = 29.99m
                },
                new Product
                {
                    Name = "Mechanical Keyboard",
                    Description = "RGB backlit mechanical keyboard with blue switches.",
                    Price = 79.99m
                },
                new Product
                {
                    Name = "Noise Cancelling Headphones",
                    Description = "Over-ear headphones with active noise cancellation.",
                    Price = 129.99m
                },
                new Product
                {
                    Name = "Smartphone Stand",
                    Description = "Adjustable desk stand compatible with all smartphones.",
                    Price = 15.00m
                },
                new Product
                {
                    Name = "USB-C Charger",
                    Description = "Fast charging USB-C wall charger 65W output.",
                    Price = 39.99m
                }
            );

            context.SaveChanges();
        }
    }
}