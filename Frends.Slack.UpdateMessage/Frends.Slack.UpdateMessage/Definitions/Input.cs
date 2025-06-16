namespace Frends.Slack.UpdateMessage.Definitions;

using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Data to use for updating Slack message.
/// </summary>
public class Input
{
    /// <summary>
    /// Channel ID where the message exists
    /// </summary>
    /// <example>C12345678</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ChannelId { get; set; }

    /// <summary>
    /// Timestamp of the message to update
    /// </summary>
    /// <example>1234567890.123456</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string MessageTs { get; set; }

    /// <summary>
    /// Message format mode
    /// </summary>
    /// <example>PlainText</example>
    public MessageMode Mode { get; set; }

    /// <summary>
    /// The plain text content (when Mode = PlainText)
    /// </summary>
    /// <example>Updated message text</example>
    [UIHint(nameof(Mode), "", MessageMode.PlainText)]
    public string Text { get; set; }

    /// <summary>
    /// Block Kit JSON (when Mode = Blocks)
    /// </summary>
    /// <example>[{ "type": "section", "text": { "type": "mrkdwn", "text": "*Updated* content" } }]</example>
    [DisplayFormat(DataFormatString = "Text")]
    [UIHint(nameof(Mode), "", MessageMode.Blocks)]
    public JArray Blocks { get; set; }
}
