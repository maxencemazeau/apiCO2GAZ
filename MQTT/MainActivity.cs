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

namespace MQTT
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Button subscribeButton;
        private TextView messageTextView;

        private IMqttClient mqttClient;
        private bool isConnected = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            // Get UI elements
            subscribeButton = FindViewById<Button>(Resource.Id.subscribeButton);
            messageTextView = FindViewById<TextView>(Resource.Id.messageTextView);

            // Create MQTT client
            mqttClient = new MqttFactory().CreateMqttClient();

            // Setup button click event handler
            subscribeButton.Click += OnSubscribeButtonClick;
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
                        .WithPayload("Received message: " + Encoding.UTF8.GetString(e.ApplicationMessage.Payload))
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
    }
}