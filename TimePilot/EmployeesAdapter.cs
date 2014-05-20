using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V4.App;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using TimePilot.Models;

namespace TimePilot
{

    public class EmployeesAdapter : BaseAdapter<Employee>, IBaseAdapter
    {
        private Activity context = null;
        private EmployeeManagement employeesManager = EmployeeManagement.GetInstance();

        public EmployeesAdapter()
        {
            
        }

        public EmployeesAdapter(Activity context): base()
        {
            this.context = context;


        }

        public override int Count
        {
            get
            {
                return employeesManager.Employees.Count();
            }
        }

        public override Employee this[int position]
        {
            get {
                if (employeesManager.Employees.ElementAtOrDefault(position) != null)
                    return employeesManager.Employees[position];
                else
                    return null;
            }
        }

        public void cellOnClickDel(object sender, EventArgs e)
        {
            /*
            ImageButton _item = sender as ImageButton;
            EmployeeManagement.GetInstance().DeleteEmployee(_item.Tag.ToString());
            NotifyDataSetChanged();
             */
          
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(this.context);
            var milTime = settings.GetBoolean("pref_milTime", false);

            var ___count = employeesManager.Employees.Count();

            var item = employeesManager.Employees[position];
            var cellView = convertView;

            var _employeeActivity = this.context as EmployeeActivity;

            if (convertView == null || !(convertView is LinearLayout)) {
                cellView = context.LayoutInflater.Inflate(Resource.Layout.EmployeeCell, parent, false);
                _Cells.Add(cellView);
            }

            ImageView imageItem = cellView.FindViewById<ImageView>(Resource.Id.arrow);
            TextView employeeName = cellView.FindViewById<TextView>(Resource.Id.employeeName);
            TextView employeeLastLog = cellView.FindViewById<TextView>(Resource.Id.employeeLast);
            //ImageButton deleteButton = cellView.FindViewById<ImageButton>(Resource.Id.deleteEmployee);
            TextView employeeHours = cellView.FindViewById<TextView>(Resource.Id.employeeHours);

            employeeHours.SetText(item.TotalHours, TextView.BufferType.Normal);
            employeeName.SetText(item.Name, TextView.BufferType.Normal);
            employeeName.Text = item.Name;

            //deleteButton.Tag = item.Number;

            //deleteButton.Click += cellOnClickDel;
            /*
            if (_employeeActivity._isEditing)
            {
                employeeHours.Visibility = ViewStates.Gone;
                //deleteButton.Visibility = ViewStates.Visible;
            }
            else
            {
                deleteButton.Visibility = ViewStates.Gone;
                employeeHours.Visibility = ViewStates.Visible;
            }
             */
            cellView.Tag = item.Number;

            employeeName.SetText(item.Name, TextView.BufferType.Normal);

            var lastLog = item.LastCheckLog();

            if (lastLog != null)
            {
                if (lastLog.CheckType == CheckType.IN)
                    imageItem.SetImageResource(Resource.Drawable.red_arrow_100x100_72);
                if (lastLog.CheckType == CheckType.OUT)
                    imageItem.SetImageResource(0);
                employeeLastLog.Text = milTime ? lastLog.CheckType + "--" + lastLog.CheckTime.ToString(@"yyyy-MM-dd HH:mm") : lastLog.CheckType + " -- " + lastLog.CheckTime.ToString(@"yyyy-MM-dd hh:mm tt", new System.Globalization.CultureInfo("en-US"));
            }
            else
            {
                imageItem.SetImageResource(0);
                employeeLastLog.Text ="-";
            }

            return cellView;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        private List<View> _Cells = new List<View>();
        public List<View> Cells
        {
            get
            {
                return _Cells;
            }
            set
            {
            }
        }

        public void OnChangeCellStatus(bool status)
        {
            foreach (var item in _Cells)
            {
                //var _deleteButton = item.FindViewById<ImageButton>(Resource.Id.deleteEmployee);
                var _employeeHours = item.FindViewById<TextView>(Resource.Id.employeeHours);
                if (status)
                {
                    _employeeHours.Visibility = ViewStates.Gone;
                    //_deleteButton.Visibility = ViewStates.Visible;

                }
                else
                {
                    //_deleteButton.Visibility = ViewStates.Gone;
                    _employeeHours.Visibility = ViewStates.Visible;
                }
            }
        }

        

    }
}