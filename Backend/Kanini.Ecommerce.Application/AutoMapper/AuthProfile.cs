using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;

namespace Kanini.Ecommerce.Application.AutoMapper;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<User, LoginResponseDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
            .ForMember(dest => dest.AccessTokenExpiry, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokenExpiry, opt => opt.Ignore())
            .ForMember(dest => dest.Message, opt => opt.Ignore());

        CreateMap<RefreshToken, RefreshTokenRequestDto>()
            .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.Token));

        // Registration mappings
        CreateMap<RegisterWithOtpRequestDto, User>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => (UserRole)src.Role))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Role == 1));

        CreateMap<RegisterWithOtpRequestDto, Customer>()
            .ForMember(
                dest => dest.Gender,
                opt =>
                    opt.MapFrom(src =>
                        src.Gender.HasValue ? (Gender)src.Gender.Value : (Gender?)null
                    )
            );

        CreateMap<CustomerProfileUpdateDto, Customer>()
            .ForMember(
                dest => dest.Gender,
                opt =>
                    opt.MapFrom(src =>
                        src.Gender.HasValue ? (Gender)src.Gender.Value : (Gender?)null
                    )
            );

        CreateMap<RegisterWithOtpRequestDto, Vendor>()
            .ForMember(
                dest => dest.DocumentStatus,
                opt => opt.MapFrom(src => DocumentStatus.Pending)
            )
            .ForMember(
                dest => dest.CurrentPlan,
                opt => opt.MapFrom(src => Domain.Enums.SubscriptionPlan.Basic)
            )
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => VendorStatus.PendingApproval))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => false));
    }
}
