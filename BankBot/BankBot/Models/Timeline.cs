using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BankBot.Models
{
    public class Timeline
    {

        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "UserName")]
        public string userName { get; set; }

        [JsonProperty(PropertyName = "Password")]
        public string password { get; set; }

        [JsonProperty(PropertyName = "Balance")]
        public int balance { get; set; }




    }
}