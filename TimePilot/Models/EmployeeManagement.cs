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
using System.IO;
using Java.IO;
using Android.Preferences;
using Java.Lang;


namespace TimePilot.Models
{
    public class EmployeeManagement
    {
        static EmployeeManagement _init = null;
        public static EmployeeManagement GetInstance()
        {
            if (_init == null)
            {
                _init = new EmployeeManagement();
            }

            return _init;
        }
        public static EmployeeActivity _context { get; set; }
        public static List<string> DeletedFilesName { get; set; }

        private EmployeeManagement()
        {
            _employees = new List<Employee>();

        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="IsNeedUpload">  </param>
        public void loadData(bool IsNeedUpload, bool isFirsttime, bool IsNeedtoDelete = true, bool IsNeedToShowMsg = false)
        {
            // 单出提示
            ProgressDialog _progressDialog = null;

            if (IsNeedToShowMsg)
            {
                _context.RunOnUiThread(new Action(() =>
                {
                    _progressDialog = new ProgressDialog(_context);
                    _progressDialog.SetMessage("Loading local files...");
                    try
                    {
                        _progressDialog.Show();
                    }
                    catch (System.Exception ex)
                    {
                        _progressDialog = null;
                    }
                }));
            }

            // 加载本机Employee同checklog
            _employees = DropboxHelper.GetInstance().importEmployeeToAndroid(_context);
            DropboxHelper.GetInstance().importChecklogsToAndroid(_context, _employees);

            // 通知主线程更新界面
            bool IsDropboxOn = false;
            ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(EmployeeManagement._context);
            IsDropboxOn = settings.GetBoolean("pref_dropboxOn", false);



            if (IsNeedToShowMsg)
            {
                _context.RunOnUiThread(new Action(() =>
                {
                    try
                    {
                        if (_progressDialog != null)
                            _progressDialog.Dismiss();
                    }
                    catch (Java.Lang.Exception ex)
                    {
                    }

                    if (IsDropboxOn)
                    {
                        _context.DisplayLoadingMessage();
                    }
                    ((EmployeeActivity)_context).Update();
                    //((EmployeesAdapter)_context.ListAdapter).NotifyDataSetChanged(); 

                }));
            }
            else
            {
                _context.RunOnUiThread(new Action(() =>
                {
                    if (IsDropboxOn)
                    {
                        _context.DisplayLoadingMessage();
                    }
                    ((EmployeeActivity)_context).Update();
                }));
            }


            // Determine whether sync dropbox is needed and logged in dropbox aready or not.

            bool isConnected = false;

            isConnected = NetworkConnectionMessageHelper.GetInstance().GetIfConnected();
            Toast toast;
        
            if (isFirsttime)
            {
                var _listData = new List<string>();
                string PhoneName = "";
                PhoneName = settings.GetString("pref_phoneName", "");
                _listData.Add(PhoneName);
                DropboxHelper.GetInstance().SaveAsPrivateFileToDropboxTransFile(_context, _listData, "name.txt");
            }
            else
            {
                var _nameFile = DropboxHelper.GetInstance().getMetaData("name.txt", false);
                if (_nameFile.Count == 0)
                {
                    var _listData = new List<string>();
                    string PhoneName = "";
                    PhoneName = settings.GetString("pref_phoneName", "");
                    _listData.Add(PhoneName);
                    DropboxHelper.GetInstance().SaveAsPrivateFileToDropboxTransFile(_context, _listData, "name.txt");
                }
            }
            if (IsDropboxOn&&isConnected)
            {

                // 同步dropbox文件到本地
                DropboxHelper.GetInstance().syncDropboxCsvFiles(_context, IsNeedtoDelete);

                if (IsNeedUpload)
                {
                    DropboxHelper.GetInstance().UploadLocalExistingChecklogToDropbox(_context);
                    DropboxHelper.GetInstance().UploadLocalExistingEmployeeToDropbox(_context);
                }

                if (IsNeedToShowMsg)
                {
                    _context.RunOnUiThread(new Action(() =>
                    {
                        _context.HideLoadingMessage();
                        if (_progressDialog == null)
                        {
                            _progressDialog = new ProgressDialog(_context);
                        }

                        _progressDialog.SetMessage("Complete Loading Dropbox files...");
                        try
                        {
                            _progressDialog.Show();
                        }
                        catch (System.Exception ex)
                        {
                            _progressDialog = null;
                        }
                    }));
                }
                else
                {
                    _context.RunOnUiThread(new Action(() =>
                    {
                        _context.HideLoadingMessage();
                    }));
                }

                    // 从新加载emplyoee同checklog，从本地文件到内存
                    _employees = DropboxHelper.GetInstance().importEmployeeToAndroid(_context);
                    DropboxHelper.GetInstance().importChecklogsToAndroid(_context, _employees);

                    // 更新界面
                    _context.RunOnUiThread(new Action(() =>
                    {
                        ((EmployeeActivity)_context).Update();
                        if (_progressDialog != null)
                        {
                            _progressDialog.Dismiss();
                        }
                    }));
               
            }
            else
            {
            
                if (!isConnected && IsDropboxOn)
                {
                    _context.RunOnUiThread(new Action(() => {
                        toast = Toast.MakeText(_context, "No Network Connection Now. DropboxSycn will be off until network available", ToastLength.Long);
                        toast.Show();
                    }));
                }
                // Update the interface
                _context.RunOnUiThread(new Action(() => {
                    _context.HideLoadingMessage();
                }));
            }

            
            if (_employees.Count == 0)
            {
                _context.RunOnUiThread(new Action(() =>
                {
                    ((EmployeeActivity)_context).NewEmpolyee();
                }));
            }
            
        }

        public void SaveData()
        {
            //TODO:
        }

        private List<Employee> _employees { get; set; }

        public List<Employee> Employees
        {
            get
            {
                if (_employees == null)
                    return new List<Employee>();

                return _employees.Where(L => L.Active).OrderBy(L => L.Name).ToList();
                //return _employees.Where(L => L.Active).OrderBy(O => O.Name).ToList();
            }
        }

        public void ChangeEmployeeName(string Number, string Name)
        {
            var _currentEmployee =  _employees.FirstOrDefault(L => L.Number.Equals(Number));
            if (_currentEmployee != null)
            {
                _currentEmployee.Name = Name;
            }
        }
        public void AddEmployee(string Name, string Number, bool Active)
        {
            Employee item = new Employee();

            item.Name = Name;
            item.Number = Number;
            item.Active = Active;

            if (_employees == null)
            {
                _employees = new List<Employee>();
            }
            _employees.Add(item);
            DropboxHelper.GetInstance().CreateEmployeeCsvFileToDropbox(_context, item);
        }

        public void DeleteEmployee(string Number)
        {
            if (_employees == null)
            {
                _employees = new List<Employee>();
            }
            var items = _employees.Where(L => L.Number == Number);
            foreach (var _item in items)
            {
                _item.Active = false;
                (new Thread(new Action(() => { DropboxHelper.GetInstance().DeleteEmployeeFromDropBoxandLocalFile(_context, _item); }))).Start();
            }

            if (DeletedFilesName == null)
            {
                DeletedFilesName = new List<string>();
            }
            DeletedFilesName.Add(Number);
        }
        public bool ValidUserID(string Number)
        {
            
            if (_employees == null) return true;
            if (_employees.Any(L => L.Name.Equals(Number)))
            {
                return false;
            }
            if (DeletedFilesName != null)
            {
                if (DeletedFilesName.Any(L => L.Equals(Number)))
                    return false;
            }
            return true;

            /*
            bool valid = true;
            for (int i = 0; i < _employees.Count; i++)
            {
                if (_employees[i].Number.Equals(Number) && _employees[i].Active==true)
                    {
                        valid = false;
                    }
            }
            return valid;
            */
        }       
     
    }
}