using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    [Serializable]
    public class ChangeInfo
    {
        public string SiteUrl { get; set; }
        public string ListId { get; set; }
        public int ItemId { get; set; }

    }
}
