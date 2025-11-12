using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;

namespace Kanini.Ecommerce.Application.AutoMapper;

public class AdminProfile : Profile
{
    public AdminProfile()
    {
        CreateMap<Vendor, PendingVendorDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
            .ForMember(
                dest => dest.Status,
                opt =>
                    opt.MapFrom(src =>
                        src.Status == VendorStatus.PendingApproval ? "PendingApproval"
                        : src.Status == VendorStatus.Active ? "Active"
                        : src.Status == VendorStatus.Inactive ? "Inactive"
                        : src.Status == VendorStatus.Suspended ? "Suspended"
                        : src.Status == VendorStatus.Rejected ? "Rejected"
                        : "Unknown"
                    )
            );

        CreateMap<Vendor, VendorApprovalDetailsDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
            .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.User.CreatedOn))
            .ForMember(
                dest => dest.IsEmailVerified,
                opt => opt.MapFrom(src => src.User.IsEmailVerified)
            )
            .ForMember(
                dest => dest.DocumentStatus,
                opt =>
                    opt.MapFrom(src =>
                        src.DocumentStatus == DocumentStatus.Pending ? "Pending"
                        : src.DocumentStatus == DocumentStatus.Verified ? "Verified"
                        : src.DocumentStatus == DocumentStatus.Rejected ? "Rejected"
                        : "Unknown"
                    )
            )
            .ForMember(
                dest => dest.Status,
                opt =>
                    opt.MapFrom(src =>
                        src.Status == VendorStatus.PendingApproval ? "PendingApproval"
                        : src.Status == VendorStatus.Active ? "Active"
                        : src.Status == VendorStatus.Inactive ? "Inactive"
                        : src.Status == VendorStatus.Suspended ? "Suspended"
                        : src.Status == VendorStatus.Rejected ? "Rejected"
                        : "Unknown"
                    )
            )
            .ForMember(
                dest => dest.CurrentPlan,
                opt =>
                    opt.MapFrom(src =>
                        src.CurrentPlan == Kanini.Ecommerce.Domain.Enums.SubscriptionPlan.Basic
                            ? "Basic"
                        : src.CurrentPlan == Kanini.Ecommerce.Domain.Enums.SubscriptionPlan.Standard
                            ? "Standard"
                        : src.CurrentPlan == Kanini.Ecommerce.Domain.Enums.SubscriptionPlan.Premium
                            ? "Premium"
                        : "Unknown"
                    )
            );
    }
}
