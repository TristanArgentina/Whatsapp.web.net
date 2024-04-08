using System.Drawing;
using ZXing;

namespace Whatsapp.web.net.example;

public class QRHelper
{
    public static Bitmap Generate(string code)
    {
        var barcodeWriter = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new ZXing.Common.EncodingOptions
            {
                Width = 100,
                Height = 100,
                NoPadding = true
            }
        };
        var pixelData = barcodeWriter.Write(code);


        var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
        var memoryStream = new MemoryStream();
        var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
        try
        {
            // we assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image
            System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
        }
        finally
        {
            bitmap.UnlockBits(bitmapData);
        }
        // save to stream as PNG
        bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
        return bitmap;
    }

    public static void ConsoleWrite(Bitmap bitmap)
    {
        for (var h = 0; h < bitmap.Height; h++)
        {
            for (var w = 0; w < bitmap.Width; w++)
            {
                var pixel = bitmap.GetPixel(w, h);
                if (pixel is { A: 255, R: 255, G: 255, B: 255 })
                {
                    Console.Write('\u2588');
                }
                else
                {
                    Console.Write(' ');
                }
            }
            Console.WriteLine();
        }
    }
}