using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using TimePilot.Models;
using Android.Support.V4.App;

namespace TimePilot
{
    public class TransactionMenuFragment :  Android.Support.V4.App.DialogFragment
    {
       
        Button DisplaymapButton;
        Button InsertatransactionButton;
        Button DeletetransactionButton;
        Button CancelButton;
        int position;
        public CheckLogs CurrentCheckLog;
        public FragmentActivity ParentActivity;

        public TransactionMenuFragment(int position, FragmentActivity parentActivity)
        {
            this.position = position;
            this.ParentActivity = parentActivity;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Android 3.x+ still wants to show title: disable
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);

            // Create our view
            var view = inflater.Inflate(Resource.Layout.TransactionMenu, container, true);

            DisplaymapButton = view.FindViewById<Button>(Resource.Id.displaymapButton);
            DisplaymapButton.Click += DisplaymapButton_Click;

            InsertatransactionButton = view.FindViewById<Button>(Resource.Id.insertatransactionButton);
            InsertatransactionButton.Click += InsertatransactionButton_Click;

            DeletetransactionButton = view.FindViewById<Button>(Resource.Id.deletetransactionButton);
            DeletetransactionButton.Click += DeletetransactionButton_Click;

            CancelButton = view.FindViewById<Button>(Resource.Id.cancelButton);
            CancelButton.Click += CancelButton_Click;

            return view;
        }

        void CancelButton_Click(object sender, EventArgs e)
        {
            Dismiss();
            ((EmployeeLogsActivity)this.Activity).ClearHeightline();
        }

        void DeletetransactionButton_Click(object sender, EventArgs e)
        {
            var builder = new AlertDialog.Builder(this.Activity);
            //builder.SetTitle("Test");
            builder.SetMessage("Are you sure to delete?");

            builder.SetPositiveButton("Yes", (s, en) =>
            {
                Dismiss();
                EmployeeLogsActivity activity = (EmployeeLogsActivity)this.Activity;
                activity.DeleteTransaction(position);

                ((EmployeeLogsActivity)this.Activity).DeleteTransaction(position);
            });
            builder.SetNegativeButton("No", (s, en) =>
            {

            });


            var dialog = builder.Create();
            dialog.Show();
        }
        void InsertatransactionButton_Click(object sender, EventArgs e)
        {
            Dismiss();
            var AddTimeCall = new Intent(this.Activity, typeof(AddTime));
            AddTimeCall.PutExtra("EmployeeNumber", ((EmployeeLogsActivity)ParentActivity).EmployeeNumber);
            //AddTimeCall.PutExtra("LogsActivity", this);
            StartActivityForResult(AddTimeCall, 1);
            ((EmployeeLogsActivity)this.Activity).ClearHeightline();
        }
        void DisplaymapButton_Click(object sender, EventArgs e)
        {
            DisplayMetrics displaymetrics = new DisplayMetrics();
            int width = displaymetrics.WidthPixels;
            int ZoomLevel = calculateZoomLevel(width);
            string mz = ZoomLevel.ToString();
            var geoUri = Android.Net.Uri.Parse("geo:" + CurrentCheckLog.x.ToString() + "," + CurrentCheckLog.y.ToString() + "?mz="+mz+"&q="+CurrentCheckLog.x.ToString() + "," + CurrentCheckLog.y.ToString());
            var mapIntent = new Intent(Intent.ActionView, geoUri);
            StartActivity(mapIntent);

            Dismiss();
            ((EmployeeLogsActivity)this.Activity).ClearHeightline();
        }

        private int calculateZoomLevel(int screenWidth)
        {
            double equatorLength = 40075004; // in meters
            double widthInPixels = screenWidth;
            double metersPerPixel = equatorLength / 256;
            int zoomLevel = 1;
            while ((metersPerPixel * widthInPixels) > 805)
            {
                metersPerPixel /= 2;
                ++zoomLevel;
            }

            return zoomLevel;
        }
    }
}