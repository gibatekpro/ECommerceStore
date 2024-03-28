namespace ECommerceStore.Models;

public class Page
{
    public Page(int size, int totalElements, int totalPages, int number)
    {
        Size = size;
        TotalElements = totalElements;
        TotalPages = totalPages;
        Number = number;
    }

    public int Size { get; init; }
    public int TotalElements { get; init; }
    public int TotalPages { get; init; }
    public int Number { get; init; }
    
}