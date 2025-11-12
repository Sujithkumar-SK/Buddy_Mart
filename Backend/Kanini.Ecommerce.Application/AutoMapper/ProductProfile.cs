using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;

namespace Kanini.Ecommerce.Application.AutoMapper;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductResponseDto>()
            .ForMember(
                dest => dest.Status,
                opt =>
                    opt.MapFrom(src =>
                        src.Status == ProductStatus.Draft ? "Draft"
                        : src.Status == ProductStatus.Active ? "Active"
                        : src.Status == ProductStatus.Inactive ? "Inactive"
                        : src.Status == ProductStatus.OutOfStock ? "OutOfStock"
                        : "Unknown"
                    )
            )
            .ForMember(
                dest => dest.VendorName,
                opt => opt.MapFrom(src => src.Vendor != null ? src.Vendor.BusinessName : null)
            )
            .ForMember(
                dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null)
            )
            .ForMember(dest => dest.ImagePaths, opt => opt.MapFrom(src => 
                src.Images != null 
                    ? src.Images.OrderBy(pi => pi.DisplayOrder).Select(pi => pi.ImagePath).ToList() 
                    : new List<string>()));

        CreateMap<Product, ProductListDto>()
            .ForMember(
                dest => dest.Status,
                opt =>
                    opt.MapFrom(src =>
                        src.Status == ProductStatus.Draft ? "Draft"
                        : src.Status == ProductStatus.Active ? "Active"
                        : src.Status == ProductStatus.Inactive ? "Inactive"
                        : src.Status == ProductStatus.OutOfStock ? "OutOfStock"
                        : "Unknown"
                    )
            )
            .ForMember(
                dest => dest.VendorName,
                opt => opt.MapFrom(src => src.Vendor != null ? src.Vendor.BusinessName : null)
            )
            .ForMember(
                dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null)
            )
            .ForMember(dest => dest.PrimaryImagePath, opt => opt.MapFrom(src => 
                src.Images != null && src.Images.Any(pi => pi.IsPrimary) 
                    ? src.Images.First(pi => pi.IsPrimary).ImagePath 
                    : src.Images != null && src.Images.Any() 
                        ? src.Images.OrderBy(pi => pi.DisplayOrder).First().ImagePath 
                        : null))
            .ForMember(dest => dest.ImagePaths, opt => opt.MapFrom(src => 
                src.Images != null 
                    ? src.Images.OrderBy(pi => pi.DisplayOrder).Select(pi => pi.ImagePath).ToList() 
                    : new List<string>()));

        CreateMap<ProductUpdateRequestDto, Product>()
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.SKU, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.VendorId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.Vendor, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.CartItems, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore());

        CreateMap<ProductCreateRequestDto, Product>()
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ProductStatus.Draft))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.Vendor, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.CartItems, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore());
    }
}
