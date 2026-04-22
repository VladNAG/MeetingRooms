using System.Net;
using System.Net.Http.Json;

namespace MeetingRooms.IntegrationTests.Infrastructure;

public static class HttpClientExtensions
{
    public static HttpClient WithEmployee(this HttpClient client, Guid userId)
    {
        client.DefaultRequestHeaders.Remove("X-User-Id");
        client.DefaultRequestHeaders.Remove("X-User-Role");
        client.DefaultRequestHeaders.Add("X-User-Id", userId.ToString());
        client.DefaultRequestHeaders.Add("X-User-Role", "Employee");
        return client;
    }

    public static HttpClient WithAdmin(this HttpClient client, Guid userId)
    {
        client.DefaultRequestHeaders.Remove("X-User-Id");
        client.DefaultRequestHeaders.Remove("X-User-Role");
        client.DefaultRequestHeaders.Add("X-User-Id", userId.ToString());
        client.DefaultRequestHeaders.Add("X-User-Role", "Admin");
        return client;
    }

    public static async Task<ApiResult<T>> PostAsync<T>(
        this HttpClient client, string url, object body)
    {
        var response = await client.PostAsync(url, JsonContent.Create(body));
        var data = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<T>()
            : default;
        return new ApiResult<T>(response.StatusCode, data);
    }

    public static async Task<ApiResult> PostAsync(
        this HttpClient client, string url, object body)
    {
        var response = await client.PostAsync(url, JsonContent.Create(body));
        return new ApiResult(response.StatusCode);
    }

    public static async Task<ApiResult<T>> GetAsync<T>(
        this HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        var data = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<T>()
            : default;
        return new ApiResult<T>(response.StatusCode, data);
    }
}
