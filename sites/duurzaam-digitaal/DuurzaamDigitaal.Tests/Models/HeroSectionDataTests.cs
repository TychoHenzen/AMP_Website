using NUnit.Framework;
using DuurzaamDigitaal.Models;

namespace DuurzaamDigitaal.Tests.Models;

[TestFixture]
public class HeroSectionDataTests
{
    [Test]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Arrange & Act
        var heroSection = new HeroSectionData();

        // Assert
        Assert.That(heroSection.Title, Is.Empty);
        Assert.That(heroSection.Subtitle, Is.Empty);
        Assert.That(heroSection.Buttons, Is.Not.Null);
        Assert.That(heroSection.Buttons, Is.Empty);
    }

    [Test]
    public void Properties_ShouldAllowSettingAndGettingValues()
    {
        // Arrange
        var heroSection = new HeroSectionData();
        
        // Act
        heroSection.Title = "Test Title";
        heroSection.Subtitle = "Test Subtitle";

        // Assert
        Assert.That(heroSection.Title, Is.EqualTo("Test Title"));
        Assert.That(heroSection.Subtitle, Is.EqualTo("Test Subtitle"));
    }

    [Test]
    public void Buttons_ShouldAllowAddingNewButtons()
    {
        // Arrange
        var heroSection = new HeroSectionData();
        var button = new HeroButton 
        { 
            Text = "Test Button",
            Href = "/test",
            Icon = "test-icon",
            IsOutline = true
        };

        // Act
        heroSection.Buttons.Add(button);

        // Assert
        Assert.That(heroSection.Buttons, Has.Count.EqualTo(1));
        Assert.That(heroSection.Buttons[0].Text, Is.EqualTo("Test Button"));
        Assert.That(heroSection.Buttons[0].Href, Is.EqualTo("/test"));
        Assert.That(heroSection.Buttons[0].Icon, Is.EqualTo("test-icon"));
        Assert.That(heroSection.Buttons[0].IsOutline, Is.True);
    }
}

[TestFixture]
public class HeroButtonTests
{
    [Test]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Arrange & Act
        var button = new HeroButton();

        // Assert
        Assert.That(button.Text, Is.Empty);
        Assert.That(button.Href, Is.Empty);
        Assert.That(button.Icon, Is.Empty);
        Assert.That(button.IsOutline, Is.False);
    }

    [Test]
    public void Properties_ShouldAllowSettingAndGettingValues()
    {
        // Arrange
        var button = new HeroButton();
        
        // Act
        button.Text = "Custom Button";
        button.Href = "/custom";
        button.Icon = "custom-icon";
        button.IsOutline = true;

        // Assert
        Assert.That(button.Text, Is.EqualTo("Custom Button"));
        Assert.That(button.Href, Is.EqualTo("/custom"));
        Assert.That(button.Icon, Is.EqualTo("custom-icon"));
        Assert.That(button.IsOutline, Is.True);
    }
}
