using System;
using Android.App;
using Android.OS;
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

namespace MQTT
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private ListView _historiqueCO2ListView;

        private Button subscribeButton;
        private TextView messageTextView;

        private Button subscribeGazButton;
        private TextView messageGazTextView;

        private IMqttClient mqttClient;
        private bool isConnected = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            // Get ListView from layout
            _historiqueCO2ListView = FindViewById<ListView>(Resource.Id.historiqueCO2ListView);

            // Call the method to bind data to ListView
            BindHistoriqueCO2Async();

            // Get UI elements
            subscribeButton = FindViewById<Button>(Resource.Id.subscribeButton);
            messageTextView = FindViewById<TextView>(Resource.Id.messageTextView);

            subscribeGazButton = FindViewById<Button>(Resource.Id.subscribeGazButton);
            messageGazTextView = FindViewById<TextView>(Resource.Id.messageGazTextView);

            

            // Create MQTT client
            mqttClient = new MqttFactory().CreateMqttClient();

            // Setup button click event handler
            subscribeButton.Click += OnSubscribeButtonClick;
            subscribeGazButton.Click += OnSubscribeGazButtonClick;
        }

        private async void BindHistoriqueCO2Async()
        {
            var api = new ApiCall();
            var historiqueCO2 = await api.GethistoriqueCO2Async();

            // Create list of string to bind data to ListView
            var historiqueCO2List = new List<string>();
            foreach (var item in historiqueCO2)
            {
                historiqueCO2List.Add($"{item.Id} - Niveau: {item.Niveau}, Date: {item.Date}, UtilisateurId: {item.UtilisateurId}");
            }

            // Bind data to ListView using ArrayAdapter
            _historiqueCO2ListView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, historiqueCO2List);
        }

        private async void OnSubscribeButtonClick(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                // Setup MQTT connection options
                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer("172.16.5.100")
                    .WithCredentials("mams", "mams")
                    .Build();

                // Connect to MQTT broker
                await mqttClient.ConnectAsync(options);

                isConnected = true;

                // Subscribe to CO2 topic
                var subscribeResult = await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("CO2").Build());


                // Setup message received event handler
                mqttClient.UseApplicationMessageReceivedHandler(async e =>
                {
                    // Display message in TextView
                    messageTextView.Text = e.ApplicationMessage.Topic + ": " + Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                    // Publish a message to a new topic
                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic("CO2_received")
                        .WithPayload("Message reçu")
                        .WithExactlyOnceQoS()
                        .WithRetainFlag()
                        .Build();

                    await mqttClient.PublishAsync(message);
                });
            }
            else
            {
                // Disconnect from MQTT broker
                await mqttClient.DisconnectAsync();

                isConnected = false;

                // Clear TextView
                messageTextView.Text = "";
            }
        }
        private async void OnSubscribeGazButtonClick(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                // Setup MQTT connection options
                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer("172.16.5.100")
                    .WithCredentials("mams", "mams")
                    .Build();

                // Connect to MQTT broker
                await mqttClient.ConnectAsync(options);

                isConnected = true;

                // Subscribe to GAZ topic
                var subscribeResult = await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("GAZ").Build());

                // Setup message received event handler
                mqttClient.UseApplicationMessageReceivedHandler(async e =>
                {
                    // Handle GAZ topic
                    if (e.ApplicationMessage.Topic == "GAZ")
                    {
                        // Display message in TextView
                        messageGazTextView.Text = e.ApplicationMessage.Topic + ": " + Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    }
                    // Handle other topics here if needed
                });
            }
            else
            {
                // Disconnect from MQTT broker
                await mqttClient.DisconnectAsync();

                isConnected = false;

                
                messageGazTextView.Text = "";
            }
        }
    }
}