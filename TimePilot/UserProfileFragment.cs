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
using Android.Provider;
using TimePilot.Models;
using Android.App;


namespace TimePilot
{
    public interface OnFragmentAttachedListener
    {
        void OnFragmentAttached();
    }

    [Android.App.Activity(Label = "My Activity")]
    public class UserProfileFragment : Android.Support.V4.App.DialogFragment
    {
        Button SaveButton;
        Button CancelButton;
        EditText Name;
        EditText Number;

        OnFragmentAttachedListener mListener = null;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Android 3.x+ still wants to show title: disable
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);

            // Create our view
            var view = inflater.Inflate(Resource.Layout.UserProfile, container, true);

            Name = view.FindViewById<EditText>(Resource.Id.txtName);
            Number = view.FindViewById<EditText>(Resource.Id.UserIdEdit);

            SaveButton = view.FindViewById<Button>(Resource.Id.btnSave);
            SaveButton.Click += SaveButton_Click;

            CancelButton = view.FindViewById<Button>(Resource.Id.btnCancelDelete);
            CancelButton.Click += CancelButton_Click;

            return view;
        }

        public override void OnResume()
        {
            // Auto size the dialog based on it's contents
            Dialog.Window.SetLayout(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);

            // Make sure there is no background behind our view
            // Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));

            // Disable standard dialog styling/frame/theme: our custom view should create full UI
            SetStyle(Android.Support.V4.App.DialogFragment.StyleNoFrame, Android.Resource.Style.Theme);

            if (mListener != null)
            {
                mListener.OnFragmentAttached();
            }

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
            string tempNumber = this.Number.Text;


            char pad = '0';
            string number = tempNumber.PadLeft(4, pad);
            EmployeeManagement employeeManager = EmployeeManagement.GetInstance();
            List<Employee> employees = employeeManager.Employees;

            if (name != "" && tempNumber != "")
            {
                if (!employeeManager.ValidUserID(number) && tempNumber != "")
                {
                    //var builder = new AlertDialog.Builder(this.Activity);
                    var builder = new AlertDialog.Builder(EmployeeManagement._context);
                    //builder.SetTitle("Test");
                    builder.SetMessage("The User ID you entered already exists; please enter a different number.");

                    builder.SetPositiveButton("OK", (s, en) =>
                    {


                    });


                    var dialog = builder.Create();
                    dialog.Show();
                    /*
                    Toast toast = Toast.MakeText(this.Activity.BaseContext, "The User ID you entered already exists; please enter a different number.", ToastLength.Short);
                    toast.Show();
                     */
                }
                else
                {
                    employeeManager.AddEmployee(name, number, true);
                    EmployeeActivity activity = (EmployeeActivity)this.Activity;
                    if (activity != null)
                        activity.Update();
                    Dismiss();
                }
            }

            else
            {
                if (name == "")
                {
                    Toast toast = Toast.MakeText(this.Activity.BaseContext, "Please enter employee Name.", ToastLength.Short);
                    toast.Show();
                }
                if (tempNumber == "")
                {
                    Toast toast = Toast.MakeText(this.Activity.BaseContext, "Please enter employee ID.", ToastLength.Short);
                    toast.Show();
                }

            }

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

        public override void OnAttach(Activity p0)
        {
            base.OnAttach(p0);
            try
            {
                mListener = (OnFragmentAttachedListener)p0;
            }
            catch (Java.Lang.Exception e)
            {
                mListener = null;
            }
            catch (System.Exception e)
            {
                mListener = null;

            }
        }

        public override void Show(FragmentManager p0, string p1)
        {
            try
            {
                base.Show(p0, p1);
            }
            catch (Exception ex)
            {
            }
        }
        public override int Show(FragmentTransaction p0, string p1)
        {
            try
            {
                return base.Show(p0, p1);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }

}