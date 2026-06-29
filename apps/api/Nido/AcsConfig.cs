namespace Amp.Api.Nido;

/// <summary>
/// Azure Communication Services email config. Bound from config section "Acs".
/// All values come from app settings (Acs__ConnectionString etc.) — never the repo.
/// When ConnectionString is empty, email sending is disabled (booking still succeeds).
/// </summary>
public class AcsConfig
{
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>e.g. DoNotReply@&lt;id&gt;.azurecomm.net</summary>
    public string SenderAddress { get; set; } = string.Empty;

    /// <summary>Inbox that receives a notification on every new booking.</summary>
    public string BusinessEmail { get; set; } = string.Empty;
}
