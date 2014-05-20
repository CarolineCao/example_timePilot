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
using Android.Locations;
using Android.Net;

namespace TimePilot.Models
{
    public class GPSHelper : Java.Lang.Object, ILocationListener
    {
        static Location _curLocation;
        static GPSHelper _helper;

        public Location GetLocation()
        {
            return _curLocation;
        }

        private GPSHelper() { }

        public static GPSHelper GetInstance()
        {
            if (_helper == null)
            {
                _helper = new GPSHelper();
                _myLocationManager = (LocationManager)EmployeeManagement._context.GetSystemService(Android.Content.Context.LocationService);
                GPSHelper.startGpsListener();
                _curLocation = new Location("default");
            }

            return _helper;
        }

        static LocationManager _myLocationManager;
        private static void startGpsListener()
        {
            if (_myLocationManager != null && _helper != null)
            {
                Criteria crit = new Criteria();
                crit.Accuracy = Accuracy.Fine;
                string _provider =  _myLocationManager.GetBestProvider(crit, false);
               // _myLocationManager.RequestLocationUpdates(LocationManager.GpsProvider, 5000, 10, _helper);
                _myLocationManager.RequestLocationUpdates(_provider, 0, 1, _helper);
                _myLocationManager.RequestLocationUpdates(LocationManager.GpsProvider, 0, 1, _helper);
                _myLocationManager.RequestLocationUpdates(LocationManager.NetworkProvider, 0, 1, _helper);
            }
        }
        private static void stopGpsListner()
        {
            if (_myLocationManager != null)
                _myLocationManager.RemoveUpdates(GPSHelper.GetInstance());
        }

        public void OnLocationChanged(Location location)
        {
            _curLocation = location;

            // check if locations has accuracy data
            if (_curLocation.HasAccuracy)
            {
                // Accuracy is in rage of 20 meters, stop listening we have a fix
                if (_curLocation.Accuracy < 20)
                {
                }
            }
        }
        public void OnProviderDisabled(string provider) { }
        public void OnProviderEnabled(string provider) { }
        public void OnStatusChanged(string provider, Availability status, Bundle extras) { }

    }
}

