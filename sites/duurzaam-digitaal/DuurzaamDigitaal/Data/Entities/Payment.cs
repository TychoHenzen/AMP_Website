#region

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

#endregion

namespace DuurzaamDigitaal.Data.Entities;

public class Payment : BaseDocument
{
    public Payment() : base("Payment", "payment")
    {
    }

    [Required]
    [JsonProperty("invoiceId")]
    public string InvoiceId { get; set; } = string.Empty;

    [Required]
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [Required]
    [JsonProperty("paymentMethod")]
    public string PaymentMethod { get; set; } = string.Empty; // Cash, PIN, Bank Transfer, iDEAL

    [Required]
    [JsonProperty("status")]
    public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Refunded

    [JsonProperty("transactionId")]
    public string? TransactionId { get; set; }

    [JsonProperty("paymentReference")]
    public string? PaymentReference { get; set; }

    [JsonProperty("completedAt")]
    public DateTime? CompletedAt { get; set; }

    [JsonProperty("failedAt")]
    public DateTime? FailedAt { get; set; }

    [JsonProperty("refundedAt")]
    public DateTime? RefundedAt { get; set; }

    [JsonProperty("refundReason")]
    public string? RefundReason { get; set; }

    [JsonProperty("notes")]
    public string? Notes { get; set; }

    [JsonProperty("customerName")]
    public string? CustomerName { get; set; }

    [JsonProperty("customerEmail")]
    [EmailAddress]
    public string? CustomerEmail { get; set; }
}