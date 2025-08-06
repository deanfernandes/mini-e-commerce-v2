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
            },
            new Product
            {
                Name = "Bluetooth Speaker",
                Description = "Portable Bluetooth speaker with 360-degree sound.",
                Price = 49.99m
            },
            new Product
            {
                Name = "Gaming Headset",
                Description = "Surround sound gaming headset with built-in microphone.",
                Price = 59.99m
            },
            new Product
            {
                Name = "Laptop Cooling Pad",
                Description = "USB-powered cooling pad with adjustable fan speed.",
                Price = 24.99m
            },
            new Product
            {
                Name = "Webcam 1080p",
                Description = "Full HD webcam with built-in microphone and privacy cover.",
                Price = 45.00m
            },
            new Product
            {
                Name = "External SSD 1TB",
                Description = "High-speed external solid state drive with USB 3.1.",
                Price = 119.99m
            },
            new Product
            {
                Name = "Wireless Earbuds",
                Description = "Compact wireless earbuds with charging case.",
                Price = 59.99m
            },
            new Product
            {
                Name = "Portable Monitor",
                Description = "15.6-inch portable USB-C monitor for laptops and phones.",
                Price = 189.99m
            },
            new Product
            {
                Name = "Smart LED Light Strip",
                Description = "Color-changing LED strip with app and voice control.",
                Price = 34.99m
            },
            new Product
            {
                Name = "Ergonomic Office Chair",
                Description = "Adjustable mesh chair with lumbar support.",
                Price = 249.99m
            },
            new Product
            {
                Name = "4K HDMI Cable",
                Description = "High-speed HDMI 2.1 cable for 4K/8K displays.",
                Price = 12.99m
            }
        );

            context.SaveChanges();
        }
    }
}