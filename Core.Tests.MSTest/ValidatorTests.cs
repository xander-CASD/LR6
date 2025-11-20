using Core;
using Core.COre;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Core.Tests.MSTest;

[TestClass]
public class ValidatorTests
{
    [DataTestMethod]
    [DataRow("a@b.com", true)]
    [DataRow("no_at_symbol", false)]
    [DataRow("", false)]
    [DataRow(null, false)]
    [DataRow("test@test.com", true)]
    [DataRow("invalid.email", false)]
    public void IsValidEmail_Works(string email, bool expected)
        => Assert.AreEqual(expected, Validator.IsValidEmail(email));

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void EnsureAdult_Throws_WhenMinor()
        => Validator.EnsureAdult(17);

    [TestMethod]
    public void EnsureAdult_DoesNotThrow_WhenAdult()
        => Validator.EnsureAdult(18);
}

[TestClass]
public class TariffServiceTests
{
    [TestMethod]
    public void CalcPrice_Basic_Correct()
        => Assert.AreEqual(15m, TariffService.CalcPrice("basic", 3));

    [TestMethod]
    public void CalcPrice_Pro_Correct()
        => Assert.AreEqual(36m, TariffService.CalcPrice("pro", 3));

    [TestMethod]
    public void CalcPrice_Enterprise_Correct()
        => Assert.AreEqual(75m, TariffService.CalcPrice("enterprise", 3));

    [TestMethod]
    public void CalcPrice_InvalidPlan_Throws()
        => Assert.ThrowsException<ArgumentException>(() => TariffService.CalcPrice("unknown", 1));

    [TestMethod]
    public void CalcPrice_InvalidMonths_Throws()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(() => TariffService.CalcPrice("basic", 0));

    [TestMethod]
    public void CalcPrice_OneMonth_Basic()
        => Assert.AreEqual(5m, TariffService.CalcPrice("basic", 1));
}

[TestClass]
public class TariffWithDiscountTests
{
    [TestMethod]
    public void Applies_Discount_From_Provider()
    {
        var mock = new Moq.Mock<IDiscountProvider>();
        mock.Setup(m => m.GetDiscount("pro")).Returns(0.1m);

        var sut = new TariffWithDiscount(mock.Object);
        var total = sut.Calc("pro", 3, 10m);

        Assert.AreEqual(27m, total);
        mock.Verify(m => m.GetDiscount("pro"), Times.Once);
    }

    [TestMethod]
    public void No_Discount_When_Zero()
    {
        var mock = new Moq.Mock<IDiscountProvider>();
        mock.Setup(m => m.GetDiscount("basic")).Returns(0m);

        var sut = new TariffWithDiscount(mock.Object);
        var total = sut.Calc("basic", 2, 10m);

        Assert.AreEqual(20m, total);
    }
}