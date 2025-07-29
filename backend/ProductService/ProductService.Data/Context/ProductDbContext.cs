using Microsoft.EntityFrameworkCore;
using ProductService.Data.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ProductService.Data.Context;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
}