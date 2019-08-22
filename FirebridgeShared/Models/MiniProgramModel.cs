using System;
using System.Collections.Generic;
using System.Text;

namespace FirebridgeShared.Models
{
    [Serializable]
    public class MiniProgramModel
    {
        public List<string> References { get; set; }
        public string EntryPoint { get; set; }
        public string Code { get; set; }
    }
}
