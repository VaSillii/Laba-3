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

namespace AppLaba3.Utils
{
    public class HandleData
    {
        public string Key { get; set; }
        public bool FlagAction { get; set; }
        public string DataText { get; set; }
        public string DataFile { get; set; }

        public HandleData(string key, bool flagAction, string dataText, string dataFile)
        {
            Key = key;
            FlagAction = flagAction;
            DataText = dataText;
            DataFile = dataFile;
        }
    }
}