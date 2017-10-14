using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateGroupSiteMicrosoftGraph.Models
{
    [Serializable]
    public class UnifiedGroup
    {
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }
        public object deletedDateTime { get; set; }
        public object classification { get; set; }
        public DateTime createdDateTime { get; set; }
        public string description { get; set; }
        public string displayName { get; set; }
        public List<object> groupTypes { get; set; }
        public string mail { get; set; }
        public bool mailEnabled { get; set; }
        public string mailNickname { get; set; }
        public object onPremisesLastSyncDateTime { get; set; }
        public List<object> onPremisesProvisioningErrors { get; set; }
        public object onPremisesSecurityIdentifier { get; set; }
        public object onPremisesSyncEnabled { get; set; }
        public List<object> proxyAddresses { get; set; }
        public DateTime renewedDateTime { get; set; }
        public bool securityEnabled { get; set; }
        public string visibility { get; set; }
    }
}