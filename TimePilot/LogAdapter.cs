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
using Android.Preferences;
using TimePilot.Models;

namespace TimePilot
{
    /// <summary>
    /// List adapter.
    /// ListView数据适配器
    /// </summary>
    public class LogAdapter : BaseAdapter<CheckLogs>, IBaseAdapter
    {
        private Activity context = null;
        public List<CheckLogs> list = null;
        public Employee employee = null;
        private ISharedPreferences settings;
        private LayoutInflater inflater;

        /// <summary>
        /// 默认构造器
        /// </summary>
        public LogAdapter()
        {
        }

        /// <summary>
        /// 带参构造器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="list"></param>
        public LogAdapter(Activity context, Employee employee)
            : base()
        {
            inflater = LayoutInflater.From(context);
            this.employee = employee;
            this.context = context;
            this.list = employee.Logs;
            this.settings = PreferenceManager.GetDefaultSharedPreferences(this.context);
        }

        public LogAdapter(EmployeeLogsActivity employeeLogsActivity)
        {
            // TODO: Complete member initialization
            this.employeeLogsActivity = employeeLogsActivity;
        }

        public void UpdateData()
        {
            this.list = this.employee.Logs;
            this.NotifyDataSetChanged();
        }

        public override int Count
        {
            get { return this.list.Count; }
        }

        public override CheckLogs this[int position]
        {
            get { return this.list[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = this.list[position];
            var view = convertView;

            if (convertView == null || !(convertView is LinearLayout))
            {
                view = inflater.Inflate(Resource.Layout.CheckLogCell, parent, false);
                _Cells.Add(view);
            }

            ImageView imageItem = view.FindViewById<ImageView>(Resource.Id.imageView_item);
            TextView tvName = view.FindViewById<TextView>(Resource.Id.textview_top);
            ImageButton deleteButton = view.FindViewById<ImageButton>(Resource.Id.DeleteLog);

            string lastCheckDescription = "-";
            if (item.CheckType == CheckType.IN)
                imageItem.SetImageResource(Resource.Drawable.red_arrow_100x100_72);
            if (item.CheckType == CheckType.OUT)
                imageItem.SetImageResource(0);
            var milTime = settings.GetBoolean("pref_milTime", false);
            lastCheckDescription = milTime ? item.CheckTime.ToString("yyyy-MM-dd HH:mm") : item.CheckTime.ToString(@"yyyy-MM-dd hh:mm tt", new System.Globalization.CultureInfo("en-US"));

            tvName.SetText(lastCheckDescription, TextView.BufferType.Normal);

            deleteButton.Click += (sender, e) =>
            {
                employee.DeleteCheckLog(item);
                this.list.Remove(item);

                if (this.context is EmployeeLogsActivity)
                 ((EmployeeLogsActivity)this.context).Update();
            };            

            return view;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        private List<View> _Cells = new List<View>();
        private EmployeeLogsActivity employeeLogsActivity;
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
                var _deleteButton = item.FindViewById<ImageButton>(Resource.Id.DeleteLog);
                if (status)
                {
                    _deleteButton.Visibility = ViewStates.Visible;
                }
                else
                {
                    _deleteButton.Visibility = ViewStates.Invisible;
                }
            }
        }
    }
}