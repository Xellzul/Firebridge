using Firebridge.Agent;
using Firebridge.Common;
using Firebridge.Common.Models;
using Firebridge.Common.Models.Packets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Serilog.Core;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Image = System.Drawing.Image;

namespace OverridePluginMaui;


public class ImageCapturerAgent : AgentBackgroundService
{

    private readonly ILogger<ImageCapturerAgent> logger;
    private readonly IAgentContext agentContext;

    public ImageCapturerAgent(ILogger<ImageCapturerAgent> logger, IAgentContext agentContext, IHostApplicationLifetime hostApplicationLifetime)
        :base(agentContext, hostApplicationLifetime)
    {
        this.agentContext = agentContext;
        this.logger = logger;
        logger.LogInformation("ImageCapturerAgent - Ctor");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("ImageCapturerAgent - ExecuteAsync");

        await Task.Delay(1000);

        while (true)
        {
            var screenshot = GetScreenshot(1920, 1080, 100);

            MemoryStream ms = new MemoryStream();
            screenshot.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] imgByteArray = ms.ToArray();
            var packet = new PluginDataPacket() { data = StreamSerializer.Serialize(imgByteArray) };
            var a = StreamSerializer.Deserialize<byte[]>(packet.data);

            logger.LogInformation("Sending {@A}", packet);
            await agentContext.Return(packet);
            await Task.Delay(100);
        }

        SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
    }

    private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
    {

    }

    private static ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format)
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

    private static Bitmap ImageResize(Bitmap img, int Width, int Height)
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

    private static Image GetScreenshot(int width, int heigth, int quality)
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

        ImageCodecInfo jpgEncoder = GetEncoder(System.Drawing.Imaging.ImageFormat.Jpeg);
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
