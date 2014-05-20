using System;


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
using Android.App;

namespace TimePilot
{
    [Android.App.Activity(Label = "TimePilot")]
    public class EmployeeActivity : FragmentActivity, GestureDetector.IOnGestureListener, View.IOnTouchListener, OnFragmentAttachedListener
    {

        EmployeesAdapter employeesAdapter;
        ListView listView;

        private GestureDetector gestureDetector;

        // See if currently editing, determines visibility of delete button
        public bool _isEditing = false;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            if (EmployeeManagement._context == null)
                EmployeeManagement._context = this;
            else
            {
                // DEBUG
                //throw new System.Exception("2rd ");
            }

            AgentApplication.getInstance().addActivity(this); 
            //((AgentApplication)this.Application).addActivity(this);

            // Set up Listview
            SetContentView(Resource.Layout.EmployeeMaster);
            employeesAdapter = new EmployeesAdapter(this);
            listView = FindViewById<ListView>(Resource.Id.employees);
            listView.Adapter = employeesAdapter;

            // Dedicate Buttons
            //this.FindViewById<Button>(Resource.Id.editButton).Click += EditButton_Click;
            this.FindViewById<Button>(Resource.Id.addButton).Click += AddButton_Click;
            this.FindViewById<Button>(Resource.Id.settingsButton).Click += SettingsButton_Click;

            // Start thread
            if (initialSettingSet())
            {
                (new Thread(new Action(() => { EmployeeManagement.GetInstance().loadData(false, false); }))).Start();
            }

            // Gestures
            gestureDetector = new GestureDetector(this);
            listView.SetOnTouchListener(this);

            //start GPS
            GPSHelper.GetInstance();
        }



        protected override void OnPostResume()
        {
            base.OnPostResume();
        }

        public bool initialSettingSet()
        {
            ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(this);

            var phoneName = settings.GetString("pref_phoneName", "");
            var phoneNumber = settings.GetString("pref_phoneNumber", "");
            var IsDropboxOn = settings.GetBoolean("pref_dropboxOn", false);

            if ((IsDropboxOn && !DropboxHelper.GetInstance().IsLogined()) || phoneName == "" || phoneNumber == "")
            //if (true) // for test
            {
                var settingsCall = new Intent(this, typeof(InitialSettingsActivity));
                StartActivity(settingsCall);

                //Finish();

                return false;
            }
            else
            {
                return true;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            employeesAdapter.NotifyDataSetChanged();
        }

        // Touch and Gestures
        public bool OnTouch(View v, MotionEvent e)
        {
            return gestureDetector.OnTouchEvent(e);
        }

        public bool OnDown(MotionEvent e)
        {
            return false;
        }
        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            //if (mySettings.FastClockIn)
            ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(this);
            var fastClockIn = settings.GetBoolean("pref_fastClockIn", false);
            if (fastClockIn)
            {
                // Determine position of two points, check if they are in same employee cell
                int pos1, pos2;
                pos1 = listView.PointToPosition((int)e1.GetX(), (int)e1.GetY());
                pos2 = listView.PointToPosition((int)e2.GetX(), (int)e2.GetY());
                if (pos1 == pos2 && EmployeeCheck(pos1))
                {
                    var x1Length = e1.GetX() + 1;
                    var x2Length = e2.GetX()+0.2;
                    var x3Length = e1.GetX() + 0.2;
                    var x4Length = e2.GetX() + 1;

                    if (x1Length < x2Length)
                    {
                        ClockIn(pos1);
                    }
                    else if (x3Length > x4Length)
                    {
                        ClockOut(pos1);
                    }
                    return true;
                }
            }
            return false;
        }

        public void OnLongPress(MotionEvent e)
        {
            var pos = listView.PointToPosition((int)e.GetX(), (int)e.GetY());
            if (EmployeeCheck(pos))
            {
                FragmentTransaction trans = this.SupportFragmentManager.BeginTransaction();
                InOutDialog dialog = new InOutDialog(pos,this);
                dialog.CurrentEmployee = employeesAdapter[pos];
                dialog.Show(trans, "inOut");
            }

        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            return false;
        }
        public void OnShowPress(MotionEvent e) { 
            
            
        }
        public bool OnSingleTapUp(MotionEvent e)
        {
            int pos = listView.PointToPosition((int)e.GetX(), (int)e.GetY());

            if (EmployeeCheck(pos) && _isEditing == false)
            {
                    Employee employee = employeesAdapter[pos];
                    var employeeCall = new Intent(this, typeof(EmployeeLogsActivity));
                    employeeCall.PutExtra("EmployeeNumber", employee.Number);
                    StartActivity(employeeCall);
            }
            else
            {
                if (_isEditing)
                {
                    FragmentTransaction trans = this.SupportFragmentManager.BeginTransaction();
                    Employee employee = employeesAdapter[pos];
                    // add some arguments to our fragment for onCreateView
                    Bundle employeeBundle = new Bundle();
                    employeeBundle.PutString("EmployeeName", employee.Name);
                    employeeBundle.PutString("EmployeeID", employee.Number);
                    EditEmployeeFragment editEmployeeDialog = new EditEmployeeFragment(pos, employeeBundle);
                    editEmployeeDialog.Show(trans, "EditEmployee");
                }
            }
            return true;
        }

        
        bool EmployeeCheck(int pos)
        {
            if (employeesAdapter[pos] != null)
                return true;
            else
                return false;
        }

        /*
        void EditButton_Click(object sender, EventArgs e)
        {
            var _editButton = this.FindViewById<Button>(Resource.Id.editButton);
            _isEditing = !_isEditing;
            if (_isEditing)
                _editButton.Text = "Done";
            else
                _editButton.Text = "Edit";

        }
    */
        public void NewEmpolyee()
        {
            //AddButton_Click(this, null);
            try
            {
                UserProfileFragment userProfileDialog = new UserProfileFragment();
                this.SupportFragmentManager.BeginTransaction().Remove(userProfileDialog).CommitAllowingStateLoss();
                this.SupportFragmentManager.BeginTransaction().Add(userProfileDialog, "userProfile").CommitAllowingStateLoss();
                userProfileDialog.Show(this.SupportFragmentManager, "userProfile");
            }
            catch (System.Exception ex)
            {
                //EmployeeManagement._context.RunOnUiThread(new Action(() =>
                this.RunOnUiThread(new Action(() =>
                {
                    AddButton_Click(this, null);
                }));
            }
        }

        void AddButton_Click(object sender, EventArgs e)
        {
            /*
            UserProfileFragment dialog = new UserProfileFragment();
            dialog.Show(this.SupportFragmentManager, "profile");
            //new FragmentActivity().SupportFragmentManager

            */
            
            //int tag = 0;
            //this.DismissDialog(tag);
            UserProfileFragment userProfileDialog = new UserProfileFragment();
            userProfileDialog.Show(this.SupportFragmentManager.BeginTransaction(), "profile");
            /*
            try
            {
                
                this.SupportFragmentManager.BeginTransaction().Remove(userProfileDialog).CommitAllowingStateLoss();
                this.SupportFragmentManager.BeginTransaction().Add(userProfileDialog, "userProfile").CommitAllowingStateLoss();
            }
            catch (Java.Lang.Exception ex)
            {
            }
            catch (System.Exception ex)
            
            {
            }
            finally
            {
                userProfileDialog.Show(this.SupportFragmentManager.BeginTransaction(), "userProfile");
            }
            */

            //var _manager = this.SupportFragmentManager.BeginTransaction();
            //_manager.Remove(userProfileDialog).CommitAllowingStateLoss();
            //_manager.Add(userProfileDialog, "profile").CommitAllowingStateLoss();
            
            

            /*
            FragmentTransaction trans = this.SupportFragmentManager.BeginTransaction();
            Bundle employeeBundle = new Bundle();
            employeeBundle.PutString("EmployeeName", "Name");
            employeeBundle.PutString("EmployeeID", "123");
            EditEmployeeFragment editEmployeeDialog = new EditEmployeeFragment(1, employeeBundle);
            editEmployeeDialog.Show(trans, "EditEmployee");
             */
        }

        void SettingsButton_Click(object sender, EventArgs e)
        {
            var settingsCall = new Intent(this, typeof(SettingsActivity));
            StartActivity(settingsCall);
        }

        public void EditEmployeeName(int pos, string ModifiedEmployeeName)
        {
            Employee employee = employeesAdapter[pos];
            employee.Name = ModifiedEmployeeName;
        }

        public void ClockIn(int pos)
        {
            Employee employee = employeesAdapter[pos];
            employee.AddLog(CheckType.IN, DateTime.Now);
            // Insert checklog check
            employeesAdapter.NotifyDataSetChanged();
            Toast toast = Toast.MakeText(this, "Clock In Successful", ToastLength.Short);
            toast.Show();
        }

        public void DisplayLoadingMessage()
        {
            var _loadingMessageView = this.FindViewById<TextView>(Resource.Id.loadingMessage);
            _loadingMessageView.Visibility = ViewStates.Visible;
            //_loadingMessageView.Height = 
        }
        public void HideLoadingMessage()
        {
            var _loadingMessageView = this.FindViewById<LinearLayout>(Resource.Id.widget_loading);
            _loadingMessageView.Visibility = ViewStates.Invisible;
        }

        public void ClockOut(int pos)
        {
            Employee employee = employeesAdapter[pos];
            employee.AddLog(CheckType.OUT, DateTime.Now);
            // Insert checklog check
            employeesAdapter.NotifyDataSetChanged();
            Toast toast = Toast.MakeText(this, "Clock Out Successful", ToastLength.Short);
            toast.Show();
        }

        // Needed for API < 11
        public void Update()
        {
            employeesAdapter.NotifyDataSetChanged();
        }
        public void DeleteEmployee(int pos)
        {
            Employee employee = employeesAdapter[pos];
            EmployeeManagement.GetInstance().DeleteEmployee(employee.Number);
            Update();
        }
        public void ModifyEmployeeName(int pos, string name)
        {
            Employee _employee = employeesAdapter[pos];
            EmployeeManagement.GetInstance().ChangeEmployeeName(_employee.Number, name);
            DropboxHelper.GetInstance().ModifyEmployeeName(EmployeeManagement._context, _employee.Number,name);
            Update();
        }
        public void ClearEmployeeChecklog(int pos)
        {
            Employee _employee = employeesAdapter[pos];
            _employee.DeleteAllCheckLog();
            Update();
        }

        public void OnFragmentAttached()
        {
        }
    }
}