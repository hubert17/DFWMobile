using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DFWMobile.Models
{
    public class SlabPromo
    {
        public int SlabPromoID { get; set; }
        public int SlabColorID { get; set; }
        public string SlabColor { get; set; }
        public double? SlabPromoPrice { get; set; }
        public int? SlabLength { get; set; }
        public int? SlabWidth { get; set; }
        public double? SlabOnHand { get; set; }
        public string SlabPromoNotes { get; set; }
        public bool Inactive { get; set; }
        public string ImageFilename { get; set; }

    }
}