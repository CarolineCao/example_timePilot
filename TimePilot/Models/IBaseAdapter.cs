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
    interface IBaseAdapter
    {
        List<View> Cells { get; set; }
        void OnChangeCellStatus(bool status);
    }
}