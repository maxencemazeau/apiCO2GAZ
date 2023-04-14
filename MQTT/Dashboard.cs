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

namespace MQTT
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
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

            // Setup button click event handler
            optionButton.Click += OnOptionButtonClick;

            updateTimer = new System.Timers.Timer();
            updateTimer.Interval = 1000; // Update UI every second
            updateTimer.AutoReset = true;
            updateTimer.Elapsed += OnUpdateTimerElapsed;

            updateTimer.Start();

        }

        private void OnOptionButtonClick(object sender, EventArgs e)
        {
            // Create intent to redirect to OptionsActivity
            // Launch the Dashboard activity if login is successful
            Intent intent = new Intent(this, typeof(Options));
            StartActivity(intent);
        }

        private async void OnUpdateTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

           
                // Retrieve the value of MessageTopic
                string messageTopic = AppData.MQTTService.MessageTopic;
            string titreTopic = AppData.MQTTService.TitreTopic;

            string messageContent = null;

            RunOnUiThread(() => {
                messageTextView.Text = messageTopic;
                titreTextView.Text = titreTopic;

                double niveau;

                if (messageTopic != null)
                {
                    double.TryParse(messageTopic, out niveau);
                    int results = Convert.ToInt32(niveau);

                    if (results > 700)
                    {
                        redView.Visibility = ViewStates.Visible;
                        greenView.Visibility = ViewStates.Gone;
                        orangeView.Visibility = ViewStates.Gone;
                        messageContent = "High";
                        

                    }
                    else if (results > 600)
                    {
                        orangeView.Visibility = ViewStates.Visible;
                        redView.Visibility = ViewStates.Gone;
                        greenView.Visibility = ViewStates.Gone;
                        messageContent = "Medium";
                        
                    }
                    else
                    {
                        greenView.Visibility = ViewStates.Visible;
                        redView.Visibility = ViewStates.Gone;
                        orangeView.Visibility = ViewStates.Gone;
                        messageContent = "OK";
                       
                    }
                }

                
            });

            // Create MQTT client options
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("172.16.5.100")
                .WithCredentials("mams", "mams")
                .Build();

            // Create MQTT client instance
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            // Connect to broker
            await mqttClient.ConnectAsync(options);

            // Publish message on "test" topic with messageContent payload
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("niveauCO2")
                .WithPayload(messageContent)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            await mqttClient.PublishAsync(message);

            // Disconnect from broker
            await mqttClient.DisconnectAsync();
        }
        }


    }

