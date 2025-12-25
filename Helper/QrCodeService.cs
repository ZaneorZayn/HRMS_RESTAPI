using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp;
using ZXing.SkiaSharp.Rendering;


namespace hrms_api.Services;

public class QrCodeService
{
    
        
    public byte[] GenerateQrCode( double latitude , double longitude )
    {
        string content = $"Hrms-Attendance-Check|LAT:{latitude}|LONG:{longitude}";
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new EncodingOptions
            {
                Height = 400,
                Width = 400 ,
                Margin = 2
            },
            Renderer = new SKBitmapRenderer()
        };

        using SKBitmap bitmap = writer.Write(content);
        using SKImage image = SKImage.FromBitmap(bitmap);
        using SKData data = image.Encode(SKEncodedImageFormat.Png, 200);
        
        return data.ToArray();
    }
}