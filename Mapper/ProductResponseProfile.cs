using AutoMapper;
using ECommerceStore.Dto;
using ECommerceStore.Models;

namespace ECommerceStore.Mapper;

public class ProductResponseProfile: Profile
{

    public ProductResponseProfile()
    {
        CreateMap(typeof(Product), typeof(ProductDto));
        
        CreateMap(typeof(ProductDto), typeof(Product));
    }
    
}