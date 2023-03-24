using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MQTT
{

    public class ApiCall
    {
        private readonly HttpClient _client;

        public ApiCall()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://10.0.2.2:8080") // Update the BaseAddress to point to your API server
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<historiqueCO2>> GethistoriqueCO2Async()
        {
            HttpResponseMessage response = await _client.GetAsync("/api/controller/historiqueCO2");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<historiqueCO2>>(json);
            }
            else
            {
                throw new Exception($"Error fetching data from API: {response.ReasonPhrase}");
            }
        }
    }
}