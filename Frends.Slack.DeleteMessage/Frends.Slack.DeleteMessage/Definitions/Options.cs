using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Slack.DeleteMessage.Definitions;

/// <summary>
/// Additional parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// True: Throw an exception.
    /// False: Error will be added to the Result.Error.AdditionalInfo list instead of stopping the Task.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Message what will be used when error occurs.
    /// </summary>
    /// <example>Task failed during execution</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("Failed to delete message")]
    public string ErrorMessageOnFailure { get; set; } = "Failed to delete message";
}