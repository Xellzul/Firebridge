using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBridgeCore.Kernel
{
    public enum IIntegrityLevel
    {
        System, // System
        High,   // Administrator
        Medium, // Normal User
        Low     // below logged in user
    }
}
