using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Domain.Entities;

namespace Kanini.Ecommerce.Application.AutoMapper;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.CreatedOn))
            .ForMember(dest => dest.CustomerName, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentMethod, opt => opt.Ignore())
            .ForMember(dest => dest.ItemCount, opt => opt.Ignore());

        CreateMap<Order, OrderDetailsResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.CreatedOn))
            .ForMember(dest => dest.CustomerName, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerPhone, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerEmail, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentMethod, opt => opt.Ignore())
            .ForMember(dest => dest.Items, opt => opt.Ignore())
            .ForMember(dest => dest.ItemCount, opt => opt.Ignore())
            .ForMember(dest => dest.Shipping, opt => opt.Ignore());

        CreateMap<OrderItem, OrderItemResponseDto>();
    }
}