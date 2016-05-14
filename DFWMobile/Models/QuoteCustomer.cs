using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DFWMobile.Models
{
    public class QuoteCustomer
    {
        public int OnlineQuoteID { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerFirstName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
    }
}