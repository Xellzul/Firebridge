using FirebridgeShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebridgeClient.Models
{
    class ZombieProgramModel
    {
        public MimiProgramModel Program { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
