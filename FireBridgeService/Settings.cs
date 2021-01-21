using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace FireBridgeService
{

    public class Settings
    {
        //custom settings? Dictianory<>
        public Settings()
        {
            Guid = new Guid();
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
