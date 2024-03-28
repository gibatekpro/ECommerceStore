namespace ECommerceStore.Models;

public class Embedded<T>
{
    public List<T> Data { get; set; }

    public Embedded(List<T> data)
    {
        Data = data;
    }
    
}