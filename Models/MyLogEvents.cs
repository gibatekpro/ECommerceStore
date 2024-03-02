namespace ECommerceStore.Models;

public class MyLogEvents
{
    public const int GenerateItems = 1000;
    public const int ListItems = 1001;
    public const int GetItem = 1002;
    public const int InsertItem = 1003;
    public const int UpdateItem = 1004;
    public const int DeleteItem = 1005;
    public const int RegisteringAccount = 1006;
    public const int VerifyingAccount = 1007;
    public const int SigningInAccount = 1008;
    public const int SigningOutAccount = 1009;
    public const int Checkout = 2009;

    public const int TestItem = 3000;

    public const int GetItemNotFound = 4000;
    public const int UpdateItemNotFound = 4001;
    
    public const int AuthenticationFailed = 4002;
    public const int CheckoutFailed = 4003;
}