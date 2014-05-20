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

namespace TimePilot.Models
{
    public enum CheckType
    {
        IN = 1,
        OUT = 0,
    }

    public class CheckLogsException : Exception
    {

        public int Message
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }

    public class CheckLogs
    {
        private Employee _employee;
        public Employee employee
        {
            get
            {
                return _employee;
            }
        }

        public CheckLogs(Employee employee, CheckType CheckType, DateTime temptime, double x, double y)
        {
            if (employee == null) throw new CheckLogsException();
            if (employee.Logs == null)
            {
                employee.Logs = new List<CheckLogs>();
            }

            //Remove Seconds
            //DateTime CheckTime = temptime.AddSeconds(-temptime.Second);
            DateTime CheckTime = temptime;

            this.CheckTime = CheckTime;
            this.CheckType = CheckType;
            employee.Logs.Add(this);
                //.Add(this);

            _employee = employee;
            this.x = x;
            this.y = y;
        }
     
        public CheckLogs(Employee employee, CheckType CheckType, DateTime temptime)
        {
            if (employee == null) throw new CheckLogsException();
            if (employee.Logs==null)
            {
                employee.Logs = new List<CheckLogs>();
            }
            //Remove Seconds
            //DateTime CheckTime = temptime.AddSeconds(-temptime.Second);
            DateTime CheckTime = temptime;
            this.CheckTime = CheckTime;
            this.CheckType = CheckType;
            _employee = employee;
            var _localtion = GPSHelper.GetInstance().GetLocation();
            x = Convert.ToDouble(_localtion.Latitude.ToString());
            y = Convert.ToDouble(_localtion.Longitude.ToString());
            employee.Logs.Add(this);
        }
        
        public CheckType CheckType { get; set; }
        public DateTime CheckTime { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        //public double Hours { get; set; }      This also is not likely needed anymore. It is not referenced anywhere.

    }
}