#region

using DuurzaamDigitaal.Models;
using Microsoft.AspNetCore.Components;

#endregion

namespace DuurzaamDigitaal.Components.Pages.Navigation;

public class RefurbishedDetailsBase : RefurbishedBase
{
    [Parameter]
    public string Id { get; set; } = string.Empty;

    protected RefurbishedDeviceData? Device { get; set; }
    protected SidebarData DeviceSidebarData { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        // Find the device from available devices
        Device = AvailableDevices.FirstOrDefault(d => d.Id == Id);

        // Set up sidebar data with warranty and condition info
        if (Device != null)
        {
            DeviceSidebarData = new SidebarData
            {
                Sections = new List<SidebarSection>
                {
                    new()
                    {
                        Title = "Garantie & Conditie",
                        Description = $"Conditie: {Device.Condition}\nGarantie: {Device.Warranty}",
                        ContactItems = new List<ContactItem>(),
                        Buttons = new List<HeroButton>
                        {
                            new()
                            {
                                Text = "Contact voor vragen",
                                Href = "/contact",
                                Icon = "bi bi-envelope",
                                IsOutline = true
                            }
                        }
                    },
                    new()
                    {
                        Title = "Direct Bestellen",
                        Description = "Interesse in deze refurbished computer?",
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
                                Text = "Bestellen",
                                Href = "/contact",
                                Icon = "bi bi-cart"
                            }
                        }
                    }
                }
            };
        }
    }
}