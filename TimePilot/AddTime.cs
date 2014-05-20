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

namespace TimePilot
{
    [Activity(Label = "Add Time")]
    public class AddTime : Activity
    {
        // Create UI Variables
        //private Button cancelButton;
        private Button saveButton;
        private ToggleButton inOutButton;
        private Button dateButton;
        private Button timeButton;
        private TextView title;

        private DateTime date;
        private DateTime insertDate;
        private CheckType checkType;
        private int hour;
        private int minute;
        private string EmployeeNumber;
        Employee employee;

        const int DATE_DIALOG_ID = 0;
        const int TIME_DIALOG_ID = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.DatePicker);

            // Assign UI Variables
            //cancelButton = FindViewById<Button>(Resource.Id.cancelButton);
            saveButton = FindViewById<Button>(Resource.Id.saveButton);
            inOutButton = FindViewById<ToggleButton>(Resource.Id.inOutButton);
            inOutButton.Checked = true;
            title = FindViewById<TextView>(Resource.Id.title);
            dateButton = FindViewById<Button>(Resource.Id.dateButton);
            timeButton = FindViewById<Button>(Resource.Id.timeButton);

            // Get current date
            date = DateTime.Today;
            hour = DateTime.Now.Hour;
            minute = DateTime.Now.Minute;

            // Display current date (this method is below)
            UpdateDisplay();

            EmployeeNumber = Intent.GetStringExtra("EmployeeNumber");
            employee = EmployeeManagement.GetInstance().Employees.Where(L => L.Number == EmployeeNumber).FirstOrDefault();

            // Add a click event handler to the buttons
            dateButton.Click += delegate {
                ShowDialog(DATE_DIALOG_ID);
            };

            timeButton.Click += delegate {
                ShowDialog(TIME_DIALOG_ID);
            };

            inOutButton.Click += delegate
            {
                UpdateDisplay();
            };
            /*
            cancelButton.Click += delegate
            {
                Finish();
            };
             */

            saveButton.Click += SaveTime_Click;

        }

        private void UpdateDisplay()
        {
            dateButton.Text = date.ToString("d");
            checkType = inOutButton.Checked ? CheckType.IN : CheckType.OUT;
            // ADD 24 HOUR TIME CHECK
            string time = string.Format("{0}:{1} {2}",
                hour > 12 ? hour - 12 : hour,
                minute.ToString().PadLeft(2, '0'),
                hour > 11 ? "PM" : "AM");
            timeButton.Text = time;
            insertDate = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
        }

        private void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            this.date = e.Date;
            UpdateDisplay();
        }

        private void TimePickerCallback(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            hour = e.HourOfDay;
            minute = e.Minute;
            UpdateDisplay();
        }

        protected override Dialog OnCreateDialog(int id)
        {
            switch (id)
            {
                case DATE_DIALOG_ID:
                    return new DatePickerDialog(this, OnDateSet, date.Year, date.Month - 1, date.Day);
                case TIME_DIALOG_ID:
                    // ADD 24 HOUR TIME CHECK
                    return new TimePickerDialog(this, TimePickerCallback, hour, minute, false);
            }
            return null;
        }

        private void SaveTime_Click(object sender, EventArgs e)
        {
            employee.AddLog(checkType, insertDate);
            Intent resultIntent = new Intent();
            SetResult(Result.Ok, resultIntent);       
            Finish();
        }

    }
}