using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TimePilot.Models;
using Android.App;

namespace TimePilot
{

    public class InOutDialog : Android.Support.V4.App.DialogFragment
    {
        Button InButton;
        Button OutButton;
        Button ClearUserRecordsButton;
        Button EditUserButton;
        Button DeleteUserButton;
        Button CancelButton;
        int position;
        public Employee CurrentEmployee;
        public FragmentActivity ParentActivity;

        public InOutDialog(int position, FragmentActivity parentActivity)
        {
            this.position = position;
            this.ParentActivity = parentActivity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Android 3.x+ still wants to show title: disable
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);

            // Create our view
            var view = inflater.Inflate(Resource.Layout.InOut, container, true);
            var NameandNumber = view.FindViewById<TextView>(Resource.Id.NameandNumber);
            NameandNumber.Text = CurrentEmployee.Name.ToString()+" "+"-"+" "+CurrentEmployee.Number.ToString();

            InButton = view.FindViewById<Button>(Resource.Id.inButton);
            InButton.Click += InButton_Click;

            OutButton = view.FindViewById<Button>(Resource.Id.outButton);
            OutButton.Click += OutButton_Click;

            CancelButton = view.FindViewById<Button>(Resource.Id.cancelButton);
            CancelButton.Click += CancelButton_Click;

            ClearUserRecordsButton = view.FindViewById<Button>(Resource.Id.clearButton);
            ClearUserRecordsButton.Click += ClearUserRecordsButton_Click;

            EditUserButton = view.FindViewById<Button>(Resource.Id.editUserButton);
            EditUserButton.Click += EditUserButton_Click;

            DeleteUserButton = view.FindViewById<Button>(Resource.Id.deleteUserButton);
            DeleteUserButton.Click += DeleteUserButton_Click;

            return view;
        }

        void DeleteUserButton_Click(object sender, EventArgs e)
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
        void EditUserButton_Click(object sender, EventArgs e)
        {

            FragmentTransaction trans = this.ParentActivity.SupportFragmentManager.BeginTransaction();
            //Employee employee = employeesAdapter[pos];
            // add some arguments to our fragment for onCreateView
            Bundle employeeBundle = new Bundle();
            employeeBundle.PutString("EmployeeName", this.CurrentEmployee.Name);
            employeeBundle.PutString("EmployeeID", this.CurrentEmployee.Number);
            EditEmployeeFragment editEmployeeDialog = new EditEmployeeFragment(position, employeeBundle);

            editEmployeeDialog.Show(trans, "EditEmployee");
            Dismiss();
        }
        void ClearUserRecordsButton_Click(object sender, EventArgs e)
        {
            var builder = new AlertDialog.Builder(this.Activity);
            //builder.SetTitle("Test");
            builder.SetMessage("Are you sure to delete all check log?");

            builder.SetPositiveButton("Yes", (s, en) =>
            {
                Dismiss();
                EmployeeActivity activity = (EmployeeActivity)this.Activity;
                activity.ClearEmployeeChecklog(position);
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

        private void InButton_Click(object sender, EventArgs e)
        {
            Dismiss();
            EmployeeActivity activity = (EmployeeActivity)this.Activity;
            activity.ClockIn(position);
        }

        private void OutButton_Click(object sender, EventArgs e)
        {
            Dismiss();
            EmployeeActivity activity = (EmployeeActivity)this.Activity;
            activity.ClockOut(position);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // Unwire event
            if (disposing)
            {
                InButton.Click -= InButton_Click;
                OutButton.Click -= OutButton_Click;
            }
        }
    }
}