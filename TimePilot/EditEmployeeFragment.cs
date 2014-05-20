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

namespace TimePilot
{
    public class EditEmployeeFragment : Android.Support.V4.App.DialogFragment
    {
        Button SaveButton;
        Button DeleteButton;
        Button CancelButton;
        EditText Name;
        TextView UserID;
        int position;
        string EmployeeName;
        string EmployeeID;
        public EditEmployeeFragment(int position, Bundle employeeBundle)
        {
            this.position = position;
            EmployeeName = employeeBundle.GetString("EmployeeName");
            EmployeeID = employeeBundle.GetString("EmployeeID");

        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            
           
            // Android 3.x+ still wants to show title: disable
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);

            // Create our view
            var view = inflater.Inflate(Resource.Layout.EditEmployeeFlagment, container, true);

            Name = view.FindViewById<EditText>(Resource.Id.txtName);
            this.Name.Text = EmployeeName;
            UserID = view.FindViewById<TextView>(Resource.Id.ShowUserId);
            this.UserID.Text = EmployeeID;
            
            SaveButton = view.FindViewById<Button>(Resource.Id.btnSave);
            SaveButton.Click += SaveButton_Click;

            CancelButton = view.FindViewById<Button>(Resource.Id.btnCancelDelete);
            CancelButton.Click += CancelButton_Click;

            //DeleteButton = view.FindViewById<Button>(Resource.Id.btnDeleteEmployee);
            //DeleteButton.Click += DeleteButton_Click;
            return view;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {

            var builder = new AlertDialog.Builder(this.Activity);
            //builder.SetTitle("Test");
            builder.SetMessage("Are you sure to delete?");

            builder.SetPositiveButton("Yes", (s, en) =>
            {
                Dismiss();
                EmployeeActivity activity = (EmployeeActivity)this.Activity;
                activity.DeleteEmployee(position);
            });
            builder.SetNegativeButton("No", (s, en) =>
            {

            });


            var dialog = builder.Create();
            dialog.Show();
           
        }

        public override void OnResume()
        {
            // Auto size the dialog based on it's contents
            Dialog.Window.SetLayout(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);

            // Make sure there is no background behind our view
            // Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));

            // Disable standard dialog styling/frame/theme: our custom view should create full UI
            SetStyle(Android.Support.V4.App.DialogFragment.StyleNoFrame, Android.Resource.Style.Theme);

            base.OnResume();
        }

        void SaveButton_Click(object sender, EventArgs e)
        {
            Save();
        }

        void CancelButton_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        protected void Save()
        {
            string name = this.Name.Text;
          
            if (name == "")
            {
                Toast toast = Toast.MakeText(this.Activity.BaseContext, "Please enter employee Name.", ToastLength.Short);
                toast.Show();
            }
              
            Dismiss();
            EmployeeActivity activity = (EmployeeActivity)this.Activity;
            activity.ModifyEmployeeName(position, name); 
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // Unwire event
            if (disposing)
            {
                SaveButton.Click -= SaveButton_Click;
                CancelButton.Click -= CancelButton_Click;
            }
        }

    }
}

