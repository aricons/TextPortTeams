using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextPortTeams.Models
{
    public class UserHubModel
    {
        public string UserName { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }
}