#region

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

#endregion

namespace DuurzaamDigitaal.Data.Entities;

public class RefurbishedDevice : BaseDocument
{
    public RefurbishedDevice() : base("RefurbishedDevice", "device")
    {
    }

    [Required]
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [JsonProperty("brand")]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [JsonProperty("model")]
    public string Model { get; set; } = string.Empty;

    [Required]
    [JsonProperty("type")]
    public string DeviceType { get; set; } = string.Empty;

    [Required]
    [JsonProperty("price")]
    public decimal Price { get; set; }

    [Required]
    [JsonProperty("condition")]
    public string Condition { get; set; } = string.Empty;

    [JsonProperty("specifications")]
    public Dictionary<string, string> Specifications { get; set; } = new();

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("imageUrls")]
    public List<string> ImageUrls { get; set; } = new();

    [JsonProperty("status")]
    public string Status { get; set; } = "Available"; // Available, Reserved, Sold

    [JsonProperty("warrantyMonths")]
    public int WarrantyMonths { get; set; }

    [JsonProperty("serialNumber")]
    public string? SerialNumber { get; set; }

    [JsonProperty("purchasePrice")]
    public decimal? PurchasePrice { get; set; }

    [JsonProperty("refurbishmentCost")]
    public decimal? RefurbishmentCost { get; set; }

    [JsonProperty("refurbishmentNotes")]
    public string? RefurbishmentNotes { get; set; }

    [JsonProperty("soldAt")]
    public DateTime? SoldAt { get; set; }

    [JsonProperty("reservedUntil")]
    public DateTime? ReservedUntil { get; set; }
}