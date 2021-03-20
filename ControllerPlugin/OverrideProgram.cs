using FireBridgeCore.Kernel;
using FireBridgeCore.Networking;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControllerPlugin
{
    [Serializable]
    public class OverrideProgram : UserProgram
    {
        int WaitTime = 1000;
        int quality = 70;
        int width = 720;
        int heigth = 480;
        bool takeScreenShot = true;
        bool shouldEnd = false;
        public override void Main(UserProgramContainer container, object args)
        {
            if(args is OverridePorgramSettings)
            {
                OverridePorgramSettings ops = (OverridePorgramSettings)args;
                this.heigth = ops.Heigth;
                this.width = ops.Width;
                this.WaitTime = ops.WaitTime;
                this.quality = ops.Quality;
                this.takeScreenShot = ops.TakeScreenShot;
            }

            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            while (container.Connection.Status == ConnectionStatus.Connected && !shouldEnd)
            {
                
                Thread.Sleep(WaitTime);
                if (takeScreenShot)
                {
                    try { 
                        var screenShot = GetScreenshot();
                        container.Respond(screenShot);
                        container.Connection.Flush();
                        screenShot.Dispose();
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                    catch(Exception e) {
                        Console.WriteLine("Error: " + e.Message);
                        break;
                    }
                }
            }
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if(e.Reason == SessionSwitchReason.ConsoleConnect)
                shouldEnd = true;
        }

        public override void OnDataRecieved(Packet packet)
        {
            switch (packet.Payload)
            {
                case OverridePorgramSettings ops:
                    this.heigth = ops.Heigth;
                    this.width = ops.Width;
                    this.WaitTime = ops.WaitTime;
                    this.quality = ops.Quality;
                    this.takeScreenShot = ops.TakeScreenShot;
                    break;
                default:
                    break;
            }
            base.OnDataRecieved(packet);
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private Bitmap ImageResize(Bitmap img, int Width, int Height)
        {
            var bmpResized = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(bmpResized))
            {
                float scale = Math.Min(Width / (float)img.Width, Height / (float)img.Height);

                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.SmoothingMode = SmoothingMode.None;

                var scaleWidth = (int)(img.Width * scale);
                var scaleHeight = (int)(img.Height * scale);

                g.DrawImage(img, ((int)Width - scaleWidth) / 2, ((int)Height - scaleHeight) / 2, scaleWidth, scaleHeight);
            }

            return bmpResized;
        }

        private Image GetScreenshot()
        {
            int screenLeft = SystemInformation.VirtualScreen.Left;
            int screenTop = SystemInformation.VirtualScreen.Top;
            int screenWidth = SystemInformation.VirtualScreen.Width;
            int screenHeight = SystemInformation.VirtualScreen.Height;

            Bitmap bmp = new Bitmap(screenWidth, screenHeight);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(screenLeft, screenTop, 0, 0, bmp.Size);
            }

            var resized = ImageResize(bmp, width, heigth);
            bmp.Dispose();

            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
            myEncoderParameters.Param[0] = myEncoderParameter;
            MemoryStream ms = new MemoryStream();
            resized.Save(ms, jpgEncoder, myEncoderParameters);
            ms.Flush();

            resized.Dispose();

            return Image.FromStream(ms);
        }
    }
}
