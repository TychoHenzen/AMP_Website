using NUnit.Framework;
using DuurzaamDigitaal.Models;

namespace DuurzaamDigitaal.Tests.Models;

[TestFixture]
public class SidebarDataTests
{
    [Test]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Arrange & Act
        var sidebar = new SidebarData();

        // Assert
        Assert.That(sidebar.Sections, Is.Not.Null);
        Assert.That(sidebar.Sections, Is.Empty);
    }

    [Test]
    public void Sections_ShouldAllowAddingNewSections()
    {
        // Arrange
        var sidebar = new SidebarData();
        var section = new SidebarSection 
        { 
            Title = "Test Section",
            Description = "Test Description"
        };

        // Act
        sidebar.Sections.Add(section);

        // Assert
        Assert.That(sidebar.Sections, Has.Count.EqualTo(1));
        Assert.That(sidebar.Sections[0].Title, Is.EqualTo("Test Section"));
        Assert.That(sidebar.Sections[0].Description, Is.EqualTo("Test Description"));
    }
}

[TestFixture]
public class SidebarSectionTests
{
    [Test]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Arrange & Act
        var section = new SidebarSection();

        // Assert
        Assert.That(section.Title, Is.Empty);
        Assert.That(section.Description, Is.Empty);
        Assert.That(section.PricingItems, Is.Not.Null);
        Assert.That(section.PricingItems, Is.Empty);
        Assert.That(section.ContactItems, Is.Not.Null);
        Assert.That(section.ContactItems, Is.Empty);
        Assert.That(section.Buttons, Is.Not.Null);
        Assert.That(section.Buttons, Is.Empty);
        Assert.That(section.FooterText, Is.Empty);
    }

    [Test]
    public void Properties_ShouldAllowSettingAndGettingValues()
    {
        // Arrange
        var section = new SidebarSection();
        
        // Act
        section.Title = "Custom Title";
        section.Description = "Custom Description";
        section.FooterText = "Custom Footer";

        // Assert
        Assert.That(section.Title, Is.EqualTo("Custom Title"));
        Assert.That(section.Description, Is.EqualTo("Custom Description"));
        Assert.That(section.FooterText, Is.EqualTo("Custom Footer"));
    }
}

[TestFixture]
public class PricingItemTests
{
    [Test]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Arrange & Act
        var item = new PricingItem();

        // Assert
        Assert.That(item.Icon, Is.Empty);
        Assert.That(item.Label, Is.Empty);
        Assert.That(item.Price, Is.Empty);
    }

    [Test]
    public void Properties_ShouldAllowSettingAndGettingValues()
    {
        // Arrange
        var item = new PricingItem();
        
        // Act
        item.Icon = "test-icon";
        item.Label = "Test Label";
        item.Price = "€99.99";

        // Assert
        Assert.That(item.Icon, Is.EqualTo("test-icon"));
        Assert.That(item.Label, Is.EqualTo("Test Label"));
        Assert.That(item.Price, Is.EqualTo("€99.99"));
    }
}

[TestFixture]
public class ContactItemTests
{
    [Test]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Arrange & Act
        var item = new ContactItem();

        // Assert
        Assert.That(item.Icon, Is.Empty);
        Assert.That(item.Label, Is.Empty);
        Assert.That(item.Value, Is.Empty);
        Assert.That(item.Href, Is.Empty);
    }

    [Test]
    public void Properties_ShouldAllowSettingAndGettingValues()
    {
        // Arrange
        var item = new ContactItem();
        
        // Act
        item.Icon = "contact-icon";
        item.Label = "Email";
        item.Value = "test@example.com";
        item.Href = "mailto:test@example.com";

        // Assert
        Assert.That(item.Icon, Is.EqualTo("contact-icon"));
        Assert.That(item.Label, Is.EqualTo("Email"));
        Assert.That(item.Value, Is.EqualTo("test@example.com"));
        Assert.That(item.Href, Is.EqualTo("mailto:test@example.com"));
    }
}
