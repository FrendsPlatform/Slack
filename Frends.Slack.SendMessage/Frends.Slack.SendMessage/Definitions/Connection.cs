using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Slack.Definitions;

/// <summary>
/// Connection parameters for Slack SendMessage task.
/// </summary>
public class Connection
{
    /// <summary>
    /// Slack OAuth token used for authentication.
    /// If the token is a **bot token** (starts with "xoxb"), messages
    /// will be sent as the Slack app's bot user.
    /// If the token is a **user token** (starts with "xoxp"), messages
    /// will be sent as the Slack user who authorized the token.
    /// Note: User tokens require the user to have authorized the app via OAuth
    /// with appropriate scopes (e.g., "chat:write").
    /// </summary>
    /// <example>xoxb-123456789</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string Token { get; set; }
}
