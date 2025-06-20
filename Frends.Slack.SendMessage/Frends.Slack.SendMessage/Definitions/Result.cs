namespace Frends.Slack.SendMessage.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    internal Result(string messageTs = null, bool success = true, Error error = null)
    {
        Success = success;
        MessageTs = messageTs;
        Error = error;
    }

    /// <summary>
    /// Indicates whether the message was sent successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// The timestamp (ID) of the sent message.
    /// </summary>
    /// <example>1234567890.123456</example>
    public string MessageTs { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, object { Exception Exception } AdditionalInfo }</example>
    public Error Error { get; set; }
}
