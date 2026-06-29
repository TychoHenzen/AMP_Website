#region

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

#endregion

namespace DuurzaamDigitaal.Data.Entities;

public class AdminUser : BaseDocument
{
    public AdminUser() : base("AdminUser", "admin")
    {
    }

    [Required]
    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [JsonProperty("email")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [JsonProperty("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [JsonProperty("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonProperty("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonProperty("role")]
    public string Role { get; set; } = "Admin"; // Admin, SuperAdmin

    [JsonProperty("isActive")]
    public bool IsActive { get; set; } = true;

    [JsonProperty("lastLoginAt")]
    public DateTime? LastLoginAt { get; set; }

    [JsonProperty("permissions")]
    public List<string> Permissions { get; set; } = new();

    [JsonProperty("failedLoginAttempts")]
    public int FailedLoginAttempts { get; set; }

    [JsonProperty("lockedUntil")]
    public DateTime? LockedUntil { get; set; }

    [JsonProperty("passwordResetToken")]
    public string? PasswordResetToken { get; set; }

    [JsonProperty("passwordResetTokenExpiry")]
    public DateTime? PasswordResetTokenExpiry { get; set; }
}