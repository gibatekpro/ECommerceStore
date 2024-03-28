using ECommerceStore.Models;

namespace ECommerceStore.Dto;

public record PagedResponseDto<T>
{
    public Embedded<T> _embedded { get; set; }
    
    public Page page { get; set; }
    
}