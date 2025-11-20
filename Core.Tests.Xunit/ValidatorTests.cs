using Core;
using Core.COre;
using Moq;
using Xunit;

namespace Core.Tests.Xunit;

public class ValidatorTests
{
    [Theory]
    [InlineData("a@b.com", true)]
    [InlineData("no_at_symbol", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("test@test.com", true)]
    [InlineData("invalid.email", false)]
    public void IsValidEmail_Works(string email, bool expected)
        => Assert.Equal(expected, Validator.IsValidEmail(email));

    [Fact]
    public void EnsureAdult_Throws_WhenMinor()
        => Assert.Throws<ArgumentException>(() => Validator.EnsureAdult(17));

    [Fact]
    public void EnsureAdult_DoesNotThrow_WhenAdult()
        => Validator.EnsureAdult(18);
}

public class TariffServiceTests
{
    [Fact]
    public void CalcPrice_Basic_Correct()
        => Assert.Equal(15m, TariffService.CalcPrice("basic", 3));

    [Fact]
    public void CalcPrice_Pro_Correct()
        => Assert.Equal(36m, TariffService.CalcPrice("pro", 3));

    [Fact]
    public void CalcPrice_Enterprise_Correct()
        => Assert.Equal(75m, TariffService.CalcPrice("enterprise", 3));

    [Fact]
    public void CalcPrice_InvalidPlan_Throws()
        => Assert.Throws<ArgumentException>(() => TariffService.CalcPrice("unknown", 1));

    [Fact]
    public void CalcPrice_InvalidMonths_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => TariffService.CalcPrice("basic", 0));

    [Fact]
    public void CalcPrice_OneMonth_Basic()
        => Assert.Equal(5m, TariffService.CalcPrice("basic", 1));
}

public class TariffWithDiscountTests
{
    [Fact]
    public void Applies_Discount_From_Provider()
    {
        var mock = new Moq.Mock<IDiscountProvider>();
        mock.Setup(m => m.GetDiscount("pro")).Returns(0.1m);

        var sut = new TariffWithDiscount(mock.Object);
        var total = sut.Calc("pro", 3, 10m);

        Assert.Equal(27m, total);
        mock.Verify(m => m.GetDiscount("pro"), Times.Once);
    }

    [Fact]
    public void No_Discount_When_Zero()
    {
        var mock = new Moq.Mock<IDiscountProvider>();
        mock.Setup(m => m.GetDiscount("basic")).Returns(0m);

        var sut = new TariffWithDiscount(mock.Object);
        var total = sut.Calc("basic", 2, 10m);

        Assert.Equal(20m, total);
    }
}