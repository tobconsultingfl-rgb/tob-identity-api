using Xunit;

namespace TOB.Identity.Domain.Test;

public class ExtensionsTests
{
    [Fact]
    public void ToYesOrNo_WhenTrue_ReturnsYes()
    {
        // Arrange
        bool value = true;

        // Act
        var result = value.ToYesOrNo();

        // Assert
        Assert.Equal("Yes", result);
    }

    [Fact]
    public void ToYesOrNo_WhenFalse_ReturnsNo()
    {
        // Arrange
        bool value = false;

        // Act
        var result = value.ToYesOrNo();

        // Assert
        Assert.Equal("No", result);
    }
}
