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

namespace MQTT
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class Dashboard : Activity
    {
   

        private Button optionButton;
        private TextView messageTextView;

        private System.Timers.Timer updateTimer;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);          

            optionButton = FindViewById<Button>(Resource.Id.optionButton);
            messageTextView = FindViewById<TextView>(Resource.Id.dynamicValueTextView);

            // Create a new instance of MQTTService
            MQTTService mqttService = new MQTTService();

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

        private void OnUpdateTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {


            // Retrieve the value of MessageTopic
            string messageTopic = AppData.MQTTService.MessageTopic;

            
            RunOnUiThread(() => {
                messageTextView.Text = messageTopic;
            });
        }


    }
}