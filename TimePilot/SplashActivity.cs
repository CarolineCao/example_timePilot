using System;
	using Android.App;
	using Android.Content;
	using Android.OS;
using Java.Lang;
using Android.Widget;
	 
		namespace TimePilot
		{
                [Activity(MainLauncher = true, Theme = "@style/Theme.Splash", NoHistory = true)]
			    public class SplashActivity : Activity
			    {
				        protected override void OnCreate (Bundle bundle)
				        {
					            base.OnCreate (bundle);
                                SetContentView(Resource.Layout.Splash);
                                var _splash = FindViewById<ImageView>(Resource.Id.SplashImage);
                                _splash.SetImageResource(Resource.Drawable.splash);
                                Thread.Sleep(2000);
						            // Start our real activity
                                    StartActivity (typeof (EmployeeActivity));
					        }
				    }
			}