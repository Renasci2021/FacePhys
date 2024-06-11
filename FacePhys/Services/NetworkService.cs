using System.Net.Http.Headers;
using System.Text.Json;
using Xamarin.Google.ErrorProne.Annotations;
namespace FacePhys.Services;

public class NetworkService
{
    private readonly string uploadUrl = "http://183.173.187.18:8000/uploader/";

    private readonly HttpClient client = new();

    public async void StartImageUpload()
    {
        var response = await client.GetAsync(uploadUrl + "start_image_transfer");
        response.EnsureSuccessStatusCode();
    }

    public async void UploadImage(byte[] image,int uploadCount)
    {
        var content = new MultipartFormDataContent
        {
            { new ByteArrayContent(image), "file", "image.png" }
        };

        var imageContent = new ByteArrayContent(image);
        imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(imageContent, "image", "image.png");

        content.Add(new StringContent(uploadCount.ToString()), "index");
        var response = await client.PostAsync(uploadUrl + "upload_image", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task<float?> EndImageUpload(float fps)
    {
        try
        {

            var content = new MultipartFormDataContent
            {
                { new StringContent(fps.ToString()), "fps" }
            };
        
            var response = await client.PostAsync(uploadUrl + "end_image_transfer", content);
            response.EnsureSuccessStatusCode();
        // 解析返回的JSON数据
            var responseData = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonDocument.Parse(responseData);

            if (jsonObject.RootElement.TryGetProperty("heartRate", out var heartRateElement) &&
            float.TryParse(heartRateElement.ToString(), out float heartRate))
            {
                return heartRate;
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
        return null; // 如果解析失败，返回null
    }
}