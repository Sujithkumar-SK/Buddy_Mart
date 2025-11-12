using Kanini.Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DomainEnums = Kanini.Ecommerce.Domain.Enums;

namespace Kanini.Ecommerce.Data.DatabaseContext;

public class EcommerceDatabaseContext(DbContextOptions<EcommerceDatabaseContext> options)
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Cart> Cart { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Shipping> Shipping { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EcommerceDatabaseContext).Assembly);

        SeedData(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Users
        modelBuilder
            .Entity<User>()
            .HasData(
                new User
                {
                    UserId = 1,
                    Email = "admin@ecommerce.com",
                    PasswordHash = "hashedpassword123",
                    Phone = "9876543210",
                    Role = DomainEnums.UserRole.Admin,
                    IsEmailVerified = true,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                    TenantId = "admin",
                },
                new User
                {
                    UserId = 2,
                    Email = "customer1@gmail.com",
                    PasswordHash = "hashedpassword123",
                    Phone = "9876543211",
                    Role = DomainEnums.UserRole.Customer,
                    IsEmailVerified = true,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                    TenantId = "vendor1",
                },
                new User
                {
                    UserId = 3,
                    Email = "vendor1@gmail.com",
                    PasswordHash = "hashedpassword123",
                    Phone = "9876543212",
                    Role = DomainEnums.UserRole.Vendor,
                    IsEmailVerified = true,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                    TenantId = "vendor1",
                }
            );

        // Seed Subscription Plans
        modelBuilder
            .Entity<SubscriptionPlan>()
            .HasData(
                new SubscriptionPlan
                {
                    PlanId = 1,
                    PlanName = "Basic",
                    Description = "Basic plan with 10 products",
                    Price = 999m,
                    MaxProducts = 10,
                    DurationDays = 30,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                    TenantId = "admin",
                },
                new SubscriptionPlan
                {
                    PlanId = 2,
                    PlanName = "Standard",
                    Description = "Standard plan with 50 products",
                    Price = 2999m,
                    MaxProducts = 50,
                    DurationDays = 30,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                    TenantId = "admin",
                }
            );

        // Seed Categories
        modelBuilder
            .Entity<Category>()
            .HasData(
                new Category
                {
                    CategoryId = 1,
                    Name = "Electronics",
                    Description = "Electronic devices and gadgets",
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                    TenantId = "admin",
                },
                new Category
                {
                    CategoryId = 2,
                    Name = "Smartphones",
                    Description = "Mobile phones and accessories",
                    ParentCategoryId = 1,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                    TenantId = "admin",
                }
            );

        // Seed Vendors
        modelBuilder
            .Entity<Vendor>()
            .HasData(
                new Vendor
                {
                    VendorId = 1,
                    BusinessName = "Tech Store",
                    OwnerName = "John Doe",
                    BusinessLicenseNumber = "BL123456",
                    BusinessAddress = "123 Main St, Chennai",
                    City = "Chennai",
                    State = "Tamil Nadu",
                    PinCode = "600001",
                    DocumentPath = "/docs/license.pdf",
                    DocumentStatus = DomainEnums.DocumentStatus.Verified,
                    VerifiedOn = new DateTime(2024, 1, 1),
                    VerifiedBy = "Admin",
                    CurrentPlan = DomainEnums.SubscriptionPlan.Basic,
                    Status = DomainEnums.VendorStatus.Active,
                    IsActive = true,
                    UserId = 3,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                    TenantId = "vendor1",
                }
            );

        // Seed Customers
        modelBuilder
            .Entity<Customer>()
            .HasData(
                new Customer
                {
                    CustomerId = 1,
                    FirstName = "Jane",
                    LastName = "Smith",
                    DateOfBirth = new DateTime(1990, 5, 15),
                    Gender = DomainEnums.Gender.Female,
                    Address = "456 Oak St, Chennai",
                    City = "Chennai",
                    State = "Tamil Nadu",
                    PinCode = "600002",
                    IsActive = true,
                    UserId = 2,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 1, 1),
                    TenantId = "vendor1",
                }
            );
    }
}
