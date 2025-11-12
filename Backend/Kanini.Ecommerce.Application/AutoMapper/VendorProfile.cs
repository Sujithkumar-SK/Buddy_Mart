using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Domain.Entities;
using DomainVendor = Kanini.Ecommerce.Domain.Entities.Vendor;

namespace Kanini.Ecommerce.Application.AutoMapper;

public class VendorProfile : Profile
{
    public VendorProfile()
    {
        CreateMap<DomainVendor, VendorProfileDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(
                dest => dest.CurrentPlan,
                opt => opt.MapFrom(src => src.CurrentPlan.ToString())
            );

        CreateMap<SubscriptionPlan, SubscriptionPlanDto>();
        
        CreateMap<Order, VendorOrderDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => $"{src.Customer.FirstName} {src.Customer.LastName}"))
            .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.Customer.User.Phone))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer.User.Email))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.CreatedOn))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.OrderItems.Count))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));
            
        CreateMap<OrderItem, VendorOrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductSKU, opt => opt.MapFrom(src => src.Product.SKU))
            .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.Images.FirstOrDefault() != null ? src.Product.Images.FirstOrDefault()!.ImagePath : null));
        
        CreateMap<VendorProfileUpdateDto, DomainVendor>()
            .ForMember(dest => dest.TaxRegistrationNumber, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.TaxRegistrationNumber) ? null : src.TaxRegistrationNumber))
            .ForMember(dest => dest.VendorId, opt => opt.Ignore())
            .ForMember(dest => dest.DocumentPath, opt => opt.Ignore())
            .ForMember(dest => dest.DocumentStatus, opt => opt.Ignore())
            .ForMember(dest => dest.VerifiedOn, opt => opt.Ignore())
            .ForMember(dest => dest.VerifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentPlan, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.Subscriptions, opt => opt.Ignore())

            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore());
    }
}