using FluentAssertions;

namespace Dvchevskii.Unit.Tests;

[TestClass]
public class ComparisonTests
{
    [TestMethod]
    public void GetHashCode_Returns_Constant()
    {
        const int HASHCODE_CONSTANT = 804741551;

        Unit.Default.GetHashCode().Should().Be(HASHCODE_CONSTANT);
        new Unit().GetHashCode().Should().Be(HASHCODE_CONSTANT);
    }

    [TestMethod]
    public void Equals_Unit_Returns_True()
    {
        Unit.Default.Equals(Unit.Default).Should().BeTrue();
        Unit.Default.Equals(new Unit()).Should().BeTrue();
    }

    [TestMethod]
    public void Equals_Unit_Boxed_Returns_True()
    {
        object unitBoxed = new Unit();

        Unit.Default.Equals(unitBoxed).Should().BeTrue();
        new Unit().Equals(unitBoxed).Should().BeTrue();
    }

    [TestMethod]
    public void Equals_NonUnit_Returns_False()
    {
        Unit.Default.Equals(0).Should().BeFalse();
        Unit.Default.Equals(null).Should().BeFalse();
        Unit.Default.Equals(string.Empty).Should().BeFalse();
        Unit.Default.Equals(false).Should().BeFalse();
    }

    [TestMethod]
    public void CompareTo_Returns_Zero()
    {
        Unit.Default.CompareTo(Unit.Default).Should().Be(0);
        Unit.Default.CompareTo(new Unit()).Should().Be(0);
        new Unit().CompareTo(new Unit()).Should().Be(0);
    }
}
