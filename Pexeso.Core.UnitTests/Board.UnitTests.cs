using FluentAssertions;
using Xunit;

namespace Pexeso.Core.UnitTests
{
    public class BoardUnitTests
    {
        [Fact]
        public void ShouldCreateValidBoard()
        {
            var rows = 2;
            var columns = 3;
            var parameters = new GameParameters(rows, columns, Utils.CreateValidCardTemplate(5));
            
            var board = new Board(parameters);

            board.BoardState.Columns.Should().Be(columns);
            board.BoardState.Rows.Should().Be(rows);
            board.IsBoardEmpty().Should().BeFalse();
        }
    }
}