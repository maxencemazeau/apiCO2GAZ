/* Copyright (C) 2023 Maxence MAZEAU
 * All rights reserved.
 *
 * Projet Qualite de l'air
 * Ecole du Web
 * Projet technologique (c)2023
 *  

    Historique des versions
           Version    Date       Auteur       Description
           1.1        24/02/23  Maxence     Première version
           1.2         23/04/23  Maxence     Deuxième version
           1.3       30/04/23   Maxence     Troisième version
   
 * */

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
            //Creer la connexion à l'API
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://10.0.2.2:8080")
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task PostNiveauAsync(string niveau, string topic)
        {
            //Appel API pour post les données
            var data = new { niveau = niveau, topic = topic};
            string json = JsonConvert.SerializeObject(data);
            Console.WriteLine("Sending JSON: " + json);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("/api/controller/envoieNiveau", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error posting data to API: {response.ReasonPhrase}");
            }
        }

        public async Task<Utilisateur> LoginAsync(string username, string password)
        {
            //Appel API pour post la connexion
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