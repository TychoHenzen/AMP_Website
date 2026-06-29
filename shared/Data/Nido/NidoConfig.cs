namespace Amp.Data.Nido;

/// <summary>
/// Cosmos location for Nido Suave's booking data. Its own database (separate from the repair
/// site's), shared Cosmos account. Bind from config section "Nido"; defaults match the
/// provisioned resources so no app settings are strictly required.
/// </summary>
public class NidoConfig
{
    public string DatabaseId { get; set; } = "nido";
    public string AppointmentsContainerId { get; set; } = "appointments";
}
