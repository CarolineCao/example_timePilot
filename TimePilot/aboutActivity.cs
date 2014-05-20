using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TimePilot
{
    [Activity(Label = "About")]
    public class aboutActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.About);
             this.FindViewById<Button>(Resource.Id.btnTimePilotWebSite).Click += btnTimePilotWebSite_Click;
             this.FindViewById<Button>(Resource.Id.btnTimePilotAppSupport).Click += btnTimePilotAppSupport_Click;
        }

        private void btnTimePilotAppSupport_Click(object sender, EventArgs e)
        {
            Intent i = new Intent(Intent.ActionView);
            i.SetData(Android.Net.Uri.Parse("http://www.timepilot.com/smartphone/android.htm"));
            StartActivity(i);
        }

        private void btnTimePilotWebSite_Click(object sender, EventArgs e)
        {
            Intent i = new Intent(Intent.ActionView);
            i.SetData(Android.Net.Uri.Parse("http://www.TimePilot.com"));
            StartActivity(i);
        }
     
     
    }
}