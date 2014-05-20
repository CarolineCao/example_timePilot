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
using TimePilot.Models;
using Android.Support.V4.App;

namespace TimePilot
{
    [Activity(Label = "Employee Logs")]
    public class EmployeeLogsActivity : FragmentActivity, View.IOnTouchListener,GestureDetector.IOnGestureListener
    //ListActivity
    {
        LogAdapter LogAdapter;
        ListView listView;
        //Intent i = Intent.GetIntent("EmployeeNumber");
        public string name = "";
        public string EmployeeNumber;
        Employee selectedEmployee;
        //Button editButton;
        Button addTimeButton;
        private GestureDetector gestureDetector;
        // See if currently editing, determines visibility of delete button
        public bool _isEditing = false;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
           
            // Get employee
            EmployeeNumber= Intent.GetStringExtra("EmployeeNumber");
            selectedEmployee = EmployeeManagement.GetInstance().Employees.Where(L => L.Number == EmployeeNumber).FirstOrDefault();
            if (selectedEmployee == null) return;

            // Set up Listview
            SetContentView(Resource.Layout.CheckLogMaster);
            LogAdapter = new LogAdapter(this, selectedEmployee);
            listView = FindViewById<ListView>(Resource.Id.checklogs);
            listView.Adapter = LogAdapter;

            var LogName = this.FindViewById<TextView>(Resource.Id.NameForLog);
            LogName.Text = selectedEmployee.Name.ToString();
            
            var TotalHours = this.FindViewById<TextView>(Resource.Id.TotalHour);
            TotalHours.Text = "Total Hours: " + selectedEmployee.TotalHours;

            //editButton = this.FindViewById<Button>(Resource.Id.EditButton);
            addTimeButton = this.FindViewById<Button>(Resource.Id.AddTimeButton);

            //editButton.Click += EditButton_Click;
            addTimeButton.Click += AddTimeButton_Click;
            listView.SetOnTouchListener(this);

            // Gestures
            gestureDetector = new GestureDetector(this);
            listView.SetOnTouchListener(this);

            var _message = this.FindViewById<TextView>(Resource.Id.message);
            _message.Text = (selectedEmployee.Logs.Count == 0)?"There are no Transactions":"";
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            //editButton.Click -= EditButton_Click;
            addTimeButton.Click -= AddTimeButton_Click;
            this.Finish();
        }

        void AddTimeButton_Click(object sender, EventArgs e)
        {
            var AddTimeCall = new Intent(this, typeof(AddTime));
            AddTimeCall.PutExtra("EmployeeNumber", EmployeeNumber);
            //AddTimeCall.PutExtra("LogsActivity", this);
            StartActivityForResult(AddTimeCall, 1);
        }

        /*
        void EditButton_Click(object sender, EventArgs e)
        {
            _isEditing = !_isEditing;
            if (_isEditing)
                editButton.Text = "Done";
            else
                editButton.Text = "Edit";

            var _adapter = LogAdapter as IBaseAdapter;
            if (_adapter != null)
            {
                _adapter.OnChangeCellStatus(_isEditing);
            }
        }
        */
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data) 
        {
            if(resultCode==Result.Ok) 
            {
                LogAdapter.UpdateData();
                var TotalHours = this.FindViewById<TextView>(Resource.Id.TotalHour);
                TotalHours.Text = "Total Hours: " + selectedEmployee.TotalHours;
                Toast toast = Toast.MakeText(this, "Transaction Added", ToastLength.Short);
                toast.Show();

                var _message = this.FindViewById<TextView>(Resource.Id.message);
                _message.Text = (selectedEmployee.Logs.Count == 0) ? "There are no Transactions" : "";
            }
        }

        public bool OnSingleTapUp(MotionEvent e)
        {
             int pos = listView.PointToPosition((int)e.GetX(), (int)e.GetY());

            if (LogAdapter.Count <= pos || pos < 0) return true;
            FragmentTransaction trans = this.SupportFragmentManager.BeginTransaction();

            TransactionMenuFragment dialog = new TransactionMenuFragment(pos, this);
            dialog.CurrentCheckLog = LogAdapter[pos];
            dialog.Show(trans, "TransactionMenu");
            return true;
        }

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
           
            return false;
        }

        public void OnLongPress(MotionEvent e)
        {
            int pos = listView.PointToPosition((int)e.GetX(), (int)e.GetY());
            var itemView = listView.GetChildAt(pos);
            itemView.SetBackgroundColor(Android.Graphics.Color.CadetBlue);

            if (LogAdapter.Count <= pos || pos < 0) return;
            FragmentTransaction trans = this.SupportFragmentManager.BeginTransaction();

            TransactionMenuFragment dialog = new TransactionMenuFragment(pos, this);
            dialog.CurrentCheckLog = LogAdapter[pos];
            dialog.Show(trans, "TransactionMenu");
        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            return false;
        }
        public void OnShowPress(MotionEvent e)
        {

        }
        public void DeleteTransaction(int pos)
        {
            CheckLogs CurrentCheckLog = LogAdapter[pos];
            selectedEmployee.DeleteCheckLog(CurrentCheckLog);
            Update();
        }
        public void ClearHeightline()
        {
            for (var i = 0; i < listView.ChildCount; i++)
            {
                var itemView = listView.GetChildAt(i);
                itemView.SetBackgroundColor(Android.Graphics.Color.White);
            }
        }
        public void Update()
        {
            var TotalHours = this.FindViewById<TextView>(Resource.Id.TotalHour);
            TotalHours.Text = "Total Hours: " + selectedEmployee.TotalHours;
            LogAdapter.NotifyDataSetChanged();

            ClearHeightline();

            var _message = this.FindViewById<TextView>(Resource.Id.message);
            _message.Text = (selectedEmployee.Logs.Count == 0) ? "There are no Transactions" : "";
        }
         
    }
}