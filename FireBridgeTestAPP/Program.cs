using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FirebridgeShared;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using FirebridgeShared.Networking;

namespace FireBridgeTestAPP
{
    class Program
    {
        static string[] GetCode()
        {
            return new string[]
            {
                @"using System;
 
                namespace DynamicNS
                {
                    public static class DynamicCode
                    {
                        public static void Main()
                        {
                            Console.WriteLine(""Hello, world!"");
                        }
                    }
                }"
            };
        }

        static void Main(string[] args)
        {
            //CSharpCodeProvider provider = new CSharpCodeProvider();
            //CompilerParameters parameters = new CompilerParameters();
            //parameters.GenerateInMemory = true;
            //parameters.ReferencedAssemblies.Add("System.dll");
            //CompilerResults results = provider.CompileAssemblyFromSource(parameters, GetCode());
            //results.Errors.Cast<CompilerError>().ToList().ForEach(error => Console.WriteLine(error.ErrorText));

            //var cls = results.CompiledAssembly.GetType("DynamicNS.DynamicCode");
            //var method = cls.GetMethod("Main", BindingFlags.Static | BindingFlags.Public);
            //method.Invoke(null, null);



            Server s = new Server();
            s.ClientConnected += S_ClientConnected;
            s.Start();
            Console.ReadKey();
        }

        private static void S_ClientConnected(object sender, EventArgs e)
        {
            ((ServerConnectionEventArgs)e).Client.Run();
        }
    }
}
