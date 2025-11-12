using Kanini.Ecommerce.Data.DatabaseContext;
using Kanini.Ecommerce.Data.Repositories.Admin;
using Kanini.Ecommerce.Data.Repositories.Auth;
using Kanini.Ecommerce.Data.Repositories.Carts;
using Kanini.Ecommerce.Data.Repositories.Customer;
using Kanini.Ecommerce.Data.Repositories.Orders;
using Kanini.Ecommerce.Data.Repositories.Payments;
using Kanini.Ecommerce.Data.Repositories.Products;
using Kanini.Ecommerce.Data.Repositories.Vendor;
using Kanini.Ecommerce.Data.Repositories.Category;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kanini.Ecommerce.Data;

public static class DataServiceRegistration
{
    public static IServiceCollection AddDataServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<EcommerceDatabaseContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DatabaseConnectionString"));
        });

        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IShippingRepository, ShippingRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;
    }
}
