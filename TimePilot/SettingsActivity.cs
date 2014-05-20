using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Java.Lang;
using TimePilot.Models;

namespace TimePilot
{
    [Android.App.Activity(Label = "TimePilot Settings")]
    public class SettingsActivity : PreferenceActivity, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        private bool _dropboxStatus = false;
        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.Preferences);
            base.OnCreate(bundle);
            if (Build.VERSION.SdkInt < BuildVersionCodes.Base11)
            {
                //RequestWindowFeature(WindowFeatures.ActionBar);
            }

            AddPreferencesFromResource(Resource.Layout.settingPreferenceScreen);
            settingsToast();
           
            var dropboxPreference = (CheckBoxPreference)PreferenceManager.FindPreference("pref_dropboxOn");
            dropboxPreference.PreferenceClick += dropbox_PreferenceClick;
            if (DropboxHelper.GetInstance().IsLogined())
            {
                dropboxPreference.Title = "Unlink to Dropbox";
            }
            else
            {
                dropboxPreference.Title = "Link to Dropbox";
            }

            var changeDsropboxPreference = (EditTextPreference)PreferenceManager.FindPreference("pref_changeAnotherAccount");
            changeDsropboxPreference.PreferenceClick += changeDsropboxPreference_PreferenceClick;

            var aboutPreference = (EditTextPreference)PreferenceManager.FindPreference("pref_about");
            aboutPreference.PreferenceClick += about_PreferenceClick;

            EditTextPreference editPhoneNumberPref = (EditTextPreference)PreferenceManager.FindPreference("pref_phoneNumber");
            EditTextPreference editPhoneNamePref = (EditTextPreference)PreferenceManager.FindPreference("pref_phoneName");
            editPhoneNumberPref.PreferenceChange += editPhoneNumberPref_PreferenceChange;
            editPhoneNamePref.PreferenceChange += editPhoneNamePref_PreferenceChange;

            ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(this);
            var phoneName = settings.GetString("pref_phoneName", "");
            var phoneNumber = settings.GetString("pref_phoneNumber", "");


            editPhoneNamePref.Summary = phoneName;
            editPhoneNumberPref.Summary = phoneNumber;
        }
        protected override void OnResume()
        {
            base.OnResume();
            var dropboxPreference = (CheckBoxPreference)PreferenceManager.FindPreference("pref_dropboxOn");
            if (DropboxHelper.GetInstance().IsLogined())
            {
                dropboxPreference.Title = "Unlink to Dropbox";
            }
            else
            {
                dropboxPreference.Title = "Link to Dropbox";
            }
        }

        // XXXXXXXXXXXXXXXX
        void changeDsropboxPreference_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            _dropboxStatus = true;
            DropboxHelper.GetInstance().LogOut();
            var intent = new Intent(this, typeof(dropboxAuthorizationActivity));
            StartActivity(intent);
            ((EditTextPreference)e.Preference).Dialog.Cancel();
        }

        private void editPhoneNamePref_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            ((EditTextPreference)e.Preference).Summary = e.NewValue.ToString();
        }

        void editPhoneNumberPref_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            ((EditTextPreference)e.Preference).Summary = e.NewValue.ToString();
        }
        private void about_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
                var intent = new Intent(this, typeof(aboutActivity));
                StartActivity(intent);
                ((EditTextPreference)e.Preference).Dialog.Cancel();
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            Toast toast = Toast.MakeText(this, "Preferences changed", ToastLength.Short);
            toast.Show();
        }


        void dropbox_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            // trigle dropbox login for initiation£»verify whether is loged in or not
            ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(this);
            var IsDropboxOn = settings.GetBoolean("pref_dropboxOn", false);

            _dropboxStatus = IsDropboxOn;

            if (IsDropboxOn && !DropboxHelper.GetInstance().IsLogined())
            {
                //var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(DropboxHelper.GetInstance().GetAuthorizationURL()));
                //StartActivity(intent);

                // driect to DropboxAuthorization page
                var intent = new Intent(this, typeof(dropboxAuthorizationActivity));
                StartActivity(intent);
            }
            else
            {
                DropboxHelper.GetInstance().LogOut();
            }

            if (DropboxHelper.GetInstance().IsLogined())
            {
                DropboxHelper.GetInstance().UploadLocalExistingEmployeeToDropbox(this);
                DropboxHelper.GetInstance().UploadLocalExistingChecklogToDropbox(this);
                ((CheckBoxPreference)PreferenceManager.FindPreference("pref_dropboxOn")).Title = "Unlink to Dropbox";
            }
            else
            {
                ((CheckBoxPreference)PreferenceManager.FindPreference("pref_dropboxOn")).Title = "Link to Dropbox";
            }
        }

        public void settingsToast()
        {
            ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(this);
            var phoneName = settings.GetString("pref_phoneName", "");
            var phoneNumber = settings.GetString("pref_phoneNumber", "");
            var IsDropboxOn = settings.GetBoolean("pref_dropboxOn", false);

            if (phoneName == "" || phoneNumber == "")
            {
                Toast.MakeText(this, "Please Initialize Both Phone Name and Phone Number", ToastLength.Long).Show();
                if (IsDropboxOn && !DropboxHelper.GetInstance().IsLogined())
                    Toast.MakeText(this, "Please login into dropox.", ToastLength.Long).Show();
            }

        }

        bool _isFinish = false;
        public override void OnBackPressed()
        {
            base.OnBackPressed();
            ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(this);
            var phoneName = settings.GetString("pref_phoneName", "");
            var phoneNumber = settings.GetString("pref_phoneNumber", "");
            var IsDropboxOn = settings.GetBoolean("pref_dropboxOn", false);

            if ((IsDropboxOn && !DropboxHelper.GetInstance().IsLogined()) || phoneName == "" || phoneNumber == "")
            {
                var settingsCall = new Intent(this, typeof(SettingsActivity));
                StartActivity(settingsCall);
                Finish();
            }
            else
            {
                if (IsDropboxOn)
                {
                    DropboxHelper.GetInstance().UploadLocalExistingEmployeeToDropbox(this);
                    DropboxHelper.GetInstance().UploadLocalExistingChecklogToDropbox(this);

                    if (_isFinish)
                    {
                        //(new Thread(new Action(() => { EmployeeManagement.GetInstance().loadData(true,false, false); }))).Start();
                        EmployeeManagement.GetInstance().loadData(true, false, true, true);
                    }
                }

                Finish();
            }
        }

        /*
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok)
            {
                
            }
        }
         */
    }
}