using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Frends.Slack.DeleteMessage.Tests.Helpers
{
    internal class SlackSendMessage
    {
        public static async Task<string> SendMessage(
            string text,
            string channelId,
            string token,
            CancellationToken cancellationToken)
        {
            var payload = new JObject
            {
                ["channel"] = channelId,
                ["text"] = text,
            };
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(
                    "https://slack.com/api/chat.postMessage",
                    new StringContent(payload.ToString(), System.Text.Encoding.UTF8, "application/json"),
                    cancellationToken);

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = JObject.Parse(responseContent);
                return responseJson["ts"]?.Value<string>();
            }
        }
    }
}
