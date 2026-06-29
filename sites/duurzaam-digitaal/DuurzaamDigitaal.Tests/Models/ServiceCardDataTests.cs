using NUnit.Framework;
using DuurzaamDigitaal.Models;

namespace DuurzaamDigitaal.Tests.Models;

[TestFixture]
public class ServiceCardDataTests
{
    [Test]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Arrange & Act
        var serviceCard = new ServiceCardData();

        // Assert
        Assert.That(serviceCard.Icon, Is.Empty);
        Assert.That(serviceCard.Title, Is.Empty);
        Assert.That(serviceCard.Description, Is.Empty);
        Assert.That(serviceCard.Features, Is.Not.Null);
        Assert.That(serviceCard.Features, Is.Empty);
        Assert.That(serviceCard.IsCentered, Is.True);
    }

    [Test]
    public void Features_ShouldAllowAddingNewFeatures()
    {
        // Arrange
        var serviceCard = new ServiceCardData();
        var feature = new ServiceFeature 
        { 
            Text = "Test Feature",
            Icon = "test-icon"
        };

        // Act
        serviceCard.Features.Add(feature);

        // Assert
        Assert.That(serviceCard.Features, Has.Count.EqualTo(1));
        Assert.That(serviceCard.Features[0].Text, Is.EqualTo("Test Feature"));
        Assert.That(serviceCard.Features[0].Icon, Is.EqualTo("test-icon"));
    }

    [Test]
    public void Properties_ShouldAllowSettingAndGettingValues()
    {
        // Arrange
        var serviceCard = new ServiceCardData();
        
        // Act
        serviceCard.Icon = "test-icon";
        serviceCard.Title = "Test Title";
        serviceCard.Description = "Test Description";
        serviceCard.IsCentered = false;

        // Assert
        Assert.That(serviceCard.Icon, Is.EqualTo("test-icon"));
        Assert.That(serviceCard.Title, Is.EqualTo("Test Title"));
        Assert.That(serviceCard.Description, Is.EqualTo("Test Description"));
        Assert.That(serviceCard.IsCentered, Is.False);
    }
}

[TestFixture]
public class ServiceFeatureTests
{
    [Test]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Arrange & Act
        var feature = new ServiceFeature();

        // Assert
        Assert.That(feature.Text, Is.Empty);
        Assert.That(feature.Icon, Is.EqualTo("bi bi-check-circle-fill text-success"));
    }

    [Test]
    public void Properties_ShouldAllowSettingAndGettingValues()
    {
        // Arrange
        var feature = new ServiceFeature();
        
        // Act
        feature.Text = "Custom Feature";
        feature.Icon = "custom-icon";

        // Assert
        Assert.That(feature.Text, Is.EqualTo("Custom Feature"));
        Assert.That(feature.Icon, Is.EqualTo("custom-icon"));
    }
}
