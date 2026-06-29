#region

using DuurzaamDigitaal.Models;
using Microsoft.AspNetCore.Components;

#endregion

namespace DuurzaamDigitaal.Components.Pages.Navigation;

public partial class RefurbishedBase : ComponentBase
{
    protected List<RefurbishedDeviceData> AvailableDevices { get; set; } = new()
    {
        new RefurbishedDeviceData
        {
            Id = "basic-office-pc",
            Name = "Basic Office PC",
            Description = "Perfect voor dagelijks computergebruik en kantoorwerk.",
            Price = 200m,
            Category = "Basic",
            Specifications = new List<string>
            {
                "Intel Core i3/i5 processor",
                "8GB RAM",
                "256GB SSD",
                "Windows 11 Pro"
            },
            Features = new List<string>
            {
                "Stil en energiezuinig",
                "Compact formaat",
                "USB 3.0 poorten",
                "WiFi ingebouwd"
            },
            Performance = new SystemPerformanceData
            {
                GraphicsScore = 40,
                ProcessorScore = 45,
                MemoryScore = 40,
                StorageScore = 50
            },
            IsAvailable = true
        },
        new RefurbishedDeviceData
        {
            Id = "creative-workstation",
            Name = "Creative Workstation",
            Description = "Ideaal voor foto- en videobewerking en multitasking.",
            Price = 350m,
            Category = "Mid",
            Specifications = new List<string>
            {
                "Intel Core i5/i7 processor",
                "16GB RAM",
                "512GB SSD",
                "Windows 11 Pro"
            },
            Features = new List<string>
            {
                "Snelle SSD opslag",
                "Veel werkgeheugen",
                "Dual monitor support",
                "Professionele koeling"
            },
            Performance = new SystemPerformanceData
            {
                GraphicsScore = 65,
                ProcessorScore = 70,
                MemoryScore = 75,
                StorageScore = 80
            },
            IsAvailable = true
        },
        new RefurbishedDeviceData
        {
            Id = "gaming-beast",
            Name = "Gaming Beast",
            Description = "Krachtige gaming PC voor de beste game-ervaring.",
            Price = 500m,
            Category = "High",
            Specifications = new List<string>
            {
                "Intel Core i7/i9 processor",
                "32GB RAM",
                "1TB NVMe SSD",
                "Dedicated Graphics"
            },
            Features = new List<string>
            {
                "4K gaming ready",
                "Ray tracing support",
                "RGB verlichting",
                "High-end koeling"
            },
            Performance = new SystemPerformanceData
            {
                GraphicsScore = 85,
                ProcessorScore = 90,
                MemoryScore = 95,
                StorageScore = 90
            },
            IsAvailable = true
        }
    };

    protected HeroSectionData HeroData { get; set; } = new()
    {
        Title = "Refurbished Computers",
        Subtitle = "Duurzame computers en laptops met 6 maanden garantie",
        Buttons = new List<HeroButton>()
    };

    protected SidebarData SidebarData { get; set; } = new()
    {
        Sections = new List<SidebarSection>
        {
            new()
            {
                Title = "Contact",
                Description = "Interesse in een refurbished systeem of heeft u vragen?",
                ContactItems = new List<ContactItem>
                {
                    new()
                    {
                        Icon = "bi bi-telephone",
                        Label = "Telefoon",
                        Value = "[Your Phone]",
                        Href = "tel:[Your Phone]"
                    },
                    new()
                    {
                        Icon = "bi bi-envelope",
                        Label = "Email",
                        Value = "[Your Email]",
                        Href = "mailto:[Your Email]"
                    }
                },
                Buttons = new List<HeroButton>
                {
                    new()
                    {
                        Text = "Afspraak Maken",
                        Href = "/afspraak",
                        Icon = "bi bi-calendar"
                    },
                    new()
                    {
                        Text = "Contact Opnemen",
                        Href = "/contact",
                        Icon = "bi bi-envelope",
                        IsOutline = true
                    }
                }
            }
        }
    };
}