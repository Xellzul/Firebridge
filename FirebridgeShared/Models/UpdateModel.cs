using System;
using System.Collections.Generic;
using System.Text;

namespace FirebridgeShared.Models
{
    [Serializable]
    public class UpdateModel
    {
        public List<string> Names { get; set; }
        public List<byte[]> Data { get; set; }
    }
}
