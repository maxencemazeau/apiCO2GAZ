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
using MQTTnet;
using MQTTnet.Client;

namespace MQTT
{
    public class MQTTService
    {
        private IMqttClient _mqttClient;

        public MQTTService()
        {
            _mqttClient = new MqttFactory().CreateMqttClient();
        }

        public string MessageTopic { get; set; }
        public string TitreTopic { get; set; }
        public string Moyen { get; set; }
        public string Fort { get; set; }
        public string Ip { get; set; }
        public string Mdp { get; set; }

        public IMqttClient MqttClient
        {
            get { return _mqttClient; }
        }
    }
}