using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BankBot.Models
{
    public class Bankdetails
    {

        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "Currency")]
        public string currency { get; set; }

        [JsonProperty(PropertyName = "Amount")]
        public int amount { get; set; }

        [JsonProperty(PropertyName = "Converted")]
        public string converted { get; set; }

        [JsonProperty(PropertyName = "Result")]
        public string result { get; set; }


    }
}