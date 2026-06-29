#region

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

#endregion

namespace DuurzaamDigitaal.Data.Entities;

public class ContactMessage : BaseDocument
{
    public ContactMessage() : base("ContactMessage", "message")
    {
    }

    [Required]
    [StringLength(100)]
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [JsonProperty("phone")]
    public string? Phone { get; set; }

    [Required]
    [StringLength(200)]
    [JsonProperty("subject")]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("isRead")]
    public bool IsRead { get; set; }

    [JsonProperty("adminNotes")]
    public string? AdminNotes { get; set; }

    [JsonProperty("category")]
    public string? Category { get; set; }
}