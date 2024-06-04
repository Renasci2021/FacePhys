using SkiaSharp;
using UltraFaceDotNet;

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

    // 裁剪图片
    public static SKBitmap CropBitmap(this SKBitmap origin, FaceInfo faceInfo)
    {
        // 计算面部区域的中心点
        float centerX = (faceInfo.X1 + faceInfo.X2) / 2;
        float centerY = (faceInfo.Y1 + faceInfo.Y2) / 2;

        // 计算面部区域的宽度和高度，选择最大值作为正方形的边长
        int sideLength = (int)Math.Max(faceInfo.X2 - faceInfo.X1, faceInfo.Y2 - faceInfo.Y1);

        // 计算正方形的左上角和右下角坐标
        int startX = (int)(centerX - sideLength / 2);
        int startY = (int)(centerY - sideLength / 2);
        int endX = startX + sideLength;
        int endY = startY + sideLength;

        // 调整坐标确保不会超出原始图像边界
        startX = Math.Max(0, startX);
        startY = Math.Max(0, startY);
        endX = Math.Min(origin.Width, endX);
        endY = Math.Min(origin.Height, endY);

        // 更新边长以匹配可能的边界调整
        sideLength = Math.Min(endX - startX, endY - startY);

        // 创建一个新的SKBitmap来存储裁剪后的图像
        SKBitmap croppedBitmap = new SKBitmap(sideLength, sideLength);

        // 使用SKRect定义裁剪区域
        SKRectI cropRect = new SKRectI(startX, startY, startX + sideLength, startY + sideLength);

        // 裁剪图像
        if (origin.ExtractSubset(croppedBitmap, cropRect))
        {
            return croppedBitmap;
        }
        else
        {
            return origin.CropBitmapToSquare();
        }
    }

    // 压缩图片分辨率
    public static SKBitmap ResizeToSize(this SKBitmap origin, int size)
    {
        var imageInfo = new SKImageInfo(size, size);
        return origin.Resize(imageInfo, SKFilterQuality.Low);
    }
}
