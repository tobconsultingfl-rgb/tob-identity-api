using Xunit;

namespace TOB.Identity.Domain.Test;

public class HelperMethodsTests
{
    [Theory]
    [InlineData("IL", State.IL)]
    [InlineData("CA", State.CA)]
    [InlineData("NY", State.NY)]
    [InlineData("TX", State.TX)]
    public void GetStateEnumValue_WithValidStateName_ReturnsCorrectState(string stateName, State expectedState)
    {
        // Act
        var result = HelperMethods.GetStateEnumValue(stateName);

        // Assert
        Assert.Equal(expectedState, result);
    }

    [Fact]
    public void GetStateEnumValue_WithInvalidStateName_ReturnsIL()
    {
        // Arrange
        string invalidState = "InvalidState";

        // Act
        var result = HelperMethods.GetStateEnumValue(invalidState);

        // Assert
        Assert.Equal(State.IL, result);
    }

    [Theory]
    [InlineData("yes", true)]
    [InlineData("Yes", true)]
    [InlineData("YES", true)]
    [InlineData(" yes ", true)]
    public void GetBoolFromYesNo_WithYes_ReturnsTrue(string input, bool expected)
    {
        // Act
        var result = HelperMethods.GetBoolFromYesNo(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("no", false)]
    [InlineData("No", false)]
    [InlineData("", false)]
    [InlineData("other", false)]
    [InlineData(null, false)]
    public void GetBoolFromYesNo_WithNonYes_ReturnsFalse(string input, bool expected)
    {
        // Act
        var result = HelperMethods.GetBoolFromYesNo(input);

        // Assert
        Assert.Equal(expected, result);
    }
}
