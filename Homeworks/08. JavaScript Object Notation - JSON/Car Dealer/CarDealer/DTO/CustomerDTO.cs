using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO
{
    public class CustomerDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
