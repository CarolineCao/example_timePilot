//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Util;
//using Android.Views;
//using Android.Widget;
//using Java.Util;

//namespace TimePilot
//{
//    public class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
//     public class DatePickerFragment : DialogFragment
//    {
//        private Activity context = null;

//        public DatePickerFragment()
//        {
//        }

//        public DatePickerFragment(Activity context)
//        {
//            this.context = context;
//        }

//        public override Dialog OnCreateDialog(Bundle savedInstanceState)
//        {
//            Calendar cal = Calendar.GetInstance(Java.Util.TimeZone.Default);
//            int year = cal.Get(CalendarField.Year);
//            int month = cal.Get(CalendarField.Month);
//            int day = cal.Get(CalendarField.DayOfMonth);

//            return new DatePickerDialog(context, this, year, month, day);
//            EventHandler<DialogClickEventArgs> okhandler;
//            var builder = new AlertDialog.Builder(Activity).SetMessage("This is my dialog.").SetPositiveButton("ok", (sender, args) => { }).SetTitle("Custom Dialog");
//            return builder.Create();
//        }

//        public void OnDateSet(DatePicker view, int year, int month, int day)
//        {
//        }
//    }
//}