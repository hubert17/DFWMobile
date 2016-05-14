using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DFWMobile.Models
{
    public class QuoteCountertop
    {
        public QuoteCountertop() { }
        public QuoteCountertop(int slabcolorID, double sfqty)
        {
            SlabColorID = slabcolorID;
            SquareFeetQty = sfqty;
            FabPricePrintOveride = null;
        }
        public int OnlineQuoteStoneID { get; set; }
        public int SlabColorID { get; set; }
        public double SquareFeetQty { get; set; }
        public double? FabPricePrintOveride { get; set; }
        public int Edge { get; set; }        

    }
}