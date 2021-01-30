using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyCommerceDemo.Models
{
    public class CheckoutCartViewModel
    {
        public Dictionary<MyCommerceDemo.Models.Product, int> Items { get; set; }
        public MyCommerceDemo.Database.CLIENTI Cliente { get; set; }
        //public string Username { get; set; }
        //public string Password { get; set; }
    }
}