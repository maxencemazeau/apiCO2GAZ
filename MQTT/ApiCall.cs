using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

        public async Task PostNiveauAsync(string niveau)
        {
            var data = new { niveau = niveau };
            string json = JsonConvert.SerializeObject(data);
            Console.WriteLine("Sending JSON: " + json); // Add this line to log the JSON object
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("/api/controller/envoieCO2", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error posting data to API: {response.ReasonPhrase}");
            }
        }

        public async Task<Utilisateur> LoginAsync(string username, string password)
        {
            var data = new { login = username, password = password };
            string json = JsonConvert.SerializeObject(data);
            Console.WriteLine("Sending JSON: " + json);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("/api/controller/connexion", content);

            if (response.IsSuccessStatusCode)
            {
                string responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Utilisateur>(responseJson);
            }
            else
            {
                throw new Exception($"Error logging in: {response.ReasonPhrase}");
            }
        }


    }
}