using AutoMapper;
using Kanini.Ecommerce.Application.AutoMapper;
using Kanini.Ecommerce.Application.Services.Admin;
using Kanini.Ecommerce.Application.Services.Auth;
using Kanini.Ecommerce.Application.Services.Carts;
using Kanini.Ecommerce.Application.Services.Category;
using Kanini.Ecommerce.Application.Services.Customer;
using Kanini.Ecommerce.Application.Services.Email;
using Kanini.Ecommerce.Application.Services.Orders;
using Kanini.Ecommerce.Application.Services.Payments;
using Kanini.Ecommerce.Application.Services.Products;
using Kanini.Ecommerce.Application.Services.Vendor;
using Microsoft.Extensions.DependencyInjection;

namespace Kanini.Ecommerce.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var mapperConfig = AutoMapperConfiguration.Configure();
        services.AddSingleton(mapperConfig);
        services.AddSingleton<IMapper>(provider => new Mapper(mapperConfig, provider.GetService));

        services.AddScoped<IVendorService, VendorService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IJwtOtpService, JwtOtpService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IShippingService, ShippingService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}
