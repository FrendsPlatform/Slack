using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frends.Slack.DeleteMessage.Definitions;
using Frends.Slack.DeleteMessage.Helpers;
using Newtonsoft.Json.Linq;

namespace Frends.Slack.DeleteMessage;

/// <summary>
/// Task class.
/// </summary>
public static class Slack
{
    /// <summary>
    /// Task to delete existing Slack messages.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-Slack-DeleteMessage)
    /// </summary>
    /// <param name="input">Channel and message identification</param>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Additional parameters.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>object { bool Success, string MessageTs, Error Error { string Message, Exception AdditionalInfo } }</returns>
    public static async Task<Result> DeleteMessage(
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

            if (string.IsNullOrWhiteSpace(input.MessageTs))
                throw new ArgumentException("Message timestamp is required", nameof(input.MessageTs));

            var payload = new JObject
            {
                ["channel"] = input.ChannelId,
                ["ts"] = input.MessageTs,
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", connection.Token);

                var response = await client.PostAsync(
                    "https://slack.com/api/chat.delete",
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
                    MessageTs = input.MessageTs,
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