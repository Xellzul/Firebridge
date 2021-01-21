using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FireBridgeCore.Networking
{
    public enum Status
    {
        Invalid,
        Starting,
        Stopping,
        FailedToStart
    }
    [Serializable]
    public class RemoteProcessStatusChangedModel
    {
        public Status Status { get; set; } = Status.Invalid;
        public bool Errored { get; set; } = false;
        public string ErrorMessage { get; set; } = "";
    }
}
