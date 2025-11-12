using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;

namespace Kanini.Ecommerce.Application.AutoMapper;

public class AnalyticsProfile : Profile
{
    public AnalyticsProfile()
    {
        CreateMap<
            (string ActivityType, string Description, DateTime CreatedOn, string UserName),
            RecentActivityDto
        >()
            .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => src.ActivityType))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName));

        CreateMap<
            (int ProductId, string ProductName, int TotalSold, decimal Revenue),
            TopSellingProductDto
        >()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.TotalSold, opt => opt.MapFrom(src => src.TotalSold))
            .ForMember(dest => dest.Revenue, opt => opt.MapFrom(src => src.Revenue));
    }
}
