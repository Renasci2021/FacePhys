using System.Net.Http.Headers;

namespace FacePhys.Services;

public class NetworkService
{
    private readonly string uploadUrl = "http://183.173.184.25:8000/uploader/";

    private readonly HttpClient client = new();

    public async void StartImageUpload()
    {
        var response = await client.GetAsync(uploadUrl + "start_image_transfer");
        response.EnsureSuccessStatusCode();
    }

    public async void UploadImage(byte[] image)
    {
        var content = new MultipartFormDataContent
        {
            { new ByteArrayContent(image), "file", "image.png" }
        };

        var imageContent = new ByteArrayContent(image);
        imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(imageContent, "image", "image.png");

        var response = await client.PostAsync(uploadUrl + "upload_image", content);
        response.EnsureSuccessStatusCode();
    }

    public async void EndImageUpload(float fps)
    {
        var content = new MultipartFormDataContent
        {
            { new StringContent(fps.ToString()), "fps" }
        };

        var response = await client.PostAsync(uploadUrl + "end_image_transfer", content);
        response.EnsureSuccessStatusCode();
    }
}