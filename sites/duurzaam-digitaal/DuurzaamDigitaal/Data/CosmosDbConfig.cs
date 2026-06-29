namespace DuurzaamDigitaal.Data;

public class CosmosDbConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseId { get; set; } = string.Empty;
    public string MessagesContainerId { get; set; } = string.Empty;
    public string AppointmentsContainerId { get; set; } = string.Empty;
    public string TimeSlotsContainerId { get; set; } = string.Empty;
    public string RefurbishedDevicesContainerId { get; set; } = string.Empty;
    public string InvoicesContainerId { get; set; } = string.Empty;
    public string PaymentsContainerId { get; set; } = string.Empty;
    public string AdminUsersContainerId { get; set; } = string.Empty;
}