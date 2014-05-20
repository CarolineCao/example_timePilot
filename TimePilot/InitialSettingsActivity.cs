using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Telephony;
using Android.Runtime;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Java.Lang;
using TimePilot.Models;
using Android.Net;

namespace TimePilot
{
    [Android.App.Activity(Label = "TimePilot Initial Settings")]
    public class InitialSettingsActivity : PreferenceActivity, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        ISharedPreferences settings;

        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.Preferences);
            base.OnCreate(bundle);
            AgentApplication.getInstance().addActivity(this); 
            AddPreferencesFromResource(Resource.Layout.InitialPreferenceScreen);
            settings = PreferenceManager.GetDefaultSharedPreferences(this);
            SetupToast();
            ISharedPreferencesEditor editor = settings.Edit();
            editor.PutString("pref_phoneNumber", GetPhoneNumber());
            EditTextPreference editPhoneNumberPref = (EditTextPreference)PreferenceManager.FindPreference("pref_phoneNumber");
            EditTextPreference editPhoneNamePref = (EditTextPreference)PreferenceManager.FindPreference("pref_phoneName");
            editPhoneNumberPref.Summary = GetPhoneNumber();
            editPhoneNamePref.Summary = "";
            editPhoneNumberPref.PreferenceChange += editPhoneNumberPref_PreferenceChange;
            editPhoneNamePref.PreferenceChange += editPhoneNamePref_PreferenceChange;
            // This gets the phone name, however, in android, phone names aren't clean like in iOS.
            //editor.PutString("pref_phoneName", Android.OS.Build.User);
            editor.Commit();
            if (Build.VERSION.SdkInt < BuildVersionCodes.Base11)
            {
                //RequestWindowFeature(WindowFeatures.ActionBar);
            }

            var setupDropbox = (CheckBoxPreference)PreferenceManager.FindPreference("pref_dropboxOn");
            setupDropbox.PreferenceClick += setupDropbox_PreferenceClick;


            var cancelButton = (PreferenceScreen)PreferenceManager.FindPreference("pref_cancel");
            cancelButton.PreferenceClick += cancelButton_PreferenceClick;
        }

        void cancelButton_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            AgentApplication.getInstance().exit(); 

            //ActivityManager activityMgr = (ActivityManager)this.GetSystemService(Context.ActivityService);
            
            //activityMgr.RestartPackage(GetPackageName());
        }

        private void editPhoneNamePref_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            ((EditTextPreference)e.Preference).Summary = e.NewValue.ToString();
        }

        void editPhoneNumberPref_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            ((EditTextPreference)e.Preference).Summary = e.NewValue.ToString();
        }

        void setupDropbox_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            // trigle dropbox login for initiation£»verify whether is loged in or not
            ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(this);
            var IsDropboxOn = settings.GetBoolean("pref_dropboxOn", false);
            bool isConnected=GetIfConnected();
            if (IsDropboxOn && !DropboxHelper.GetInstance().IsLogined()&&isConnected )
            {
                Toast.MakeText(this, "This will take a moment. Please be patient.", ToastLength.Long).Show();
                //var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(DropboxHelper.GetInstance().GetAuthorizationURL()));
                //StartActivity(intent);

                // driect to DropboxAuthorization page
                var intent = new Intent(this, typeof(dropboxAuthorizationActivity));
                StartActivity(intent);
            }
            else
            {
                if(!GetIfConnected())
                    Toast.MakeText(this, "No network connection, please try it later.", ToastLength.Long).Show();
            }
        }

        public void SetupToast()
        {
            ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(this);
            var phoneName = settings.GetString("pref_phoneName", "");
            var phoneNumber = settings.GetString("pref_phoneNumber", "");
            var IsDropboxOn = settings.GetBoolean("pref_dropboxOn", false);

            if (phoneName == "" || phoneNumber == "")
            {
                //Toast.MakeText(this, "Please initialize the phone name and phone number", ToastLength.Long).Show();
                if (IsDropboxOn && !DropboxHelper.GetInstance().IsLogined())
                    Toast.MakeText(this, "Please login into dropox.", ToastLength.Long).Show();
            }

        }

        
        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            Toast toast = Toast.MakeText(this, "Preferences Added", ToastLength.Short);
            toast.Show();
           
            Preference pref = FindPreference(key);
            //if (PREF instanceof EditTextPreference) {
            //EditTextPreference etp = (EditTextPreference) pref;
            //pref.setSummary(etp.getText());
            //}
            
        }

        bool _backed = false;
        public override void OnBackPressed()
        {
            base.OnBackPressed();
            ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(this);
            var phoneName = settings.GetString("pref_phoneName", "");
            var phoneNumber = settings.GetString("pref_phoneNumber", "");
            var IsDropboxOn = settings.GetBoolean("pref_dropboxOn", false);

            if ((IsDropboxOn && !DropboxHelper.GetInstance().IsLogined()) || string.IsNullOrEmpty(phoneName) || string.IsNullOrEmpty(phoneNumber)) {
                //StartActivity(settingsCall);
                if (!_backed)
                {
                    var settingsCall = new Intent(this, typeof(InitialSettingsActivity));
                    StartActivity(settingsCall);
                    Finish();
                }
            } else {
                //var settingsCall = new Intent(this, typeof(EmployeeActivity));
                //StartActivity(settingsCall);
                if (!_backed)
                {
                    //(new Thread(new Action(() => { EmployeeManagement.GetInstance().loadData(false, true); }))).Start();
                    EmployeeManagement.GetInstance().loadData(false, true, true, true);
                    _backed = true;
                    Finish();
                }
            }
        }
        public string GetPhoneNumber()
        {
            TelephonyManager phoneManager = (TelephonyManager)GetSystemService(Context.TelephonyService);
            string number = phoneManager.Line1Number;
            return number;
        }
        public bool GetIfConnected()
        {
            Android.Net.ConnectivityManager conMgr = (ConnectivityManager)GetSystemService(Context.ConnectivityService);
            bool isConnected = false;
            NetworkInfo[] netInfo = conMgr.GetAllNetworkInfo();
            foreach (NetworkInfo ni in netInfo)
            {
                if (ni.TypeName=="WIFI")
                    if (ni.IsConnected)
                        isConnected = true;
                if (ni.TypeName=="mobile")
                    if (ni.IsConnected)
                        isConnected = true;
            }
          
                return isConnected;
        }


    }
}