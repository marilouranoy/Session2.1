/*
 * API Test Automation Training - Batch 4
 * Marilou A. Ranoy
 * 
 * Homework 2.1
 * RESTful API Test using HTTP Client and MSTest
 * Base URL: https://petstore.swagger.io/#/
 * Endpoint: /pet
 * Create a test using PUT request to update an existing pet
 * Add an Assertion for HTTP Status Code
 * Add an Assertion if data is correctly updated
 * Create a cleanup method using DELETE request 
 * 
*/

using Newtonsoft.Json;

namespace HWSession2_1
{
    //Class that defines our Pet model
    public partial class PetModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("category")]
        public Category Category { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("photoUrls")]
        public string[] PhotoUrls { get; set; }

        [JsonProperty("tags")]
        public Category[] Tags { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    //Class that defines our Category model
    public partial class Category
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
