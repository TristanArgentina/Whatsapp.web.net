using System.Drawing;
using ZXing;

namespace Whatsapp.web.net.example;

public class GenerateQR
{
    public async Task<Stream> Generate(string code)
    {
        var barcodeWriter = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new ZXing.Common.EncodingOptions
            {
                Width = 300,
                Height = 300
            }
        };
        var pixelData = barcodeWriter.Write(code);


        using var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
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
        return memoryStream;
    }
}