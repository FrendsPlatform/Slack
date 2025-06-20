using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Slack.SendMessage.Definitions;

/// <summary>
/// Additional parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Whether to expand URLs in the message into rich previews (e.g. YouTube, websites).
    /// Note: Slack may unfurl a link only once per channel to reduce duplication.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool UnfurlLinks { get; set; } = true;

    /// <summary>
    /// Whether to show media previews (e.g. images, video thumbnails) for links in the message.
    /// Note: Media previews may appear only on first post of the same link.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool UnfurlMedia { get; set; } = true;

    /// <summary>
    /// True: Throw an exception.
    /// False: Error will be added to the Result.Error.AdditionalInformation list instead of stopping the Task.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Message what will be used when error occurs.
    /// </summary>
    /// <example>Task failed during execution</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("Failed to send message to Slack")]
    public string ErrorMessageOnFailure { get; set; } = "Failed to send message to Slack";
}
