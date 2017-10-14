using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateGroupSiteMicrosoftGraph.Models
{

    public class NewGroup
    {
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "groupTypes")]
        public List<string> GroupTypes { get; set; }

        [JsonProperty(PropertyName = "mailEnabled")]
        public bool MailEnabled { get; set; }

        [JsonProperty(PropertyName = "mailNickname")]
        public string MailNickname { get; set; }

        [JsonProperty(PropertyName = "securityEnabled")]
        public bool SecurityEnabled { get; set; }
    }
}