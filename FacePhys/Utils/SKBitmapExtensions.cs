using SkiaSharp;

namespace FacePhys.Utils;

public static class SKBitmapExtensions
{
    // 解码图片数据为 SKBitmap
    public static SKBitmap DecodeImage(this byte[] data)
    {
        using var stream = new SKMemoryStream(data);
        return SKBitmap.Decode(stream);
    }

    // 裁剪图片为正方形
    public static SKBitmap CropBitmapToSquare(this SKBitmap origin)
    {
        int size = Math.Min(origin.Width, origin.Height);
        int left = (origin.Width - size) / 2;
        int top = (origin.Height - size) / 2;

        var rect = new SKRectI(left, top, left + size, top + size);
        var cropped = new SKBitmap(size, size);
        origin.ExtractSubset(cropped, rect);

        return cropped;
    }

    // 旋转图片
    public static SKBitmap RotateBitmap(this SKBitmap bitmap, float degrees)
    {
        var matrix = SKMatrix.CreateRotationDegrees(degrees, bitmap.Width / 2, bitmap.Height / 2);

        var rotatedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);
        using (var canvas = new SKCanvas(rotatedBitmap))
        {
            canvas.Clear();
            canvas.SetMatrix(matrix);
            canvas.DrawBitmap(bitmap, 0, 0);
        }

        return rotatedBitmap;
    }
}
