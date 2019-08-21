using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextPortCore.Models.API
{
    public class NumberItem
    {
        public string Number { get; set; }

        // Constructors
        public NumberItem()
        {
            this.Number = string.Empty;
        }

        public NumberItem(string num)
        {
            this.Number = num;
        }
    }
}
