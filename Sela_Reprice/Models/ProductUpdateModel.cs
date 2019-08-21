using System;
using Newtonsoft.Json;

namespace Sela_Reprice.Models
{
    public  class ProductUpdateModel{
        [JsonProperty("productId")]
        public Guid Id { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }
    }
}