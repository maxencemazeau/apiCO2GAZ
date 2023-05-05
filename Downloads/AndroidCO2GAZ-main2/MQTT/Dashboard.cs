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
using Android.App;
using Android.OS;
using Android.Util;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client;
using Android.Widget;
using MQTTnet.Client.Subscribing;
using System.Text;
using System.Collections.Generic;
using Android.Content;
using Java.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace MQTT
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class Dashboard : Activity
    {


        private Button optionButton;
        private TextView messageTextView;
        private TextView titreTextView;
        private View redView;
        private View orangeView;
        private View greenView;

        private System.Timers.Timer updateTimer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            optionButton = FindViewById<Button>(Resource.Id.optionButton);
            messageTextView = FindViewById<TextView>(Resource.Id.dynamicValueTextView);
            titreTextView = FindViewById<TextView>(Resource.Id.titleTextView);

            redView = FindViewById<View>(Resource.Id.redView);
            orangeView = FindViewById<View>(Resource.Id.orangeView);
            greenView = FindViewById<View>(Resource.Id.greenView);

            
            optionButton.Click += OnOptionButtonClick;

            updateTimer = new System.Timers.Timer();
            updateTimer.Interval = 3000; //Interval de 3 sec
            updateTimer.AutoReset = true;
            updateTimer.Elapsed += OnUpdateTimerElapsed;

            updateTimer.Start();

        }

        private void OnOptionButtonClick(object sender, EventArgs e)
        {
          
            Intent intent = new Intent(this, typeof(Options));
            StartActivity(intent);
        }

        private async void OnUpdateTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Recupère les valeurs de la page option
            string messageTopic = AppData.MQTTService.MessageTopic;
            string titreTopic = AppData.MQTTService.TitreTopic;
            string moyen = AppData.MQTTService.Moyen;
            string fort = AppData.MQTTService.Fort;
            string ip = AppData.MQTTService.Ip;
            string mdp = AppData.MQTTService.Mdp;

            string messageContent = null;

            messageTextView.Text = messageTopic;
            titreTextView.Text = titreTopic;

            //Compare les valeurs pour afficher le bon carré
            await Task.Run(() =>
            {
                double niveau;

                if (messageTopic != null)
                {
                    double.TryParse(messageTopic, out niveau);
                    int results = Convert.ToInt32(niveau);

                    int moyenInt = int.Parse(moyen);
                    int fortInt = int.Parse(fort);

                    if (results > fortInt)
                    {
                        messageContent = "High";

                        RunOnUiThread(() =>
                        {
                            redView.Visibility = ViewStates.Visible;
                            greenView.Visibility = ViewStates.Gone;
                            orangeView.Visibility = ViewStates.Gone;
                        });
                    }
                    else if (results > moyenInt)
                    {
                        messageContent = "Medium";

                        RunOnUiThread(() =>
                        {
                            orangeView.Visibility = ViewStates.Visible;
                            redView.Visibility = ViewStates.Gone;
                            greenView.Visibility = ViewStates.Gone;
                        });
                    }
                    else
                    {
                        messageContent = "OK";

                        RunOnUiThread(() =>
                        {
                            greenView.Visibility = ViewStates.Visible;
                            redView.Visibility = ViewStates.Gone;
                            orangeView.Visibility = ViewStates.Gone;
                        });
                    }
                    //Appel post api
                    var api = new ApiCall();
                    api.PostNiveauAsync(messageTopic, titreTopic);
                }
            });

            //Paramètre de la connexion au broker
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("172.16.5.100")
                .WithCredentials("mams", "mams")
                .Build();

            // Create MQTT client instance
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            // Connecte au broker
            await mqttClient.ConnectAsync(options);

            //Publie le message en fonction du niveau
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("niveauCO2")
                .WithPayload(messageContent)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            await mqttClient.PublishAsync(message);

            
            await mqttClient.DisconnectAsync();
        }

    }
}
