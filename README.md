# ECommerceStore
A .NET Core (C#) RESTful backend service with the following features: • SQL/SQL Server database with at least 5 tables and foreign key relationships. • Implementation of JWT token for user authentication. • Integration of Identity Framework for user management, roles, and access control. • Email service triggered upon user sign-up.


Checkout sample:

{
   "customer":{
      "firstName":"afasa",
      "lastName":"afasa",
      "email":"afasa@test.com"
   },
   "shippingAddress":{
      "street":"afasa",
      "city":"afasa",
      "state":"Alberta",
      "country":"Canada",
      "zipCode":"afasa"
   },
   "billingAddress":{
      "street":"fsfsf",
      "city":"sfdsf",
      "state":"Acre",
      "country":"Brazil",
      "zipCode":"19111"
   },
   "order":{
      "totalPrice":36.98,
      "totalQuantity":2
   },
   "orderItems":[
      {
         "imageUrl":"assets/images/products/coffeemugs/coffeemug-luv2code-1000.png",
         "quantity":1,
         "unitPrice":18.99,
         "productId":26
      },
      {
         "imageUrl":"assets/images/products/mousepads/mousepad-luv2code-1000.png",
         "quantity":1,
         "unitPrice":17.99,
         "productId":51
      }
   ]
}
