using FacePhys.Services;
using FacePhys.Utils;
using SkiaSharp;
using UltraFaceDotNet;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.Maui.Controls;



namespace FacePhys.Managers;

public class CameraWorkflowManager
{
    
    private CameraService _cameraService;
    private DetectService _detectService;

    private bool cameraReady = false;

    private SKBitmap? skBitmap;

    public bool DetectAtNextFrame { get; set; } = false;

    public event Action<SKBitmap?>? BitmapUpdated;
    public event Action<string>? LogUpdated;

    private FaceInfo faceInfo;

    // 上传图片计数
    private int uploadCount = 0;
    private int uploadMaxCount = 30;
    // 上传图片的地址
    private string uploadUrl = "http://183.172.39.13:8000/uploader/";
    private HttpClient client = new HttpClient();
    public CameraWorkflowManager(CameraService cameraService, DetectService detectService)
    {
        _cameraService = cameraService;
        _detectService = detectService;
        _cameraService.FrameCaptured += OnFrameCaptured;
    }

    /// <summary>
    /// 打开摄像头，实时显示摄像头画面
    /// </summary>
    public async void OpenCamera()
    {
        var result = await _cameraService.CheckCameraPermissionAsync();
        if (result)
        {
            cameraReady = true;
            _cameraService.StartCamera();
            LogUpdated?.Invoke("Camera started.");
        }
        else
        {
            LogUpdated?.Invoke("Camera permission is required to take a photo.");
        }
    }

    private async void OnFrameCaptured(object? sender, byte[] imageData)
    {
        if (!cameraReady)
        {
            return;
        }

        // ! 旋转角度根据实际情况调整
        skBitmap = PrepareBitmap(imageData, -90);
        BitmapUpdated?.Invoke(skBitmap);

        
        if (DetectAtNextFrame)
        {
            DetectAtNextFrame = false;
            faceInfo = await Task.Run(() => DetectFace(skBitmap));

        }
        if (faceInfo != null)
        {
            if(uploadCount == 0)
            {
                // 打印进入
                //Application.Current.MainPage.DisplayAlert("Success", "即将开始传输", "OK");
                // 开始上传
                StartImageTransfer(this, EventArgs.Empty);
            }
            if(uploadCount < uploadMaxCount)
            {
                var croppedBitmap = skBitmap.CropBitmap(faceInfo);
                var resizedBitmap = croppedBitmap.ResizeToSize(8);

                // 逐帧上传
                uploadCount++;
                UploadImage(this, EventArgs.Empty,resizedBitmap);
            }
            else
            {
                // 结束上传
                uploadCount = 0;
                faceInfo = null;
                EndImageTransfer(this, EventArgs.Empty);
            }
        }
    }

    private SKBitmap PrepareBitmap(byte[] imageData, int rotation = 0)
    {
        return imageData.DecodeImage().CropBitmapToSquare().RotateBitmap(rotation);
    }

    private FaceInfo? DetectFace(SKBitmap skBitmap)
    {
        using var skData = skBitmap.Encode(SKEncodedImageFormat.Png, 100);
        var detectResult = _detectService.Detect(skData.ToArray());
        if (detectResult != null && detectResult.Boxes.Count > 0)
        {
            LogUpdated?.Invoke($"{detectResult.Boxes.Count} faces detected.");
            return detectResult.Boxes.First();
        }
        else
        {
            LogUpdated?.Invoke($"No face detected.");
            return null;
        }
    }

    

    private async void StartImageTransfer(object sender, EventArgs e)
    {
        try
        {
            // var response = await client.GetAsync("http://10.0.2.2:8000/uploader/start_image_transfer");
            // response.EnsureSuccessStatusCode();
            // Application.Current.MainPage.DisplayAlert("Success", "传输开始成功！", "OK");
            var response = await client.GetAsync(uploadUrl+"start_image_transfer");
            response.EnsureSuccessStatusCode();
            Device.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage.DisplayAlert("Success", "传输开始成功！", "OK");
            });

        }
        catch (Exception ex)
        {
            
           //Application.Current.MainPage.DisplayAlert("Error", "传输开始失败！" + ex.ToString(), "OK");
            //Console.WriteLine(ex.ToString());
            Device.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage.DisplayAlert("Error", "传输开始失败！" + ex.ToString(), "OK");
            });

        }
    }

    private async void UploadImage(object sender, EventArgs e, SKBitmap result)
    {
        var content = new MultipartFormDataContent();
        
        // 将 SKBitmap 转换为 Stream
        using (var stream = new MemoryStream())
        {
            result.Encode(stream, SKEncodedImageFormat.Png, 100);
            stream.Seek(0, SeekOrigin.Begin);  // 重置流的位置到开头，确保图片数据完整

            var imageContent = new StreamContent(stream);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            content.Add(imageContent, "image", result.GetHashCode().ToString());
        }

        content.Add(new StringContent(uploadCount.ToString()), "index");

        try
        {
            // var response = await client.PostAsync( "http://10.0.2.2:8000/uploader/upload_image", content);
            // response.EnsureSuccessStatusCode();
            // // await DisplayAlert("Success", "图片上传成功！", "OK");
            // Application.Current.MainPage.DisplayAlert("Success", "图片上传成功！"+uploadCount.ToString(), "OK");
            var response = await client.PostAsync(uploadUrl+"upload_image", content);
            response.EnsureSuccessStatusCode();
            Device.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage.DisplayAlert("Success", "图片上传成功！" + uploadCount.ToString(), "OK");
            });
        }
        catch (Exception ex)
        {
            // await DisplayAlert("Error", "图片上传失败：" + ex.Message, "OK");
            //Application.Current.MainPage.DisplayAlert("Error", "图片上传失败！" + ex.Message, "OK");
            Device.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage.DisplayAlert("Error", "图片上传失败！" + ex.Message, "OK");
            });
        }
    }


	private async void EndImageTransfer(object sender, EventArgs e)
	{
		try
		{
			// var response = await client.GetAsync("http://10.0.2.2:8000/uploader/end_image_transfer");
			// response.EnsureSuccessStatusCode();
			// // await DisplayAlert("Success", response.Content.ReadAsStringAsync().Result,"OK");
            // Application.Current.MainPage.DisplayAlert("Success", "传输结束成功！", "OK");
            var response = await client.GetAsync(uploadUrl+"end_image_transfer");
            response.EnsureSuccessStatusCode();
            Device.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage.DisplayAlert("Success", "传输结束成功！", "OK");
            });
		}
		catch (Exception ex)
		{
			// await DisplayAlert("Error", "传输结束失败：" + ex.Message, "OK");
            //Application.Current.MainPage.DisplayAlert("Error", "传输结束失败！" + ex.Message, "OK");
            Device.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage.DisplayAlert("Error", "传输结束失败！" + ex.Message, "OK");
            });
		}
	}


}
