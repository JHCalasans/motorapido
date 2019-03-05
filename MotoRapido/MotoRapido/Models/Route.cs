using System;
using System.Collections.Generic;
using System.Text;

namespace MotoRapido.Models
{
    public class Route
    {
        public PolylineOverView overview_polyline { get; set; }

        public class PolylineOverView
        {
            public String points { get; set; }
        }
    }
}
