namespace Frends.Slack.SendMessage.Definitions;

using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Data to use for sending Slack message.
/// </summary>
public class Input
{
    /// <summary>
    /// The Slack channel or user ID.
    /// </summary>
    /// <example>C01234567 or U12345678</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ChannelId { get; set; }

    /// <summary>
    /// Message format mode.
    /// </summary>
    /// <example>PlainText</example>
    public MessageMode Mode { get; set; }

    /// <summary>
    /// The plain text of the message to send.
    /// </summary>
    /// <example>Hello from our integration!</example>
    [UIHint(nameof(Mode), "", MessageMode.PlainText)]
    public string Text { get; set; }

    /// <summary>
    /// Slack Block Kit JSON for rich formatting.
    /// </summary>
    /// <example>[{ "type": "section", "text": { "type": "mrkdwn", "text": "*Hello* _world_!" } }]</example>
    [DisplayFormat(DataFormatString = "Text")]
    [UIHint(nameof(Mode), "", MessageMode.Blocks)]
    public JArray Blocks { get; set; }

    /// <summary>
    /// Timestamp of a message to reply in a thread.
    /// </summary>
    /// <example>1234567890.123456</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ThreadTs { get; set; }
}
