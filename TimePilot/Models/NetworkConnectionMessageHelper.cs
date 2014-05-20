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
using Java.Lang;
using Android.Net;

using Android.Support.V4.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using System.Collections.Generic;
using TimePilot.Models;
using Java.Lang;
using Android.Net;
using Android.Net.Wifi;

namespace TimePilot.Models
{
    public class NetworkConnectionMessageHelper 
    {
        public bool GetIfConnected()
        {
            Android.Net.ConnectivityManager conMgr = (ConnectivityManager)EmployeeManagement._context.GetSystemService(Context.ConnectivityService);
            bool isConnected = false;
            NetworkInfo[] netInfo = conMgr.GetAllNetworkInfo();
            foreach (NetworkInfo ni in netInfo)
            {
                if (ni.TypeName == "WIFI")
                    if (ni.IsConnected)
                        isConnected = true;
                if (ni.TypeName == "mobile")
                    if (ni.IsConnected)
                        isConnected = true;
            }

            return isConnected;
        }


        public static NetworkConnectionMessageHelper _init = null;
   

        public static NetworkConnectionMessageHelper GetInstance()
        {
            if (_init == null)
            {
                _init = new NetworkConnectionMessageHelper();
            }

            return _init;
        }
        public NetworkConnectionMessageHelper()
        {
         
        }
      
      
    }
}

