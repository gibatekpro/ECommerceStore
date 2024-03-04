# ECommerceStore
Author: Gibah Anthony

Organization: Gibatekpro

[Base url](https://storecomerce.azurewebsites.net)

[Rest-Api Base url](https://storecomerce.azurewebsites.net/api)

[Swagger url](https://storecomerce.azurewebsites.net/swagger/index.html)

[Github url](https://github.com/gibatekpro/ECommerceStore)

## Description

This project provides provides a Restful Service for an E-Commerce websites.
It simulates the purchase of items on a pre-populated database

A .NET Core (C#) RESTful backend service with the following features: 
* SQL/SQLite Server database with Entities
and foreign key relationships.

* Implementation of JWT token for user authentication. 

* Integration of Identity Framework for user management, roles, and access control. 

* Email service triggered upon user sign-up.

* Authentication and authorization

* Use of git for version control

## Usage
Checkout sample (Must be authenticated)
```json
{
  "userProfile":{
    "firstName":"Tony",
    "lastName":"Gibah"
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
  "orderItems":[
    {

      "quantity":1,
      "productId":30
    },
    {

      "quantity":1,
      "productId":20
    }
  ]
}
```