using Core;
using Core.COre;
using Moq;
using NUnit.Framework;

namespace Core.Tests.NUnit;

[TestFixture]
public class ValidatorTests
{
    [TestCase("a@b.com", true)]
    [TestCase("no_at_symbol", false)]
    [TestCase("", false)]
    [TestCase("test@test.com", true)]
    [TestCase("invalid.email", false)]
    public void IsValidEmail_Works(string email, bool expected)
        => Assert.That(Validator.IsValidEmail(email), Is.EqualTo(expected));

    [Test]
    public void IsValidEmail_Null_ReturnsFalse()
        => Assert.That(Validator.IsValidEmail(null), Is.False);

    [Test]
    public void EnsureAdult_Throws_WhenMinor()
        => Assert.Throws<ArgumentException>(() => Validator.EnsureAdult(17));

    [Test]
    public void EnsureAdult_DoesNotThrow_WhenAdult()
        => Validator.EnsureAdult(18);
}

[TestFixture]
public class TariffServiceTests
{
    [Test]
    public void CalcPrice_Basic_Correct()
        => Assert.That(TariffService.CalcPrice("basic", 3), Is.EqualTo(15m));

    [Test]
    public void CalcPrice_Pro_Correct()
        => Assert.That(TariffService.CalcPrice("pro", 3), Is.EqualTo(36m));

    [Test]
    public void CalcPrice_Enterprise_Correct()
        => Assert.That(TariffService.CalcPrice("enterprise", 3), Is.EqualTo(75m));

    [Test]
    public void CalcPrice_InvalidPlan_Throws()
        => Assert.Throws<ArgumentException>(() => TariffService.CalcPrice("unknown", 1));

    [Test]
    public void CalcPrice_InvalidMonths_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => TariffService.CalcPrice("basic", 0));

    [Test]
    public void CalcPrice_OneMonth_Basic()
        => Assert.That(TariffService.CalcPrice("basic", 1), Is.EqualTo(5m));
}

[TestFixture]
public class TariffWithDiscountTests
{
    [Test]
    public void Applies_Discount_From_Provider()
    {
        var mock = new Moq.Mock<IDiscountProvider>();
        mock.Setup(m => m.GetDiscount("pro")).Returns(0.1m);

        var sut = new TariffWithDiscount(mock.Object);
        var total = sut.Calc("pro", 3, 10m);

        Assert.That(total, Is.EqualTo(27m));
        mock.Verify(m => m.GetDiscount("pro"), Times.Once);
    }

    [Test]
    public void No_Discount_When_Zero()
    {
        var mock = new Moq.Mock<IDiscountProvider>();
        mock.Setup(m => m.GetDiscount("basic")).Returns(0m);

        var sut = new TariffWithDiscount(mock.Object);
        var total = sut.Calc("basic", 2, 10m);

        Assert.That(total, Is.EqualTo(20m));
    }
}