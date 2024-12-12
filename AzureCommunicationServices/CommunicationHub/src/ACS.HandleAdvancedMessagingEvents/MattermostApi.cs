using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ACS.HandleAdvancedMessagingEvents;

public class MattermostApi
{
    public static async Task SendMessageToMattermost(string baseUrl, string token, 
        string channelId, string message)
    {
        // Create a custom HttpClientHandler to bypass SSL validation
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, certificate, chain, sslPolicyErrors) => true
        };

        using var client = new HttpClient(handler);
        client.BaseAddress = new Uri(baseUrl);
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        var postData = new
        {
            channel_id = channelId,
            message = message
        };

        var jsonContent = JsonConvert.SerializeObject(postData);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/v4/posts", content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Message sent successfully!");
        }
        else
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Failed to send message. Status code: {response.StatusCode}, Response: {responseContent}");
        }
    }
}