using FluentAssertions;

namespace Dvchevskii.Unit.Tests;

[TestClass]
public class UnitTests
{
    [TestMethod]
    public void Test_DefaultEqualsNew()
    {
        Unit.Default.Should().Be(new Unit());
    }

    [TestMethod]
    public void Test_NewUnitsEqual()
    {
        new Unit().Should().Be(new Unit());
    }

    [TestMethod]
    public void Test_EqOp()
    {
        (new Unit() == new Unit()).Should().BeTrue();
    }

    [TestMethod]
    public void Test_NonEqOp()
    {
        (new Unit() != new Unit()).Should().BeFalse();
    }

    [TestMethod]
    public void Test_GtOp()
    {
        (new Unit() > new Unit()).Should().BeFalse();
    }

    [TestMethod]
    public void Test_GtOrEqOp()
    {
        (new Unit() >= new Unit()).Should().BeTrue();
    }

    [TestMethod]
    public void Test_LtOp()
    {
        (new Unit() < new Unit()).Should().BeFalse();
    }

    [TestMethod]
    public void Test_LtOrEqOp()
    {
        (new Unit() <= new Unit()).Should().BeTrue();
    }

    [TestMethod]
    public void Test_ToString()
    {
        new Unit().ToString().Should().Be("()");
    }

    [TestMethod]
    public void Test_Equals()
    {
        new Unit().Equals(Unit.Default).Should().BeTrue();
        new Unit().Equals(new Unit()).Should().BeTrue();
    }

    [TestMethod]
    public void Test_CompareTo()
    {
        new Unit().CompareTo(new Unit()).Should().Be(0);
    }

    [TestMethod]
    public void Test_EqualHashCode()
    {
        new Unit().GetHashCode().Should().Be(Unit.Default.GetHashCode());
    }
}
