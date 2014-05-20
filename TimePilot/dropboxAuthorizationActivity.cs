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
using Android.Webkit;
using TimePilot.Models;

namespace TimePilot
{
    [Activity(Label = "Authorization")]
    public class dropboxAuthorizationActivity : Activity
    {
        protected WebView _web;
        protected DropboxAuthizationWebviewClient _webClient;

        protected int clearCacheFolder(Java.IO.File dir) {

            int deletedFiles = 0;
            if (dir!= null && dir.IsDirectory) {
                try {
                    foreach(var child in dir.ListFiles() ){

                        //first delete subdirectories recursively
                        if (child.IsDirectory)
                        {
                            deletedFiles += clearCacheFolder(child);
                        }

                        //then delete the files and subdirectories in this dir
                        //only empty directories can be deleted, so subdirs have been done first
                            if (child.Delete()) {
                                deletedFiles++;
                            }
                    }
                }
                catch(Exception e) {
                }
            }
            return deletedFiles;
        }

        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.dropboxAuthorization);

            // For adding webview js must be enabled and add INTERNET permission to AndroidManifest xml
            _web = this.FindViewById<WebView>(Resource.Id.webViewBox);
            _web.Settings.JavaScriptEnabled = true;
            _web.Settings.JavaScriptCanOpenWindowsAutomatically = true;
            _web.Settings.SetSupportZoom(false);
            _web.SetScrollContainer(true);
            _web.SetNetworkAvailable(true);
            
            _webClient = new DropboxAuthizationWebviewClient();
            _webClient._authorization = this;

            _web.ClearCache(true);
            _web.ClearHistory();
            _web.ClearFormData();
            clearCacheFolder(this.CacheDir);
            this.DeleteDatabase("webview.db");
            this.DeleteDatabase("webviewCache.db");


            var _url = DropboxHelper.GetInstance().GetAuthorizationURL();
            if (_url.Contains("connect_login"))
            {
                _url = _url.Replace("connect_login", "logout");
            }
            else
            {
                _url = "https://www.dropbox.com/logout?cont=" + System.Web.HttpUtility.UrlEncode(_url); //_url.Replace(":", "%3A").Replace("?","%3D");
            }
            

            _web.LoadUrl(_url);

            _web.SetWebViewClient(_webClient);


            var button = this.FindViewById<Button>(Resource.Id.testbutton);
            button.Click += button_Click;
        }

        // For test
        void button_Click(object sender, EventArgs e) {
            string _url = "";
            _web.LoadUrl(DropboxHelper.GetInstance().GetAuthorizationURL());
        }

        public void CloseView()
        {
            Finish();
        }

        ProgressDialog _progressDialog = null;
        public void ShowLoading()
        {
            if (_progressDialog == null)
            {
                _progressDialog = new ProgressDialog(this);
                _progressDialog.SetMessage("Loading ...");
            }
            try
            {
                _progressDialog.Show();
            }
            catch (Exception ex) { }
        }
        public void CloseLoading()
        {
            try
            {
                _progressDialog.Dismiss();
            }
            catch (Exception ex)
            {
            }
        }
    }

    // Inherit webViewClient for add loading function and see whether authorization is completed
    public class DropboxAuthizationWebviewClient : WebViewClient
    {
        public dropboxAuthorizationActivity _authorization;
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            return base.ShouldOverrideUrlLoading(view, url);
        }
        public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);

            //see whether authorization is completed
            if (url.ToLower().Contains("google") && url.ToLower().Contains("uid") && url.ToLower().Contains("oauth_token"))
            {
                DropboxHelper.GetInstance().SaveAccessToken();
                _authorization.CloseView();
            }

            _authorization.ShowLoading();
        }
        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);

            _authorization.CloseLoading();
        }

    }
}