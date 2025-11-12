using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Domain.Entities;

namespace Kanini.Ecommerce.Application.AutoMapper
{
    public class AutoMapperConfiguration
    {
        public static MapperConfiguration Configure()
        {
            var config = new MapperConfiguration(config =>
            {
                ConfigureMappings(config);
            });

            return config;
        }

        private static void ConfigureMappings(IMapperConfigurationExpression config)
        {
            config.AddProfile<VendorProfile>();
            config.AddProfile<CustomerProfile>();
            config.AddProfile<AuthProfile>();
            config.AddProfile<AdminProfile>();
            config.AddProfile<ProductProfile>();
            config.AddProfile<CartProfile>();
            config.AddProfile<AnalyticsProfile>();
            config.AddProfile<CategoryProfile>();
            config.AddProfile<OrderProfile>();
            config.AddProfile<PaymentProfile>();
            config.AddProfile<ShippingProfile>();
        }
    }
}
