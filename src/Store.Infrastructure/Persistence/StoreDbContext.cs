using Microsoft.EntityFrameworkCore;
using Store.Domain.Entities;

namespace Store.Infrastructure.Persistence;

public class StoreDbContext : DbContext
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(category => category.Id);
            entity.Property(category => category.Name)
                .HasMaxLength(100)
                .IsRequired();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Name)
                .HasMaxLength(150)
                .IsRequired();
            entity.Property(product => product.Price)
                .HasPrecision(18, 2)
                .IsRequired();
            entity.HasOne(product => product.Category)
                .WithMany(category => category.Products)
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(order => order.Id);
            entity.Property(order => order.CustomerName)
                .HasMaxLength(150)
                .IsRequired();
            entity.Property(order => order.OrderDate)
                .IsRequired();
            entity.Property(order => order.Quantity)
                .IsRequired();
            entity.Property(order => order.ProductPriceAtOrderTime)
                .HasPrecision(18, 2)
                .IsRequired();
            entity.HasOne(order => order.Product)
                .WithMany(product => product.Orders)
                .HasForeignKey(order => order.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Username)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(user => user.Password)
                .HasMaxLength(256)
                .IsRequired();
            entity.Property(user => user.Role)
                .HasMaxLength(20)
                .IsRequired();
            entity.HasIndex(user => user.Username)
                .IsUnique();
        });
    }
}
