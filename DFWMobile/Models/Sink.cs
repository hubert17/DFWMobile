using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DFWMobile.Models
{
    public class Sink
    {
        public int SinkID { get; set; }
        public string CatalogID { get; set; }
        public string SinkName { get; set; }
        public string SinkShortName { get; set; }
        public string Price { get; set; }
        public string ImageFilename { get; set; }
        public string Type { get; set; }

    }
}