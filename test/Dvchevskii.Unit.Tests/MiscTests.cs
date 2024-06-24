using FluentAssertions;

namespace Dvchevskii.Unit.Tests;

[TestClass]
public class MiscTests
{
    [TestMethod]
    public void ToString_ShouldReturnEmptyParen()
    {
        Unit.Default.ToString().Should().Be("()");
    }
}
