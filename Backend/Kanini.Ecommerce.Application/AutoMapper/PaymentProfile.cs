using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Domain.Entities;

namespace Kanini.Ecommerce.Application.AutoMapper;

public class PaymentProfile : Profile
{
    public PaymentProfile()
    {
        CreateMap<Payment, PaymentResponseDto>()
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
            .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Payment, PaymentDetailsResponseDto>()
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.RazorpayPaymentId, opt => opt.MapFrom(src => src.TransactionId));
    }
}