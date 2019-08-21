using Newtonsoft.Json;
using System;

namespace Core
{
    public class Product
    {
        [JsonProperty("productId")]
        public Guid Id { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Created { get; set; }
    }
}