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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace HWSession2_1
{
    //Test class for our test methods
    [TestClass]
    public class HttpClientTests
    {
        //variable declarations for our test methods

        private static HttpClient httpClient;
        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";
        private static readonly string PetEndpoint = "pet";
        private static string GetURL(string endpoint) => $"{BaseURL}{endpoint}";
        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));
        private readonly List<PetModel> cleanUpList = new List<PetModel>();

        //test initialize method
        [TestInitialize]
        public void TestInitialize()
        {
            httpClient = new HttpClient();
        }

        //test cleanup method
        //In this method, we delete test data that we added via Post method
        [TestCleanup]
        public async Task TestCleanUp()
        {
            foreach (var cleanUp in cleanUpList)
            {
                var httpResponse = await httpClient.DeleteAsync(GetURL($"{PetEndpoint}/{cleanUp.Id}"));
            }
        }

        //our main method for testing Put method of the API endpoint https://petstore.swagger.io/#/pet/
        [TestMethod]
        public async Task TestPutMethod()
        {
            //declare a constant variable for the PhotoUrl property of our sample pet data
            const string PHOTOURL = "https://www.rd.com/wp-content/uploads/2021/04/GettyImages-988013222-scaled-e1618857975729.jpg?w=1670";

            //define our pet data by setting properties to correct sample values
            //this is our pet data that we will use in Post method which we will be using in testing our Put method as well
            PetModel petData = new PetModel()
            {
                Id = 1,
                Category = new Category()
                {
                    Id = 0,
                    Name = "cat"
                },
                Name = "TestCat",
                PhotoUrls = PHOTOURL.Split(),
                Status = "available",
            };


            //serialize our petData object
            var request = JsonConvert.SerializeObject(petData);

            //define our post request which includes the serialized pet data object, encoding, and the content type
            var postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            //execute our Post request
            await httpClient.PostAsync(GetURL(PetEndpoint), postRequest);

            //execute Get request and deserialize the response to be used for comparison later
            var getResponse = await httpClient.GetAsync(GetURI($"{PetEndpoint}/{petData.Id}"));
            var listpetData = JsonConvert.DeserializeObject<PetModel>(getResponse.Content.ReadAsStringAsync().Result);

            //update the value of pet data to be used in our Put method
            petData = new PetModel()
            {
                Id = listpetData.Id,
                Name = "TestKittyCat_Updated", //we only updated the name of the pet
                Category = listpetData.Category,
                PhotoUrls = listpetData.PhotoUrls,
                Status = listpetData.Status,
                Tags = listpetData.Tags
            };

            //serialize the pet data to be used in our Put request
            request = JsonConvert.SerializeObject(petData);
            var putRequest = new StringContent(request, Encoding.UTF8, "application/json");

            //execute put request/method and get the response including the status code
            var httpResponse = await httpClient.PutAsync(GetURL($"{PetEndpoint}/"), putRequest);
            var statusCode = httpResponse.StatusCode;

            //now we execute Get method to check if our pet name has been updated
            getResponse = await httpClient.GetAsync(GetURI($"{PetEndpoint}/{petData.Id}"));
            //deserialize the response to convert it into object
            listpetData = JsonConvert.DeserializeObject<PetModel>(getResponse.Content.ReadAsStringAsync().Result);
            //we read the latest pet name for comparison in our Assertion
            var createdPetDataName = listpetData.Name;

            //clean up test data that we used in our testing
            cleanUpList.Add(listpetData);

            //define assertions that will determine if our test passed or failed
            //First assertion - we check if status code is equal to 200 (OK)
            Assert.AreEqual(HttpStatusCode.OK, statusCode, "Status code is not equal to 200.");
            //Second assertion - we check if the latest name matches the updated name that we set
            Assert.AreEqual(petData.Name, createdPetDataName, "Names are not matching.");
        }
    }
}