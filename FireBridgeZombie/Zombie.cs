using FirebridgeShared.Models;
using FirebridgeShared.Networking;
using Microsoft.CSharp;
using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace FireBridgeZombie
{
    class Zombie
    {

        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const int MOUSEEVENTF_RIGHTUP = 0x010;

        public Zombie()
        {
            s = new Server();
            DiscoveryServer = new DiscoveryServer();
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            s.ClientConnected += S_ClientConnected;
            s.ConnectionDisconnected += S_ConnectionDisconnected;
        }

        private void S_ConnectionDisconnected(object sender, EventArgs e)
        {
            var e2 = (ServerConnectionEventArgs)e;
        }

        private void Connection_MessageRecieved(object sender, EventArgs e)
        {
            var packet = ((MessageEventArgs)e).Packet;
            var connection = (Connection)sender;


            switch (packet.Id)
            {
                case 1: //Run Code


                    var code = (MiniProgramModel)packet.Data;
                    CSharpCodeProvider provider = new CSharpCodeProvider();
                    CompilerParameters parameters = new CompilerParameters();
                    parameters.GenerateInMemory = true;
                    parameters.ReferencedAssemblies.AddRange(code.References.ToArray());
                    CompilerResults results = provider.CompileAssemblyFromSource(parameters, code.Code);
                    results.Errors.Cast<CompilerError>().ToList().ForEach(error =>
                    {
                        Trace.WriteLine(error.ErrorText);
                        connection.SendPacket(new Packet() { Id = 0, Data = error.ErrorText });
                    });

                    if (results.Errors.Count > 0)
                        break;

                    IPrincipal principal;


                    Thread.CurrentPrincipal = WindowsIdentity.GetCurrent();
                    WindowsIdentity identity = principal.Identity as WindowsIdentity;
                    WindowsImpersonationContext impersonationContext = null;
                    if (identity != null)
                    {
                        impersonationContext = identity.Impersonate();
                    }
                    try
                    {
                        // your code here
                    }
                    finally
                    {
                        if (impersonationContext != null)
                        {
                            impersonationContext.Undo();
                        }
                    }



                    var cls = results.CompiledAssembly.GetType(code.EntryPoint);
                    var method = cls.GetMethod("Main", BindingFlags.Static | BindingFlags.Public);


                    IntPtr accessToken = ApplicationLoader.GetHandle();
                    WindowsIdentity identity = new WindowsIdentity(accessToken);
                    WindowsImpersonationContext context = identity.Impersonate();

                    method.Invoke(null, new[] { sender });

                    context.Undo();

                    break;
                case 2: //Restart
                    s.Stop();
                    DiscoveryServer.Stop();
                    Environment.Exit((int)packet.Data);
                    break;
                case 3: //Screenshot
                    var screenshotRequestModel = (ScreenshotRequestModel)packet.Data;
                    Rectangle bounds = Screen.GetBounds(Point.Empty);
                    using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                    {
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                        }

                        var brush = new SolidBrush(Color.Black);

                        var bmp = new Bitmap(screenshotRequestModel.Width, screenshotRequestModel.Height);
                        var graph = Graphics.FromImage(bmp);


                        graph.InterpolationMode = InterpolationMode.Low;
                        graph.CompositingQuality = CompositingQuality.HighSpeed;
                        graph.SmoothingMode = SmoothingMode.HighSpeed;

                        graph.FillRectangle(brush, new RectangleF(0, 0, screenshotRequestModel.Width, screenshotRequestModel.Height));
                        graph.DrawImage(bitmap, 0, 0, screenshotRequestModel.Width, screenshotRequestModel.Height);


                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                        System.Drawing.Imaging.Encoder myEncoder =
                            System.Drawing.Imaging.Encoder.Quality;

                        EncoderParameters myEncoderParameters = new EncoderParameters(1);

                        EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                        myEncoderParameters.Param[0] = myEncoderParameter;

                        Bitmap bmp2;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bmp.Save(ms, jpgEncoder, myEncoderParameters);
                            ms.Flush();
                            bmp2 = (Bitmap)Image.FromStream(ms);
                            connection.SendPacket(new Packet() { Id = 3, Data = bmp2 });
                        }
                    }
                    break;
                case 4: //Update
                    var data = (UpdateModel)packet.Data;
                    for (int i = 0; i < data.Names.Count; i++)
                        File.WriteAllBytes(data.Names[i] + ".new", data.Data[i]);

                    s.Stop();
                    DiscoveryServer.Stop();
                    Environment.Exit(69);
                    break;
                    break;
                case 8: //SetMousePos
                    {
                        var model = (ClickModel)packet.Data;

                        int x = (int)(Screen.PrimaryScreen.Bounds.Width * model.X);
                        int y = (int)(Screen.PrimaryScreen.Bounds.Height * model.Y);

                        SetCursorPos(x, y);
                    }
                    break;
                case 9: //click
                    {
                        var model = (ClickModel)packet.Data;
                        
                        int x = (int)(Screen.PrimaryScreen.Bounds.Width * model.X);
                        int y = (int)(Screen.PrimaryScreen.Bounds.Height * model.Y);

                        if (model.LeftMouse)
                        {
                            mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
                            mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
                        }
                        else
                        {
                            mouse_event(MOUSEEVENTF_RIGHTDOWN, x, y, 0, 0);
                            mouse_event(MOUSEEVENTF_RIGHTUP, x, y, 0, 0);
                        }
                    }
                    break;
                case 10:

                    break;
                default:
                    Trace.WriteLine("Unknown Packet of " + packet.Id);
                    connection.SendPacket(new Packet() { Id = 0, Data = "Unknown Packet of " + packet.Id });
                    break;
            }
        }

}
