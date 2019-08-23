using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FirebridgeShared
{
    public static class Extensions
    {
        public static void SafeInvoke(this Delegate dele, object sender , EventArgs e)
        {
            if (dele == null) return;

            foreach (Delegate singleCast in dele.GetInvocationList())
            {
                var syncInvoke = singleCast.Target as ISynchronizeInvoke;

                if (syncInvoke != null && syncInvoke.InvokeRequired)
                    syncInvoke.BeginInvoke(singleCast, new object[] { sender,  e });
                else
                    singleCast.DynamicInvoke(sender, e);
            }
        }
    }
}
