using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FireBridgeController
{
    public class Settings
    {
        //custom settings? Dictianory<>
        public Settings()
        {
            Guid = Guid.NewGuid();
        }

        public Guid Guid { get; set; }
        public bool Save(string file)
        {
            try
            {
                File.WriteAllText(file, JsonSerializer.Serialize<Settings>(this));
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static Settings Load(string file)
        {
            try
            {
                return JsonSerializer.Deserialize<Settings>(File.ReadAllText(file));
            }
            catch
            {
                return null;
            }
        }
    }
}
