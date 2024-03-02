# ECommerceStore

A .NET Core (C#) RESTful backend service with the following features: • SQL/SQL Server database with at least 5 tables
and foreign key relationships. • Implementation of JWT token for user authentication. • Integration of Identity
Framework for user management, roles, and access control. • Email service triggered upon user sign-up.

Checkout sample:

{
"customer":{
"firstName":"Tony",
"lastName":"Gibah",
"email":"tony@test.com"
},
"shippingAddress":{
"street":"Wembley",
"city":"Brent",
"state":"London",
"country":"United Kingdom",
"zipCode":"HA9 0FR"
},
"billingAddress":{
"street":"Wembley",
"city":"Brent",
"state":"London",
"country":"United Kingdom",
"zipCode":"HA9 0FR"
},
"order":{
"totalPrice":36.98,
"totalQuantity":2
},
"orderItems":[
{
"imageUrl":"assets/images/products/coffeemugs/coffeemug.png",
"quantity":1,
"unitPrice":18.99,
"productId":26
},
{
"imageUrl":"assets/images/products/mousepads/mousepad.png",
"quantity":1,
"unitPrice":17.99,
"productId":51
}
]
}
