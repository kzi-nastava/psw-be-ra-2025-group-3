using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Mappers
{
    public class PaymentProfile: Profile
    {
        public PaymentProfile()
        {
            CreateMap<TourPurchaseTokenDto, TourPurchaseToken>().ReverseMap();
            CreateMap<OrderItem, ShoppingCartItemDto>();
            CreateMap<ShoppingCart, ShoppingCartDto>();
        }
    }
}
