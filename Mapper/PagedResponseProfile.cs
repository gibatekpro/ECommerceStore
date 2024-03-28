using AutoMapper;
using ECommerceStore.Dto;
using ECommerceStore.Models;

namespace ECommerceStore.Mapper;

public class PagedResponseProfile: Profile
{
    public PagedResponseProfile()
    {
        CreateMap(typeof(PagedResponse<>), typeof(PagedResponseDto<>));
    }
}