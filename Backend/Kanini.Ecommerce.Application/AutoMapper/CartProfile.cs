using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Domain.Entities;

namespace Kanini.Ecommerce.Application.AutoMapper;

public class CartProfile : Profile
{
    public CartProfile()
    {
        // Mapping for stored procedure results (Cart with Product data populated)
        CreateMap<Cart, CartItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : ""))
            .ForMember(dest => dest.ProductSKU, opt => opt.MapFrom(src => src.Product != null ? src.Product.SKU : ""))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : 0))
            .ForMember(dest => dest.DiscountPrice, opt => opt.MapFrom(src => src.Product != null ? src.Product.DiscountPrice : null))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => 
                src.Product != null 
                    ? src.Product.Price * src.Quantity 
                    : 0))
            .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => 
                src.Product != null && src.Product.Images != null && src.Product.Images.Any(i => i.IsPrimary && !i.IsDeleted)
                    ? src.Product.Images.FirstOrDefault(i => i.IsPrimary && !i.IsDeleted)!.ImagePath
                    : null))
            .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => 
                src.Product != null && src.Product.Vendor != null ? src.Product.Vendor.BusinessName : ""))
            .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Product != null ? src.Product.StockQuantity : 0))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Product != null ? src.Product.IsActive : false))
            .ForMember(dest => dest.AddedOn, opt => opt.MapFrom(src => src.CreatedOn));
    }
}
