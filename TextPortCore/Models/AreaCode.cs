using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TextPortCore.Models
{
    public partial class AreaCode
    {
        public int AreaCodeId { get; set; }
        public string AreaCodeNum { get; set; }
        public string GeographicArea { get; set; }
        public bool TollFree { get; set; }
    }
}
