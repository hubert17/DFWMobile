using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DFWMobile.Models.ViewModels
{
    public class OnlineQuoteVM
    {
        public List<SlabColor> SlabColors { get; set; }
        public List<Edge> Edges { get; set; }
        public List<MeasureAsset> Measures { get; set; }
        public List<Sink> Sinks { get; set; }
        public List<ServiceJ> Services { get; set; }

    }
}