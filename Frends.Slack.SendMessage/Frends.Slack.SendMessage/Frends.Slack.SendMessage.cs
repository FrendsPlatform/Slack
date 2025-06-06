using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frends.Slack.Definitions;
using Frends.Slack.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Frends.Slack.SendMessage;

/// <summary>
/// Task class.
/// </summary>
public static class Slack
{
    /// <summary>
    /// Task to send message to Slack channels or users with support for text, blocks, and link/media previews.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-Slack-SendMessage)
    /// </summary>
    /// <param name="input">Essential parameters.</param>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Additional parameters.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>object { bool Success, string MessageTs, Error Error { string Message, dynamic AdditionalInfo } }</returns>
    public static async Task<Result> SendMessage(
        [PropertyTab] Input input,
        [PropertyTab] Connection connection,
        [PropertyTab] Options options,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(connection.Token))
                throw new ArgumentException("Slack token is required", nameof(connection.Token));

            if (string.IsNullOrWhiteSpace(input.ChannelId))
                throw new ArgumentException("Channel ID is required", nameof(input.ChannelId));

            if (input.Mode == MessageMode.PlainText && string.IsNullOrWhiteSpace(input.Text))
                throw new ArgumentException("Text is required for PlainText mode", nameof(input.Text));

            if (input.Mode == MessageMode.Blocks && (input.Blocks == null || input.Blocks.Count == 0))
                throw new ArgumentException("Blocks are required for Blocks mode", nameof(input.Blocks));

            var payload = new JObject
            {
                ["channel"] = input.ChannelId,
                ["unfurl_links"] = options.UnfurlLinks,
                ["unfurl_media"] = options.UnfurlMedia,
            };

            if (input.Mode == MessageMode.PlainText)
            {
                payload["text"] = input.Text;
            }
            else
            {
                payload["blocks"] = input.Blocks;
            }

            if (!string.IsNullOrWhiteSpace(input.ThreadTs))
            {
                payload["thread_ts"] = input.ThreadTs;
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", connection.Token);

                var response = await client.PostAsync(
                    "https://slack.com/api/chat.postMessage",
                    new StringContent(payload.ToString(), System.Text.Encoding.UTF8, "application/json"),
                    cancellationToken);

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = JObject.Parse(responseContent);

                if (!response.IsSuccessStatusCode || !(responseJson["ok"]?.Value<bool>() ?? false))
                {
                    var error = responseJson["error"]?.Value<string>() ?? "Unknown error";
                    throw new Exception($"Slack API error: {error}");
                }

                return new Result
                {
                    Success = true,
                    MessageTs = responseJson["ts"]?.Value<string>(),
                };
            }
        }
        catch (Exception ex)
        {
            return ErrorHandler.Handle(
                ex,
                options.ThrowErrorOnFailure,
                options.ErrorMessageOnFailure);
        }
    }
}
