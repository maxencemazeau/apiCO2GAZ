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
using Android.Content;
using MQTT;
using System.Threading.Tasks;

namespace MQTT
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class Options : Activity
    {
      

        private EditText topicEditText;

        private Button retour;
        //private Button retourButton;

        private Button subscribeButton;
        private TextView messageTextView;

        private Button topicButton;
        private TextView moyenEditText;
        private TextView fortEditText;

        private Button brokerButton;
        private TextView ipEditText;

        private IMqttClient mqttClient;
        private bool isConnected = false;

        private MQTTService mqttService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.content_main);

            

            // Call the method to bind data to ListView
            //BindHistoriqueCO2Async();

            // Get UI elements
            topicButton = FindViewById<Button>(Resource.Id.buttonDuTopic);
            messageTextView = FindViewById<TextView>(Resource.Id.messageTextView);

            topicEditText = FindViewById<EditText>(Resource.Id.topic);

            ipEditText = FindViewById<TextView>(Resource.Id.ipbroker);

            moyenEditText = FindViewById<TextView>(Resource.Id.moyen);

            fortEditText = FindViewById<TextView>(Resource.Id.fort);

            retour = FindViewById<Button>(Resource.Id.retour);

            //optionButton = FindViewById<Button>(Resource.Id.optionButton);



            // Create MQTT client
            mqttClient = new MqttFactory().CreateMqttClient();

            topicButton.Click += OnTopicButtonClick;


            retour.Click += OnRetourButtonClick;

            var mqttService = new MQTTService();
            // Setup button click event handler

        }

        private void OnRetourButtonClick(object sender, EventArgs e)
        {
            // Create intent to redirect to OptionsActivity
            // Launch the Dashboard activity if login is successful
            Intent intent = new Intent(this, typeof(Dashboard));
            StartActivity(intent);
        }


        //private async void OnSubscribeButtonClick(object sender, EventArgs e)
        //{
        //    if (!isConnected)
        //    {
        //        // Setup MQTT connection options
        //        var options = new MqttClientOptionsBuilder()
        //            .WithTcpServer("172.16.5.100")
        //            .WithCredentials("mams", "mams")
        //            .Build();

        //        // Connect to MQTT broker
        //        await mqttClient.ConnectAsync(options);

        //        isConnected = true;

        //        // Subscribe to CO2 topic
        //        var subscribeResult = await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("CO2").Build());


        //        // Setup message received event handler
        //        mqttClient.UseApplicationMessageReceivedHandler(async e =>
        //        {
        //            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        //            // Display message in TextView
        //            messageTextView.Text = e.ApplicationMessage.Topic + ": " + Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        //            AppData.MQTTService.MessageTopic = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        //            AppData.MQTTService.TitreTopic = e.ApplicationMessage.Topic;

        //            // Extract niveau value from payload


        //            // Post niveau value to API
        //            var api = new ApiCall();
        //            await api.PostNiveauAsync(payload);

                    
        //        });
        //    }
        //    else
        //    {
        //        // Disconnect from MQTT broker
        //        await mqttClient.DisconnectAsync();

        //        isConnected = false;

        //        // Clear TextView
        //        messageTextView.Text = "";
        //    }
        //}

        private async void OnTopicButtonClick(object sender, EventArgs e)
        {

            string topic = topicEditText.Text;
            string brokerIp = ipEditText.Text;
            string mid = moyenEditText.Text;
            string high = fortEditText.Text;

            if (!isConnected)
            {
                // Setup MQTT connection options
                var options = new MqttClientOptionsBuilder()
                .WithTcpServer(brokerIp)
                .WithCredentials("mams", "mams")
                .Build();

                // Connect to MQTT broker
                await mqttClient.ConnectAsync(options);

                isConnected = true;

                // Subscribe to GAZ topic
                var subscribeResult = await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(topic).Build());

                // Setup message received event handler
                mqttClient.UseApplicationMessageReceivedHandler(async e =>
                {
                    string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    // Display message in TextView
                    messageTextView.Text = e.ApplicationMessage.Topic + ": " + Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    AppData.MQTTService.MessageTopic = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    AppData.MQTTService.TitreTopic = e.ApplicationMessage.Topic;
                    AppData.MQTTService.Moyen = mid;
                    AppData.MQTTService.Fort = high;

                });
            }
            else
            {
                // Disconnect from MQTT broker
                await mqttClient.DisconnectAsync();

                isConnected = false;


               
            }
        }

        
    }
}