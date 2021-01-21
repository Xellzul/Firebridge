﻿using FireBridgeCore.Networking;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireBridgeCore.Kernel.UserPrograms
{
    [Serializable]
    public class OverrideProgram : UserProcess
    {
        int WaitTime = 1000;
        int quality = 70;
        int width = 720;
        int heigth = 480;
        bool takeScreenShot = true;

        public override void Main()
        {
            this.MessageRecieved += OverrideProgram_MessageRecieved;
            while (this.RemoteConnection.Status == ConnectionStatus.Connected)
            {
                Thread.Sleep(WaitTime);
                if(takeScreenShot)
                {
                    var screenShot = GetScreenshot();
                    Respond(screenShot);
                    screenShot.Dispose();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
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

        private void OverrideProgram_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            switch(e.Message.Payload)
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
        }
    }
}