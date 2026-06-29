#region

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

#endregion

namespace Amp.Data.Entities;

public class Invoice : BaseDocument
{
    public Invoice() : base("Invoice", "invoice")
    {
    }

    [Required]
    [JsonProperty("invoiceNumber")]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    [JsonProperty("customerName")]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [JsonProperty("customerEmail")]
    [EmailAddress]
    public string CustomerEmail { get; set; } = string.Empty;

    [JsonProperty("customerPhone")]
    public string? CustomerPhone { get; set; }

    [JsonProperty("customerAddress")]
    public string? CustomerAddress { get; set; }

    [Required]
    [JsonProperty("items")]
    public List<InvoiceItem> Items { get; set; } = new();

    [Required]
    [JsonProperty("subtotal")]
    public decimal Subtotal { get; set; }

    [Required]
    [JsonProperty("taxRate")]
    public decimal TaxRate { get; set; } = 0.21m; // 21% BTW

    [Required]
    [JsonProperty("taxAmount")]
    public decimal TaxAmount { get; set; }

    [Required]
    [JsonProperty("total")]
    public decimal Total { get; set; }

    [Required]
    [JsonProperty("status")]
    public string Status { get; set; } = "Draft"; // Draft, Sent, Paid, Cancelled, Overdue

    [JsonProperty("dueDate")]
    public DateTime DueDate { get; set; }

    [JsonProperty("paidAt")]
    public DateTime? PaidAt { get; set; }

    [JsonProperty("paymentId")]
    public string? PaymentId { get; set; }

    [JsonProperty("notes")]
    public string? Notes { get; set; }

    [JsonProperty("appointmentId")]
    public string? AppointmentId { get; set; }

    [JsonProperty("refurbishedDeviceId")]
    public string? RefurbishedDeviceId { get; set; }
}

public class InvoiceItem
{
    [Required]
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [JsonProperty("quantity")]
    public int Quantity { get; set; }

    [Required]
    [JsonProperty("unitPrice")]
    public decimal UnitPrice { get; set; }

    [Required]
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("itemType")]
    public string? ItemType { get; set; } // Service, Product, Part

    [JsonProperty("itemId")]
    public string? ItemId { get; set; }
}