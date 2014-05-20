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
    public class Employee
    {
        public Employee()
        {
          
            _Logs = new List<CheckLogs>();
            Name = "";
            Number = "";
            Active = true;
            iButton = "";
            /*
            var _ram = new Random();
        
            for (var i = 10; i > 0; i--)
            {
                int _time = _ram.Next(0, 12);
                int _time_min = _ram.Next(0, 60);
                this.AddLog(CheckType.IN, DateTime.Today.AddDays(i * -1).AddHours(_time).AddMinutes(_time_min));
                _time = _ram.Next(12, 23);
                _time_min = _ram.Next(0, 60);
                this.AddLog(CheckType.OUT, DateTime.Today.AddDays(i * -1).AddHours(_time).AddMinutes(_time_min));
            }
           */

        }

        public string Name { get; set; }
        public string Number { get; set; }
        public bool Active { get; set; }
        public string iButton { get; set; }
        private List<CheckLogs> _Logs;


        public List<CheckLogs> Logs
        {
            get
            {
                //_Logs.Sort((x, y) => x.CheckTime.CompareTo(y.CheckTime));
                _Logs = _Logs.OrderBy(L => L.CheckTime).ToList();
                return _Logs;
            }
            set
            {
                this._Logs = value;
            }
        }

        public string TotalHours
        {
            get
            {
                if (_Logs == null) return "0.00";
                else if (CheckLogError()) return "-.--";
                else
                {
                    int count = _Logs.Count;
                    /*
                    if (LastCheckLog() != null && LastCheckLog().CheckType == CheckType.IN)
                        count = _Logs.Count - 1;
                    else
                        count = _Logs.Count;
                     */
                    long ticket = 0;
                    var _tempLog = _Logs.OrderBy(L => L.CheckTime).ToList();
                    for (int i = 1; i < count; i++) {
                       // if (_tempLog[i].CheckType == CheckType.IN) continue;
                        if (_tempLog[i].CheckType == CheckType.OUT && _tempLog[i - 1].CheckType == CheckType.IN)
                        {
                            var _tempDate1 = new DateTime();
                            var _tempDate2 = new DateTime();
                            DateTime.TryParse(_tempLog[i].CheckTime.ToString(@"yyyy-MM-dd HH:mm"), out _tempDate1);
                            DateTime.TryParse(_tempLog[i - 1].CheckTime.ToString(@"yyyy-MM-dd HH:mm"), out _tempDate2);


                            ticket += _tempDate1.Subtract(_tempDate2).Ticks;

                            /*


                            minutes += _tempDate1.Subtract(_tempDate2).TotalMinutes;
                             * */

                        }
                        //else
                         //   return "-.--";
                    }

                    var resultDate = new DateTime(ticket);
                    var hours = resultDate.Hour;
                    var mins = resultDate.Minute;
                    hours += (resultDate.Day-1)*24;

                    return hours.ToString("00") + ":" + mins.ToString("00");  //Math.Round(minutes / 60, 2).ToString("F2");
                    //return minutes.ToString();
                }
            }        
        }

        public void AddLog(CheckType type, DateTime datetime) {
            var item = new CheckLogs(this, type, datetime);
            DropboxHelper.GetInstance().UploadCheckLogToDropbox(EmployeeManagement._context, item); 
            _Logs.Sort((x, y) => x.CheckTime.CompareTo(y.CheckTime));
        }
        public void AddLogWithoutSync(CheckType type, DateTime datetime, double thisX, double thisY) {
            var item = new CheckLogs(this, type, datetime, thisX, thisY);
            _Logs.Sort((x, y) => x.CheckTime.CompareTo(y.CheckTime));
        }
        //public bool DeleteCheckLog(string Id)
        public void DeleteCheckLog(CheckLogs log)
        {
            List<CheckLogs> logs = this._Logs;
            //var items = logs.Where(L => L.Id == Id);
            //CheckLogs log = logs.First(L => L.Id == Id);
            var count = logs.Count;
            logs.Remove(log);
            DropboxHelper.GetInstance().DeleteCheckLogFromDropBox(EmployeeManagement._context, log);
        }
        public void DeleteAllCheckLog()
        {
            DropboxHelper.GetInstance().ClearChecklogOfEmpolyee(EmployeeManagement._context, this);
            this._Logs = new List<CheckLogs>();
        }

        public CheckType CurrentType
        {
            get
            {
                if (_Logs.Count == 0) return CheckType.OUT;
                return _Logs.OrderByDescending(L => L.CheckTime).FirstOrDefault().CheckType;
            }
        }
        
        public Boolean CheckLogError()
        {
            var _tempLogs =  _Logs.OrderByDescending(L => L.CheckTime).ToList();

            for (int i = 1; i < _tempLogs.Count; i++)
            {
                if ((_tempLogs[i - 1].CheckType == CheckType.OUT && _tempLogs[i].CheckType == CheckType.OUT) || (_tempLogs[i - 1].CheckType == CheckType.IN && _tempLogs[i].CheckType == CheckType.IN))
                {
                    return true;
                }
            }
            return false;
        }
        
        public CheckLogs LastCheckLog()
        {
            if (_Logs != null)
                return _Logs.OrderByDescending(L => L.CheckTime).FirstOrDefault();
            else
                return null;
        }
    }
}