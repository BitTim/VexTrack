using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace VexTrack.Core.Helper;

public static class ApiHelper
{
    private static readonly HttpClient Client = new();

    public static JObject Request(string url)
    {
        return new JObject(Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result.Content);
    }
}