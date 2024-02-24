﻿using ECommerceStore.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceStore.Models
{
    public class Address
    {

        public long Id { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string Street { get; set; }

        public string ZipCode { get; set; }

        //public Order Order { get; set; }//TODO: check
    }
}
