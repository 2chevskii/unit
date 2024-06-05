using FluentAssertions;

// ReSharper disable EqualExpressionComparison

namespace Dvchevskii.Unit.Tests;

[TestClass]
public class OperatorTests
{
    [TestMethod]
    public void OpEqual_Unit_Returns_True()
    {
        (Unit.Default == Unit.Default).Should().BeTrue();
        (new Unit() == Unit.Default).Should().BeTrue();
        (new Unit() == new Unit()).Should().BeTrue();
    }

    [TestMethod]
    public void OpEqual_Unit_Boxed_Returns_True()
    {
        object unitBoxed = Unit.Default;

        (unitBoxed == Unit.Default).Should().BeTrue();
        (unitBoxed == new Unit()).Should().BeTrue();
    }

    [TestMethod]
    public void OpNotEqual_Unit_Returns_False()
    {
        (Unit.Default != Unit.Default).Should().BeFalse();
        (new Unit() != Unit.Default).Should().BeFalse();
        (new Unit() != new Unit()).Should().BeFalse();
    }

    [TestMethod]
    public void OpNotEqual_Unit_Boxed_Returns_False()
    {
        object unitBoxed = Unit.Default;

        (unitBoxed != Unit.Default).Should().BeFalse();
        (unitBoxed != new Unit()).Should().BeFalse();
    }

    [TestMethod]
    public void OpGt_OpLt_Returns_False()
    {
        (Unit.Default > new Unit()).Should().BeFalse();
        (Unit.Default < new Unit()).Should().BeFalse();
    }

    [TestMethod]
    public void OpGte_OpLte_Returns_True()
    {
        (Unit.Default >= new Unit()).Should().BeTrue();
        (Unit.Default <= new Unit()).Should().BeTrue();
    }
}
