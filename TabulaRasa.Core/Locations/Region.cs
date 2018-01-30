using System.Collections.Generic;

namespace TabulaRasa.Core.Locations
{
    public class Region 
    {
        public int ZoneId { get; set; }
        public List<int> RoomIds { get; set; }
    }
}