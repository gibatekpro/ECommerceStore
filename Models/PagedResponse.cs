namespace ECommerceStore.Models;

public record PagedResponse<T>
{
    public Embedded<T> _Embedded { get; set; }
    
    public Page Page { get; set; }
    
    public PagedResponse(Embedded<T> _embedded, Page _page)
    {
        _Embedded = _embedded;
        Page = _page;
    }
}