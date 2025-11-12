using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kanini.Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    ImagePath = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false, comment: "Tenant isolation - each vendor has unique tenant ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Product categories with hierarchical support");

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    PlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanName = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    MaxProducts = table.Column<int>(type: "INT", nullable: false),
                    DurationDays = table.Column<int>(type: "INT", nullable: false, comment: "Duration in days"),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false, comment: "Tenant isolation - each vendor has unique tenant ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.PlanId);
                },
                comment: "Master subscription plans managed by admin");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "NVARCHAR(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR(256)", maxLength: 256, nullable: false),
                    Phone = table.Column<string>(type: "NVARCHAR(10)", nullable: false),
                    Role = table.Column<int>(type: "INT", nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "BIT", nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Last successful login timestamp"),
                    FailedLoginAttempts = table.Column<int>(type: "int", nullable: false, comment: "Number of failed login attempts"),
                    LockoutEnd = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Account lockout until this time"),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false, comment: "Tenant isolation - each vendor has unique tenant ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false, comment: "Customer's first name"),
                    MiddleName = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: true, comment: "Customer's middle name"),
                    LastName = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false, comment: "Customer's last name"),
                    DateOfBirth = table.Column<DateTime>(type: "DATE", nullable: true, comment: "Customer's date of birth"),
                    Gender = table.Column<int>(type: "INT", nullable: true),
                    Address = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    PinCode = table.Column<string>(type: "NVARCHAR(10)", maxLength: 10, nullable: true),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false, comment: "Tenant isolation - each vendor has unique tenant ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_Customers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Customer profile information");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    RefreshTokenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "BIT", nullable: false, defaultValue: false),
                    RevokedAt = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    RevokedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    RevokedReason = table.Column<string>(type: "NVARCHAR(200)", maxLength: 200, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false, comment: "Tenant isolation - each vendor has unique tenant ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.RefreshTokenId);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Refresh tokens for JWT authentication");

            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    VendorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessName = table.Column<string>(type: "NVARCHAR(150)", maxLength: 150, nullable: false),
                    OwnerName = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    BusinessLicenseNumber = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false),
                    BusinessAddress = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    PinCode = table.Column<string>(type: "NVARCHAR(10)", maxLength: 10, nullable: true),
                    TaxRegistrationNumber = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: true),
                    DocumentPath = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: false, comment: "Registration document file path"),
                    DocumentStatus = table.Column<int>(type: "INT", nullable: false, comment: "Document verification status"),
                    RejectionReason = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true, comment: "Reason for document rejection"),
                    VerifiedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true, comment: "Document verification timestamp"),
                    VerifiedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true, comment: "Admin who verified the document"),
                    CurrentPlan = table.Column<int>(type: "INT", nullable: false, comment: "Current subscription plan of the vendor"),
                    Status = table.Column<int>(type: "INT", nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false, defaultValueSql: "NEWID()", comment: "Tenant isolation - each vendor has unique tenant ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.VendorId);
                    table.ForeignKey(
                        name: "FK_Vendors_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Vendor information and business details");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: true),
                    ShippingAmount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: true),
                    TaxAmount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: true),
                    Status = table.Column<int>(type: "INT", nullable: false),
                    ShippingAddress = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: false),
                    ShippingCity = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    ShippingState = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    ShippingPinCode = table.Column<string>(type: "NVARCHAR(10)", maxLength: 10, nullable: true),
                    ShippingPhone = table.Column<string>(type: "NVARCHAR(15)", maxLength: 15, nullable: true),
                    ShippedDate = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    DeliveredDate = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    Notes = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false, comment: "Tenant isolation - each vendor has unique tenant ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Customer orders and order management");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "NVARCHAR(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(1000)", maxLength: 1000, nullable: true),
                    SKU = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    DiscountPrice = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: true),
                    StockQuantity = table.Column<int>(type: "INT", nullable: false),
                    MinStockLevel = table.Column<int>(type: "INT", nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Weight = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Dimensions = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "INT", nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false, comment: "Tenant isolation - each vendor has unique tenant ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Product catalog and inventory management");

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    SubscriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "DATE", nullable: false),
                    EndDate = table.Column<DateTime>(type: "DATE", nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false, comment: "Tenant isolation - each vendor has unique tenant ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.SubscriptionId);
                    table.ForeignKey(
                        name: "FK_Subscriptions_SubscriptionPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Vendor subscription instances");

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    PaymentMethod = table.Column<int>(type: "INT", nullable: false),
                    Status = table.Column<int>(type: "INT", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    TransactionId = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    FailureReason = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false, comment: "Tenant isolation - each vendor has unique tenant ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Payment processing and transaction details");

            migrationBuilder.CreateTable(
                name: "Shipping",
                columns: table => new
                {
                    ShippingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrackingNumber = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    CourierService = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "INT", nullable: false),
                    ShippedDate = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    EstimatedDeliveryDate = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    ActualDeliveryDate = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    DeliveryNotes = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false, comment: "Tenant isolation - each vendor has unique tenant ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipping", x => x.ShippingId);
                    table.ForeignKey(
                        name: "FK_Shipping_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Order shipping and delivery tracking");

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "INT", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false, comment: "Tenant isolation - each vendor has unique tenant ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.CartId);
                    table.ForeignKey(
                        name: "FK_Cart_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cart_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Shopping cart items for customers");

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "INT", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false, comment: "Tenant isolation - each vendor has unique tenant ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Individual items within an order");

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImagePath = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: false),
                    AltText = table.Column<string>(type: "NVARCHAR(200)", maxLength: 200, nullable: true),
                    DisplayOrder = table.Column<int>(type: "INT", nullable: false),
                    IsPrimary = table.Column<bool>(type: "BIT", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false, comment: "Tenant isolation - each vendor has unique tenant ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Product images for marketing and display");

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rating = table.Column<int>(type: "INT", nullable: false),
                    Comment = table.Column<string>(type: "NVARCHAR(1000)", maxLength: 1000, nullable: true),
                    IsVerifiedPurchase = table.Column<bool>(type: "BIT", nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false, comment: "Tenant isolation - each vendor has unique tenant ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Customer product reviews with tenant isolation");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CreatedBy", "CreatedOn", "DeletedBy", "DeletedOn", "Description", "ImagePath", "IsActive", "IsDeleted", "Name", "ParentCategoryId", "TenantId", "UpdatedBy", "UpdatedOn" },
                values: new object[] { 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Electronic devices and gadgets", null, true, false, "Electronics", null, "admin", null, null });

            migrationBuilder.InsertData(
                table: "SubscriptionPlans",
                columns: new[] { "PlanId", "CreatedBy", "CreatedOn", "DeletedBy", "DeletedOn", "Description", "DurationDays", "IsActive", "IsDeleted", "MaxProducts", "PlanName", "Price", "TenantId", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Basic plan with 10 products", 30, true, false, 10, "Basic", 999m, "admin", null, null },
                    { 2, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Standard plan with 50 products", 30, true, false, 50, "Standard", 2999m, "admin", null, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedBy", "CreatedOn", "DeletedBy", "DeletedOn", "Email", "FailedLoginAttempts", "IsActive", "IsDeleted", "IsEmailVerified", "LastLogin", "LockoutEnd", "PasswordHash", "Phone", "Role", "TenantId", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "admin@ecommerce.com", 0, true, false, true, null, null, "hashedpassword123", "9876543210", 3, "admin", null, null },
                    { 2, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "customer1@gmail.com", 0, true, false, true, null, null, "hashedpassword123", "9876543211", 1, "vendor1", null, null },
                    { 3, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "vendor1@gmail.com", 0, true, false, true, null, null, "hashedpassword123", "9876543212", 2, "vendor1", null, null }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CreatedBy", "CreatedOn", "DeletedBy", "DeletedOn", "Description", "ImagePath", "IsActive", "IsDeleted", "Name", "ParentCategoryId", "TenantId", "UpdatedBy", "UpdatedOn" },
                values: new object[] { 2, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Mobile phones and accessories", null, true, false, "Smartphones", 1, "admin", null, null });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "City", "CreatedBy", "CreatedOn", "DateOfBirth", "DeletedBy", "DeletedOn", "FirstName", "Gender", "IsActive", "IsDeleted", "LastName", "MiddleName", "PinCode", "State", "TenantId", "UpdatedBy", "UpdatedOn", "UserId" },
                values: new object[] { 1, "456 Oak St, Chennai", "Chennai", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1990, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Jane", 2, true, false, "Smith", null, "600002", "Tamil Nadu", "vendor1", null, null, 2 });

            migrationBuilder.InsertData(
                table: "Vendors",
                columns: new[] { "VendorId", "BusinessAddress", "BusinessLicenseNumber", "BusinessName", "City", "CreatedBy", "CreatedOn", "CurrentPlan", "DeletedBy", "DeletedOn", "DocumentPath", "DocumentStatus", "IsActive", "IsDeleted", "OwnerName", "PinCode", "RejectionReason", "State", "Status", "TaxRegistrationNumber", "TenantId", "UpdatedBy", "UpdatedOn", "UserId", "VerifiedBy", "VerifiedOn" },
                values: new object[] { 1, "123 Main St, Chennai", "BL123456", "Tech Store", "Chennai", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, null, "/docs/license.pdf", 2, true, false, "John Doe", "600001", null, "Tamil Nadu", 1, null, "vendor1", null, null, 3, "Admin", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "IX_Cart_CustomerId",
                table: "Cart",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_CustomerId_ProductId",
                table: "Cart",
                columns: new[] { "CustomerId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cart_ProductId",
                table: "Cart",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_TenantId",
                table: "Cart",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TenantId",
                table: "Customers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId_ProductId",
                table: "OrderItems",
                columns: new[] { "OrderId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_VendorId",
                table: "Orders",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentDate",
                table: "Payments",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status",
                table: "Payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionId",
                table: "Payments",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionId1",
                table: "Payments",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_DisplayOrder",
                table: "ProductImages",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId_IsPrimary",
                table: "ProductImages",
                columns: new[] { "ProductId", "IsPrimary" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Status",
                table: "Products",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId",
                table: "Products",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId_VendorId",
                table: "Products",
                columns: new[] { "TenantId", "VendorId" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_VendorId",
                table: "Products",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ExpiresAt",
                table: "RefreshTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CustomerId",
                table: "Reviews",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CustomerId_ProductId",
                table: "Reviews",
                columns: new[] { "CustomerId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductId",
                table: "Reviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Rating",
                table: "Reviews",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TenantId",
                table: "Reviews",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipping_OrderId",
                table: "Shipping",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipping_ShippedDate",
                table: "Shipping",
                column: "ShippedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Shipping_Status",
                table: "Shipping",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Shipping_TrackingNumber",
                table: "Shipping",
                column: "TrackingNumber",
                unique: true,
                filter: "[TrackingNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlans_IsActive",
                table: "SubscriptionPlans",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlans_PlanName",
                table: "SubscriptionPlans",
                column: "PlanName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PlanId",
                table: "Subscriptions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_VendorId",
                table: "Subscriptions",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_VendorId_IsActive",
                table: "Subscriptions",
                columns: new[] { "VendorId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Phone",
                table: "Users",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_BusinessLicense",
                table: "Vendors",
                column: "BusinessLicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_BusinessLicenseNumber",
                table: "Vendors",
                column: "BusinessLicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_Status",
                table: "Vendors",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_TenantId",
                table: "Vendors",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_UserId",
                table: "Vendors",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Shipping");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Vendors");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
