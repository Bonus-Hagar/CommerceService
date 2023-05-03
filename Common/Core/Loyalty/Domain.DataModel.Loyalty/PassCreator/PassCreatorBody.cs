using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSRetail.Omni.Domain.DataModel.Loyalty.PassCreator
{
    public class PassCreatorBody
    {
        [JsonProperty("userProvidedId")]
        public string Id { get; set; }
        //[JsonProperty("barcodeValue")]
        [JsonIgnore]
        public string Barcode { get; set; }
        [JsonProperty("card_holder_name")]
        //[JsonProperty("5b361ad1b82342.33286937")]
        public string CustomerName { get; set; }
        [JsonProperty("card_number")]
        //[JsonProperty("5b361ad1b82592.54970699")]
        public string CustomerNumber { get; set; }
    }
}
