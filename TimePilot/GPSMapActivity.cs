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
using Android.GoogleMaps;
using Android.Locations;
using System.Threading;
using Android.Util;

namespace TimePilot
{
    [Activity(Label = "GPSMap")]
    public class GPSMapActivity : Android.GoogleMaps.MapActivity
    {

        public GeoPoint thePoint;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var x = Intent.GetDoubleExtra("x", 0);
            var y = Intent.GetDoubleExtra("y", 0);
            if (thePoint == null) { thePoint = new GeoPoint((int)x, (int)y); }
          
            SetContentView(Resource.Layout.Map);

            var map = FindViewById<MapView>(Resource.Id.theMap);

            map.Clickable = true;
            map.Traffic = false;
            map.Satellite = true;

            map.SetBuiltInZoomControls(true);
            
            map.Controller.SetZoom(1);

            map.Controller.SetCenter(thePoint);

            this.FindViewById<Button>(Resource.Id.back).Click += GPSMapActivity_Click;
        }

      

        void GPSMapActivity_Click(object sender, EventArgs e)
        {
            Finish();
        }
        protected override bool IsRouteDisplayed
        {
            get
            {
                return false;
            }
        }



        public void OnProviderDisabled(string provider)
        {

        }

        public void OnProviderEnabled(string provider)
        {

        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {

        }
    }

}