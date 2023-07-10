using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace VexTrack.Core.Helper;

public static class ApiHelper
{
    private static readonly HttpClient Client = new();

    public static JObject Request(string url)
    {
        var response = Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result;
        if (!response.IsSuccessStatusCode) return new JObject();

        var result = response.Content.ReadAsStringAsync().Result;
        return JObject.Parse(result);
    }

    public static async Task<string> DownloadImage(string url, string destination, string fileName)
    {
        if (string.IsNullOrEmpty(url)) return "";
        
        var uri = new Uri(url);
        
        // Get the file extension
        var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
        var fileExtension = Path.GetExtension(uriWithoutQuery);

        // Create file path and ensure directory exists
        var path = Path.Combine(destination, $"{fileName}{fileExtension}");
        Directory.CreateDirectory(destination);

        // Download the image and write to the file
        var imageBytes = await Client.GetByteArrayAsync(uri);
        await File.WriteAllBytesAsync(path, imageBytes);

        return path;
    }
}