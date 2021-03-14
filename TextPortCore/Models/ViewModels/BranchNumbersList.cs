using System.Collections.Generic;

using TextPortCore.Models;

namespace TextPortCore.ViewModels
{
    public class BranchNumbersList
    {
        public List<DedicatedVirtualNumber> Numbers { get; set; }

        public BranchNumbersList(List<DedicatedVirtualNumber> numbersList)
        {
            Numbers = numbersList;
        }
    }
}
