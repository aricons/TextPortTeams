using System.Collections.Generic;

namespace TextPortCore.Models
{
    public class Carrier
    {
        public int CarrierId { get; set; }

        public string Name { get; set; }

        public List<Country> Countries { get; set; }
    }
}
