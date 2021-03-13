using FireBridgeCore.Kernel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace FireBridgeCore.Controller
{
    public sealed class PluginManager
    {
        static PluginManager _instance;

        public static PluginManager Instance
        {
            get { return _instance ??= new PluginManager(); }
        }
        private PluginManager() { }

        public SortedSet<FireBridgePlugin> Plugins = new SortedSet<FireBridgePlugin>(
            Comparer<FireBridgePlugin>.Create((a, b) => a.Order.CompareTo(b.Order))
            );

        public void StartPlugins()
        {
            foreach (var plugin in Plugins)
            {
                plugin.Start();
            }
        }

        public bool LoadPlugin(string file)
        {
            try
            {
                var data = File.ReadAllBytes(file);

                Assembly asm = AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(data, false));

                foreach (Type type in asm.GetTypes())
                {
                    if (type.GetCustomAttributes(typeof(FireBridgePluginAttribute), true).Length > 0)
                    {
                        var plugin = (FireBridgePlugin)Activator.CreateInstance(type);
                        plugin.AssemblyData = data;
                        Plugins.Add(plugin);
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void LoadFolder(string folder)
        {
            if (!Directory.Exists(folder))
                return;

            foreach (var file in Directory.GetFiles(folder))
            {
                if (!file.EndsWith(".dll"))
                    continue;

                LoadPlugin(file);
            }
        }
    }
}