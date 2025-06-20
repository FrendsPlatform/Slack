namespace Frends.Slack.DeleteMessage.Definitions;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Data to be used for deleting a Slack message.
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
    /// Timestamp of the message to delete.
    /// </summary>
    /// <example>1234567890.123456</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string MessageTs { get; set; }
}
