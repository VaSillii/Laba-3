using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laba_3.Utils
{
    public class ResponseClient
    {
        public string Key { get; set; }
        public bool FlagAction { get; set; }
        public string DataTextOld { get; set; }
        public string DataTextNew { get; set; }
        public string DataFileOld { get; set; }
        public string DataFileNew { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

    }
}
