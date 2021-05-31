using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektWebAPI.Models.v2
{
    public class GeoMessageV2DTO
    {
        
        public Message Message { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
