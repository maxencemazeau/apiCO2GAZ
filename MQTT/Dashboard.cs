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
            string titreTopic = AppData.MQTTService.TitreTopic;
            

            
            RunOnUiThread(() => {
                messageTextView.Text = messageTopic;
                titreTextView.Text = titreTopic;

                double niveau;

                if (messageTopic != null)
                {

                    double.TryParse(messageTopic, out niveau);
                    Console.WriteLine(messageTopic);
                    int results = Convert.ToInt32(niveau);

                    if (results > 700)
                    {

                        redView.Visibility = ViewStates.Visible;
                        greenView.Visibility = ViewStates.Gone;
                        orangeView.Visibility = ViewStates.Gone;

                    }
                    else if (results > 600)
                    {

                        greenView.Visibility = ViewStates.Gone;
                        redView.Visibility = ViewStates.Gone;
                        greenView.Visibility = ViewStates.Gone;

                    }
                    else
                    {
                        greenView.Visibility = ViewStates.Visible;
                        redView.Visibility = ViewStates.Gone;
                        orangeView.Visibility = ViewStates.Gone;

                    }
                }

            });

       
        }


    }
}