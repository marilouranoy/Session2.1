using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWSession2_1
{
    [TestClass]
    public class HttpClientTests
    {
        private static HttpClient httpClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string PetEndpoint = "pet";

        private static string GetURL(string endpoint) => $"{BaseURL}{endpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetModel> cleanUpList = new List<PetModel>();

        [TestInitialize]
        public void TestInitialize()
        {
            httpClient = new HttpClient();
        }

        [TestCleanup]
        public async Task TestCleanUp()
        {
            foreach (var cleanUp in cleanUpList)
            {
                var httpResponse = await httpClient.DeleteAsync(GetURL($"{PetEndpoint}/{cleanUp.Id}"));
            }
        }

        [TestMethod]
        public async Task PutMethod()
        {
            const string PHOTOURL = "https://www.rd.com/wp-content/uploads/2021/04/GettyImages-988013222-scaled-e1618857975729.jpg?w=1670";
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
                /*Tags = new Category[]
                {
                    Id = 0,
                    Name = "cat"
                },*/
            };

            var request = JsonConvert.SerializeObject(petData);
            var postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            await httpClient.PostAsync(GetURL(PetEndpoint), postRequest);

            var getResponse = await httpClient.GetAsync(GetURI($"{PetEndpoint}/{petData.Id}"));

            var listpetData = JsonConvert.DeserializeObject<PetModel>(getResponse.Content.ReadAsStringAsync().Result);

            var createdPetDataId = listpetData.Id;

            petData = new PetModel()
            {
                Id = listpetData.Id,
                Name = "TestKittyCat_Updated",
                Category = listpetData.Category,
                PhotoUrls = listpetData.PhotoUrls,
                Status = listpetData.Status,
                Tags = listpetData.Tags
            };

            request = JsonConvert.SerializeObject(petData);
            postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            var httpResponse = await httpClient.PutAsync(GetURL($"{PetEndpoint}/"), postRequest);

            var statusCode = httpResponse.StatusCode;


            getResponse = await httpClient.GetAsync(GetURI($"{PetEndpoint}/{petData.Id}"));
            listpetData = JsonConvert.DeserializeObject<PetModel>(getResponse.Content.ReadAsStringAsync().Result);
            var createdPetData = listpetData.Name;

            cleanUpList.Add(listpetData);


            Assert.AreEqual(HttpStatusCode.OK, statusCode, "Status code is not equal to 200.");
            Assert.AreEqual(petData.Name, createdPetData, "Names are not matching.");
        }
    }
}