using System;
using FluentAssertions;
using Xunit;

namespace Matches.Core.UnitTests
{
    public class GameParametersUnitTests
    {
        
        [Fact]
        public void ShouldThrowExceptionWhenCreatingGameParametersWhenRowsNumberOutOfRange()
        {
            Action action = () => new GameParameters(-9, 5, Utils.CreateValidCardTemplate(5));

            action.Should().Throw<ArgumentOutOfRangeException>();
        }
        
        [Fact]
        public void ShouldThrowExceptionWhenCreatingGameParametersWhenColumnsNumberOutOfRange()
        {
            Action action = () => new GameParameters(2, -5, Utils.CreateValidCardTemplate(5));

            action.Should().Throw<ArgumentOutOfRangeException>();
        }
        
        [Fact]
        public void ShouldThrowExceptionWhenCreatingGameParametersWhenTotalNumberCardsInBoardAreNotEven()
        {
            Action action = () => new GameParameters(3, 3, Utils.CreateValidCardTemplate(5));

            action.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void ShouldThrowExceptionWhenCreatingGameParametersWhenThereAreNotEnoughCardsInCardTemplate()
        {
            Action action = () => new GameParameters(3, 4, Utils.CreateValidCardTemplate(5));

            action.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void ShouldCreateGameParametersWhenAllParametersIncluded()
        {
            var rows = 2;
            var columns = 4;
            var template = Utils.CreateValidCardTemplate(5);
            
            var board = new GameParameters(rows, columns, template);

            board.Rows.Should().Be(rows);
            board.Columns.Should().Be(columns);
        }
    }
}