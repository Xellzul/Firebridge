using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockPlugin
{
    [Serializable]
    public class UnlockArgs
    {
        public string Password { get; set; }
        public string Username { get; set; }
    }
}
