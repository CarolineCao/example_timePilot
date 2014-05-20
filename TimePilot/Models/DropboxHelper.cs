using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RestSharp;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DropNet;
using DropNet.Models;
using Java.IO;
using Java.Lang;
using Android.Preferences;
using Android.Net;



namespace TimePilot.Models
{
    public class DropboxHelper
    {
        DropNetClient _client;
        static DropboxHelper _init = null;
        private bool _isLogined = false;
        private bool _ifConnected = true;

        ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(EmployeeManagement._context);
        public static DropboxHelper GetInstance()
        {
            if (_init == null)
            {
                _init = new DropboxHelper();
            }

            return _init;
        }
        private DropboxHelper()
        {
          
            //_client = new DropNetClient("yre3pe1t1rsub96", "i9qrsbs5mbzkn2g", "05kuo18k341af6z", "kjij2aqhmzjk9wm");
            try
            {
                var _setting = EmployeeManagement._context.GetSharedPreferences("dropbox_token", FileCreationMode.Private);
                string _userSecret = _setting.GetString("dropbox_secret", string.Empty);
                string _userToken = _setting.GetString("dropbox_token", string.Empty);
                if (string.IsNullOrEmpty(_userSecret) || string.IsNullOrEmpty(_userToken))
                {
                    _client = new DropNetClient("yre3pe1t1rsub96", "i9qrsbs5mbzkn2g");
                    _isLogined = false;
                }
                else
                {
                    _client = new DropNetClient("yre3pe1t1rsub96", "i9qrsbs5mbzkn2g", _userToken, _userSecret);
                    _isLogined = true;
                }
            }
            catch (System.Exception ex)
            {
                _client = new DropNetClient("yre3pe1t1rsub96", "i9qrsbs5mbzkn2g");
                _isLogined = false;
            }

            _client.UseSandbox = true;
        }

        public void LogOut()
        {
            var _setting = EmployeeManagement._context.GetSharedPreferences("dropbox_token", FileCreationMode.Private);
            var _editor = _setting.Edit();
            _editor.PutString("dropbox_secret", "");
            _editor.PutString("dropbox_token", "");
            _editor.Commit();

            ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(EmployeeManagement._context);
            var settingEdit = settings.Edit();
            settingEdit.PutBoolean("pref_dropboxOn", false);
            settingEdit.Commit();
            ///_client = new DropNetClient("yre3pe1t1rsub96", "i9qrsbs5mbzkn2g");
            
            _isLogined = false;

            this.ClearLocalfiles();
        }
        public bool IsLogined()
        {
            return _isLogined;
        }
        public bool IsConnected()
        {
            bool _currectStatus = NetworkConnectionMessageHelper.GetInstance().GetIfConnected();

            // Reconnect to dropbox
            if (!_ifConnected && _currectStatus && _isLogined)
            {
                _ifConnected = _currectStatus;
                EmployeeManagement.GetInstance().loadData(true, false);
            }
            if (_ifConnected && !_currectStatus && _isLogined)
            {
                try
                {
                    Toast toast = Toast.MakeText(EmployeeManagement._context.BaseContext, "No network now, DropboxSync will be off and trun on when network available", ToastLength.Long);
                    toast.Show();
                }
                catch (System.Exception ex) { 
                }
            }

            _ifConnected = _currectStatus;
            return _ifConnected;
        }
        public string GetAuthorizationURL()
        {
            // Google means it is succuessful signal.
            try
            {
                var authorizeUrl = _client.GetTokenAndBuildUrl("http://www.google.com");
                return authorizeUrl;
            }
            catch (DropNet.Exceptions.DropboxException ex)
            {
                return "";
            }
        }
        public bool SaveAccessToken()
        {
            try
            {
                var _token = _client.GetAccessToken();
                // Save token for next log in
                var _setting = EmployeeManagement._context.GetSharedPreferences("dropbox_token", FileCreationMode.Private);
                var _editor = _setting.Edit();
                _editor.PutString("dropbox_secret", _token.Secret);
                _editor.PutString("dropbox_token", _token.Token);
                _editor.Commit();

                //_client.UserLogin.Secret;
                //_client.UserLogin.Token;
                //if (_client.UserLogin == null || _client.UserLogin.Secret != _token.Secret || _client.UserLogin.Token != _token.Token)
                //{
                //    _client = new DropNetClient("yre3pe1t1rsub96", "i9qrsbs5mbzkn2g", _token.Token, _token.Secret);
                //}

                _isLogined = true;
                return true;
            }
            catch (DropNet.Exceptions.DropboxException ex)
            {
                _isLogined = false;
                return false;
            }
        }


        protected void ClearLocalfiles()
        {

            foreach (var _item in EmployeeManagement._context.FileList())
            {
                string item = (string)_item;
                string csv = ".csv";
                string CSV = ".CSV";
                if (item.IndexOf(csv) >= 0 || item.IndexOf(CSV) >= 0)
                {
                    EmployeeManagement._context.DeleteFile(item);
                }
            }
        }
        public  void SaveAsPrivateFile(Context ctx, byte[] data, string filename)
        {
            try
            {
                var fos = ctx.OpenFileOutput(filename, FileCreationMode.WorldWriteable);
                fos.Write(data, 0, data.Length);
                fos.Close();      
            }
            catch (Java.IO.IOException ex)
            {
                throw new System.Exception(ex.Message);
            }
        }
        public void SaveAsPrivateFileToDropboxRootFile(Context ctx, List<string> data, string filename)
        {
            List<byte> byteArray= new List<byte>();
            foreach(var item in data)
            {
                if (byteArray.Count!=0)
                {
                    byteArray.AddRange(System.Text.Encoding.Default.GetBytes("\r\n"));
                }
                byteArray.AddRange(System.Text.Encoding.Default.GetBytes(item.Replace(",","@%@")));
            }
            SaveAsPrivateFile(ctx, byteArray.ToArray(), filename);

            ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(EmployeeManagement._context);
            var IsDropboxOn = settings.GetBoolean("pref_dropboxOn", false);
            if (IsDropboxOn)
            {
                if (!IsConnected())
                {
                    return;
                }
                var uploaded = _client.UploadFile("/", filename, byteArray.ToArray());
            }
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filename"></param>
        /// <param name="IsTransFile">True: /TimePilot_aPhone/aPxxx/ Flase: /</param>
        public void SaveFileToDropbox(List<string> data, string filename, bool IsTransFile)
        {
            List<byte> byteArray = new List<byte>();
            foreach (var item in data)
            {
                if (byteArray.Count != 0)
                {
                    byteArray.AddRange(System.Text.Encoding.Default.GetBytes("\r\n"));
                }
                byteArray.AddRange(System.Text.Encoding.Default.GetBytes(item));
            }

          
            var IsDropboxOn = settings.GetBoolean("pref_dropboxOn", false);
            if (IsDropboxOn)
            {
                if (!IsConnected())
                {
                    return;
                }

                if (IsTransFile)
                {
                    //(new Thread(new Action(() => {
                        var phoneNumber = settings.GetString("pref_phoneNumber", "");
                        var uploaded = _client.UploadFile("/TimePilot_aPhone/" + "aP" + phoneNumber + "/", filename, byteArray.ToArray());
                    //}))).Start();
                }
                else
                {
                   ////// (new Thread(new Action(() => {
                        var phoneNumber = settings.GetString("pref_phoneNumber", "");
                        var uploaded = _client.UploadFile("/", filename, byteArray.ToArray());
                   // }))).Start();
                }
            }
        }

        public void SaveAsPrivateFileToDropboxTransFile(Context ctx, List<string> data, string filename) {
            List<byte> byteArray = new List<byte>();
            foreach (var item in data)
            {
                if (byteArray.Count != 0)
                {
                    byteArray.AddRange(System.Text.Encoding.Default.GetBytes("\r\n"));
                }
                byteArray.AddRange(System.Text.Encoding.Default.GetBytes(item));
            }
            SaveAsPrivateFile(ctx, byteArray.ToArray(), filename);

            SaveFileToDropbox(data, filename, true);
        }
         
        public string DownloadFileFromDropbox(string fileName)
        {
            byte[] downloadedByte = _client.GetFile("/"+fileName);
            string downloadedString = System.Text.Encoding.Default.GetString(downloadedByte);
            return downloadedString;
        }
        public Dictionary<int, List<MetaData>> getMetaData(string path, bool IsLoadChild)
        {
            Dictionary<int, List<MetaData>> _MetaData = new Dictionary<int, List<MetaData>>();
            if (!IsConnected())
            {
                return _MetaData;
            }

            try
            {
                var metaData = _client.GetMetaData(path,true);
                var _Contents = metaData.Contents;
                for (int i = 0; i < _Contents.Count; i++)
                {
                    if (_Contents[i].Is_Deleted)
                    {
                        if (!_MetaData.ContainsKey(0))
                        {
                            _MetaData.Add(0, new List<MetaData>());
                        }
                        _MetaData[0].Add(_Contents[i]);
                        continue;
                    }
                    if (_Contents[i].Is_Dir == true && IsLoadChild)
                    {
                        var _metaData =  getMetaData(_Contents[i].Path.ToString(), IsLoadChild);
                        foreach (var item in _metaData)
                        {
                            if (!_MetaData.ContainsKey(item.Key))
                            {
                                _MetaData.Add(item.Key, new List<MetaData>());
                            }
                            _MetaData[item.Key].AddRange(item.Value);
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(_Contents[i]);
                        if (!_MetaData.ContainsKey(1))
                        {
                            _MetaData.Add(1, new List<MetaData>());
                        }
                        _MetaData[1].Add(_Contents[i]);
                    }
                }
            }
            catch (DropNet.Exceptions.DropboxException ex)
            {
                
            }
            return _MetaData;
        }
        public List<MetaData> getDeletedMetaData(string path, bool IsLoadChild)
        {
            List<MetaData> _MetaData = new List<MetaData>();
            if (!IsConnected())
            {
                return _MetaData;
            }

            try
            {
                var metaData = _client.GetMetaData(path);
                var _Contents = metaData.Contents;
                for (int i = 0; i < _Contents.Count; i++)
                {
                    if (_Contents[i].Is_Deleted)
                    {
                        _MetaData.Add(_Contents[i]);
                    }
                }
            }
            catch (DropNet.Exceptions.DropboxException ex)
            {

            }
            return _MetaData;
        }
        public void syncDropboxCsvFiles(Context ctx, bool IsNeedToDelete)
        {
            if (!IsConnected())
            {
                return;
            }

            Dictionary<string, DateTime> _AndroidlFilesModifiedDate = new Dictionary<string, DateTime>();
            Dictionary<string, byte[]> _AndroidlFilesContent = new Dictionary<string, byte[]>();

            var _localFiles = ctx.FileList();
            foreach (var _item in _localFiles)
            {
                if (_item.IndexOf(".csv") <= 0 && _item.IndexOf(".CSV") <= 0)
                {
                    continue;
                }
                var AndroidfilemodifiedDate = System.IO.File.GetLastWriteTimeUtc(_item);
                _AndroidlFilesModifiedDate.Add(_item, AndroidfilemodifiedDate);
            }

            var _list = new List<MetaData>(); //getMetaData("/", false); // Employee
            var _DeletedFileslist = new List<MetaData>();
            string number = settings.GetString("pref_phoneNumber", string.Empty);
            if (!string.IsNullOrEmpty(number))
            {
                var _subList = getMetaData("/TimePilot_aPhone/aP" + number, false);
                if(_subList.ContainsKey(1))
                 _list.AddRange(_subList[1].ToList());

                if(_subList.ContainsKey(0))
                    _DeletedFileslist.AddRange(_subList[0].ToList());
            }
            foreach (var item in _list)
            {
                if (item.Extension==".csv" || item.Extension==".CSV")
                {
                    if (_AndroidlFilesModifiedDate.ContainsKey(item.Name))
                    {
                        var AndroidFileDate = _AndroidlFilesModifiedDate[item.Name];
                        if (AndroidFileDate < item.UTCDateModified)
                        {
                            try
                            {
                                byte[] dropboxFileContent = _client.GetFile(item.Path);
                                SaveAsPrivateFile(ctx, dropboxFileContent, item.Name);
                            }
                            catch (DropNet.Exceptions.DropboxException ex)
                            {
                                //throw new System.Exception(ex.Message);
                            }
                            catch (System.Exception ex)
                            {
                                //throw new System.Exception(ex.Message);
                            }
                        }
                    } 
                    else {
                        byte[] dropboxFileContent = _client.GetFile(item.Path);
                        SaveAsPrivateFile(ctx, dropboxFileContent, item.Name);
                    }
                }
            }

            foreach (var item in _DeletedFileslist)
            {
                if (EmployeeManagement.DeletedFilesName == null)
                {
                    EmployeeManagement.DeletedFilesName = new List<string>();
                }
                EmployeeManagement.DeletedFilesName.Add(item.Name.Substring(0, item.Name.LastIndexOf(".")-1));
            }

            if (IsNeedToDelete)
            {
                var _listTransFile = _list.Where(L => L.Name.Contains("Trans"));
                foreach (var item in _DeletedFileslist)
                {
                    var _theFile = _listTransFile.Where(L => item.Name.Equals(L.Name)).FirstOrDefault();
                    if (_theFile == null)
                    {
                        ctx.DeleteFile(item.Name);
                    }
                }
            }

        }

	    public static string[] readFile(string path) {
		    BufferedReader input = new BufferedReader(new FileReader(path));
            string line = null;
            List<string> _list = new List<string>();
		    while ((line = input.ReadLine()) != null) {
                line = line.Replace("@%@", ",");
                _list.Add(line);
		    }
            input.Close();
            return _list.ToArray();
	    }
        
        public List<Employee> importEmployeeToAndroid(Context ctx)
        {
            string marker = "@!#:#@!";

            List<Employee> EmployeeList=new List<Employee>();
            //Dictionary<string, byte[]> _AndroidlFilesContent = new Dictionary<string, byte[]>();
            foreach (var _item in ctx.FileList())
            {
                string item=(string)_item;
                string csv = ".csv";
                string CSV = ".CSV";
                if (item.IndexOf(csv) >= 0 || item.IndexOf(CSV) >= 0)
                {
                    if (item.IndexOf("Trans") == 0)
                    {
                        continue;
                    }

                    var file = ctx.GetFileStreamPath(item);
                    var line = readFile(file.AbsolutePath);
                    if (line.Length == 0)
                    {
                        continue;
                    }

                    string firstLine = line[0];
                    int startIndex = firstLine.IndexOf("\"", 0);
                    int endIndex = 0;
                    if (startIndex > 0)
                    {
                        endIndex = firstLine.IndexOf("\"", startIndex+ 1);
                    }
                    if (startIndex > 0 && endIndex > startIndex)
                    {
                        string oldString = firstLine.Substring(startIndex, endIndex-1);
                        string newString = oldString.Replace(",", marker);
                        firstLine = firstLine.Replace(oldString, newString);
                    }

                    var cells =  firstLine.Split(',');
                    
                    string employeeName;
                    if (cells[1].IndexOf("\"") >= 0)
                    {
                        employeeName = cells[1].Substring(1, cells[1].Length - 2).Replace(marker,",");
                    }
                    else
                        employeeName = cells[1].Replace(marker, ",");
                    if (cells.Length < 26)
                        continue;

                    Employee e = new Employee();
                    e.Name = employeeName;
                    e.Number = cells[2].Replace(marker, ",");
                    EmployeeList.Add(e);
              }
            }
            return EmployeeList;
        }
        public void importChecklogsToAndroid(Context ctx, List<Employee> Employees)
        {
            IsConnected();

            var files = ctx.FileList();
            foreach (var _item in Employees) 
            {
                var fileName = files.Where(L => L.ToLower().Equals("trans-ap"+_item.Number + ".csv")).FirstOrDefault();
                if (fileName == null) continue;
                var file = ctx.GetFileStreamPath(fileName);
                var lines = readFile(file.AbsolutePath);
                if (lines.Length == 0)
                {
                    continue;
                }   

                foreach (var line in lines) {
                    var cells = line.Split(',');
                    var cells_new = new List<string>();
                    foreach (var cell in cells)
                    {
                        if (cell.Length > 1 && cell.Substring(0, 1) == "\"" && cell.Substring(cell.Length - 2, 1) == "\"")
                        {
                            cells_new.Add(cell.Substring(1, cell.Length - 2));
                        }
                        else
                        {
                            cells_new.Add(cell);
                        }
                    }

                    if (cells.Length < 6 || cells_new.Count < 6) continue;

                    DateTime _time = DateTime.MinValue;
                    DateTime.TryParse(cells_new[3], out _time);
                    
                    //TODO: ´ÓÎÄ¼þ¶Áx and y
                    if (cells_new[0] == "0")
                    {
                        if (cells_new.Count > 6)
                        {
                            _item.AddLogWithoutSync(CheckType.OUT, _time, Convert.ToDouble(cells_new[5]), Convert.ToDouble(cells_new[6]));
                        }
                    }
                    else if (cells_new[0] == "1")
                    {
                        if (cells_new.Count > 6)
                        {
                            _item.AddLogWithoutSync(CheckType.IN, _time, Convert.ToDouble(cells_new[5]), Convert.ToDouble(cells_new[6]));
                        }
                    }
                }
            }
        }
        public void UploadLocalExistingChecklogToDropbox(Context ctx)
        {
            var files = ctx.FileList();
            foreach (var _item in files.Where(L => L.Contains("Trans-a") || L.Contains("name")))
            {
                var file = ctx.GetFileStreamPath(_item);
                var lines = readFile(file.AbsolutePath);
                if (lines.Length == 0)
                {
                    continue;
                }

                SaveFileToDropbox(lines.ToList(), _item, true);
            }
        }
        public void UploadLocalExistingEmployeeToDropbox(Context ctx)
        {
            var files = ctx.FileList();
            foreach (var _item in files.Where(L => !L.Contains("Trans-a")))
            {
                if (!_item.Contains(".CSV"))
                {
                    continue;
                }

                var file = ctx.GetFileStreamPath(_item);
                var lines = readFile(file.AbsolutePath);
                if (lines.Length == 0)
                {
                    continue;
                }
                ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(EmployeeManagement._context);
                string number = settings.GetString("pref_phoneNumber", string.Empty);
                var _list = new List<MetaData>();
                if (!string.IsNullOrEmpty(number))
                {

                    var _subList = getMetaData("/TimePilot_aPhone/aP" + number, false);
                    if (_subList.ContainsKey(1))
                        _list.AddRange(_subList[1].ToList());
                }
                var _dropboxFile = _list.Where(L => !L.Is_Deleted && !L.Is_Dir && L.Name.Equals(_item)).FirstOrDefault();

                if (_dropboxFile == null) 
                {
                    SaveFileToDropbox(lines.ToList(), _item, true);
                    
                }
            }
        }
        public void UploadCheckLogToDropbox(Context ctx, CheckLogs checklogs)
        {
            var files = ctx.FileList();
            var _CheckType = 0;
            if(checklogs.CheckType.Equals(CheckType.IN))
            _CheckType = 1;
            string Name = checklogs.employee.Name;
            string Number = checklogs.employee.Number;
            string CheckTime = checklogs.CheckTime.ToString(@"yyyy-MM-dd HH:mm:ss");
            var _localtion = GPSHelper.GetInstance().GetLocation();

            ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(EmployeeManagement._context);
            string phoneName = "";
            phoneName = settings.GetString("pref_phoneName", "");
            string content = string.Format("{0},\"{1}\",{2},{3},{4},{5},{6}", _CheckType, Name, Number, CheckTime, phoneName, checklogs.x, checklogs.y);
            var fileName = files.Where(L => L.ToLower().Equals("trans-ap" + checklogs.employee.Number + ".csv")).FirstOrDefault();
            List<string> list = new List<string>();
               if (fileName == null)
               {
                   fileName = "Trans-aP" + checklogs.employee.Number + ".csv";

                   list.Add(content);
                   SaveAsPrivateFileToDropboxTransFile(ctx, list, fileName);
               }
               else
               {
                   var file = ctx.GetFileStreamPath(fileName);
                   var lines = readFile(file.AbsolutePath);
                   int count = lines.Length;
                   
                   foreach (var line in lines)
                   {
                       list.Add(line);
                   }
                   
                   list.Add(content);

                   SaveAsPrivateFileToDropboxTransFile(ctx, list, fileName); 
               }
        }
        public void DeleteEmployeeFromDropBoxandLocalFile(Context ctx, Employee employee)
        {
            var files = ctx.FileList();
            ctx.DeleteFile(employee.Number + ".CSV");
            string number = settings.GetString("pref_phoneNumber", string.Empty);
            if (!string.IsNullOrEmpty(number) && this.IsLogined())
            {
                try
                {
                    _client.Delete("/TimePilot_aPhone/aP" + number + "/" + employee.Number + ".CSV");
                }
                catch(DropNet.Exceptions.DropboxException ex)
                {
                    throw new System.Exception(ex.Message);
                }
            }
          
        }
        public void DeleteCheckLogFromDropBox(Context ctx, CheckLogs log)
        {
            var files = ctx.FileList();
            var fileName = files.Where(L => L.ToLower().Equals("trans-ap" + log.employee.Number + ".csv")).FirstOrDefault();
            List<string> list = new List<string>();

            var file = ctx.GetFileStreamPath(fileName);
            var lines = readFile(file.AbsolutePath);
            int count = lines.Length;

            // Needed so only first instance is deleted in case of duplicate transactions
            bool firstDeleted = false;

            foreach (var line in lines)
            {
                var comma = ',';
                var cells = line.Split(comma);
                var _CheckType = 0;
                if (log.CheckType.Equals(CheckType.IN))
                    _CheckType = 1;

                var match = cells[0].Equals(_CheckType.ToString()) && cells[1].Equals("\""+log.employee.Name+"\"") && cells[2].Equals(log.employee.Number) && cells[3].Equals(log.CheckTime.ToString(@"yyyy-MM-dd HH:mm:ss")) ;
                // If not a match, or if a match exists but a log was already deleted, then add it to the list.
                if (!match || (match && firstDeleted))
                    list.Add(line);
                else
                    firstDeleted = true; // If test fails, that means a match was found, make sure not to delete future duplicates
            }

            SaveAsPrivateFileToDropboxTransFile(ctx, list, fileName);
        }
        public void ClearChecklogOfEmpolyee(Context ctx, Employee employee)
        {
            var files = ctx.FileList();
            ctx.DeleteFile("Trans-aP" + employee.Number + ".csv");
            string number = settings.GetString("pref_phoneNumber", string.Empty);
            if (!string.IsNullOrEmpty(number))
            {
                try
                {
                    _client.Delete("/TimePilot_aPhone/aP" + number + "/Trans-aP" + employee.Number + ".csv");
                }
                catch (DropNet.Exceptions.DropboxException ex)
                {
                    throw new System.Exception(ex.Message);
                }
            }
        }
       public void CreateEmployeeCsvFileToDropbox(Context ctx, Employee employee)
       {
           string filename = employee.Number.ToString() + ".CSV";
           List<string> data = new List<string>();
           var _localtion = GPSHelper.GetInstance().GetLocation();
           string GPSx = "";
           string GPSy = "";
           if (_localtion != null)
           {
               GPSx = _localtion.Latitude.ToString();
               GPSy = _localtion.Longitude.ToString();
           }
           var val_0=0;
           if(employee.CurrentType.Equals(CheckType.IN))
           val_0 = 1;
          
           var val_1 = employee.Name.ToString();
           var val_2 = employee.Number.ToString();
           var val_3 = DateTime.Today.ToString();
           var val_4 = employee.TotalHours.ToString();
           string content = string.Format("{0},\"{1}\",{2},,{3},{4},,,,,,,,,,,,,,,,,,,,{5},{6}", val_0, val_1, val_2, val_3, val_4, GPSx, GPSy);
           data.Add(content);
           SaveAsPrivateFileToDropboxTransFile(ctx, data, filename);
       }
       public void ModifyEmployeeName(Context ctx, string EmployeeNumber, string EmployeeName)
       {
           var fileName = ctx.FileList().Where(L => L.Equals(EmployeeNumber + ".CSV")).FirstOrDefault();
           if (string.IsNullOrEmpty(fileName)) return;

           var file=ctx.GetFileStreamPath(fileName);
           var line = readFile(file.AbsolutePath);
           string firstLine = line[0];
           var cells = firstLine.Split(',');
           cells[1] = "\""+EmployeeName+"\"";
           string content= string.Join(",", cells);
           SaveAsPrivateFileToDropboxTransFile(ctx, (new List<string>() { content }), fileName);
       
       }

      
        }
       
    }

 
     
