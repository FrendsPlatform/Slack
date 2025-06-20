using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Slack.DeleteMessage.Definitions;

/// <summary>
/// Connection parameters for Slack DeleteMessage task.
/// </summary>
public class Connection
{
    /// <summary>
    /// Slack OAuth token used for authentication.
    /// The token owner must be the original message poster
    /// or have appropriate permissions to delete the message.
    /// Note: User tokens require the user to have authorized the app via OAuth
    /// with appropriate scopes (e.g., "chat:write").
    /// </summary>
    /// <example>xoxb-123456789</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string Token { get; set; }
}
