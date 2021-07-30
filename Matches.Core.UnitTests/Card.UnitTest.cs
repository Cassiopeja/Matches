using FluentAssertions;
using Xunit;

namespace Matches.Core.UnitTests
{
    public class CardUnitTests
    {
        [Fact]
        public void CardsWithSameIdShouldBeEquals()
        {
            var card1 = new Card(1, "card1");
            var card2 = new Card(1, "card2");

            card1.Should().BeEquivalentTo(card2);
        }
        
        [Fact]
        public void CardsWithDifferentIdShouldBeNotEqual()
        {
            var card1 = new Card(1, "card1");
            var card2 = new Card(2, "card2");

            card1.Should().NotBeEquivalentTo(card2);
        }
    }
}