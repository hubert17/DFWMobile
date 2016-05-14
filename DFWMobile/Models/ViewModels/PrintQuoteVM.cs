using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DFWMobile.Models.ViewModels
{
    public class PrintQuoteVM
    {
        public PrintQuoteVM()
        {
            TotalSlab = 0.00;
            TotalInstallFab = 0.00;
            TotalSink = 0.00;
            TotalService = 0.00;
            TotalDeposit = 0.00;
            AmountDue = 0.00;
            FabPricePrintOveride = false;
            Slab = new List<PrintQuoteDetail>();
            InstallFab = new List<PrintQuoteDetail>();
            Sink = new List<PrintQuoteDetail>();
            ServiceJ = new List<PrintQuoteDetail>();
        }
        public int OnlineQuoteID { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public bool FabPricePrintOveride { get; set; }
        public virtual List<PrintQuoteDetail> Slab { get; set; }
        public double TotalSlab { get; set; }
        public virtual List<PrintQuoteDetail> InstallFab { get; set; }
        public double TotalInstallFab { get; set; }
        public virtual List<PrintQuoteDetail> Sink { get; set; }
        public double TotalSink { get; set; }
        public virtual List<PrintQuoteDetail> ServiceJ { get; set; }
        public double TotalService { get; set; }
        public double TotalCost { get; set; }
        public double TotalDeposit { get; set; }
        public double AmountDue { get; set; }

    }

    public class PrintQuoteDetail
    {
        public PrintQuoteDetail()
        {
            Total = 0.00;
        }
        public string Catalog { get; set; }
        public string Name { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:c}")]
        public double Price { get; set; }
        public double Quantity { get; set; }
        public double Total { get; set; }
    }


}