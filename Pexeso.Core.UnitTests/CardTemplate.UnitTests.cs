using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Pexeso.Core.UnitTests
{
    public class CardTemplateUnitTests
    {
        [Fact]
        public void ShouldThrowExceptionWhenCreatingCardTemplateWithNullFrontImages()
        {
            Action action = () =>
            {
                var unused = new CardTemplate("test", null, "backImage");
            };

            action.Should().Throw<ArgumentNullException>();
        }
        
        [Fact]
        public void ShouldThrowExceptionWhenCreatingCardTemplateWithNullBackImage()
        {
            Action action = () =>
            {
                var unused = new CardTemplate("test", new []{"front1", "front2"}, null);
            };

            action.Should().Throw<ArgumentNullException>();
        }
        
        [Fact]
        public void ShouldThrowExceptionWhenCreatingCardTemplateWithEmptyListOfFrontImages()
        {
            Action action = () =>
            {
                var unused = new CardTemplate("test", new List<string>(), "backImage");
            };

            action.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void ShouldCreateCardTemplateWhenAllParametersIncluded()
        {
            var backImage = "backImage";
            var cardTemplate = new CardTemplate("test", new []{"front1", "front2", "front3"}, backImage);

            cardTemplate.TotalCount.Should().Be(3);
            cardTemplate.BackCardImageUrl.Should().Be(backImage);
        }
    }
}