using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using Xunit;

namespace Pexeso.Core.UnitTests
{
    public class GameUnitTests
    {
        
        [Fact]
        public void ShouldCreateGame()
        {
            var id = "1212312";
            var players = Utils.CreatePlayers(2);
            var board = Utils.CreatePrimitiveBoardTwoOnThree();

            var game = new Game(id, players, board);

            game.Id.Should().BeEquivalentTo(id);
            game.GameState.Should().BeEquivalentTo(GameState.WaitingForFirstMove);
            var player = game.ChooseNextPlayer();
            player.Value.ConnectionId.Should().BeEquivalentTo(players[0].ConnectionId);
        }

        [Fact]
        public void ShouldPlayWithoutErrors()
        {
            var id = "1212312";
            var players = Utils.CreatePlayers(2);
            var board = Utils.CreatePrimitiveBoardTwoOnThree();
            var player1 = players[0];
            var player2 = players[1];

            var game = new Game(id, players, board);

            var result = game.OpenCard(player1.Id, 0, 0);
            result.Should().BeSuccess();
            game.GameState.Should().Be(GameState.DoneFirstMove);
            
            var nextPlayer = game.ChooseNextPlayer();
            nextPlayer.Should().BeSuccess();
            nextPlayer.Value.Id.Should().BeEquivalentTo(player1.Id);

            result = game.OpenCard(player1.Id, 1, 0);
            result.Should().BeSuccess();
            game.GameState.Should().Be(GameState.OpenedTwoEqualCards);
            game.IsGameFinished().Should().BeFalse();

            nextPlayer = game.ChooseNextPlayer();
            nextPlayer.Should().BeSuccess();
            nextPlayer.Value.Id.Should().BeEquivalentTo(player1.Id);
            
            result = game.OpenCard(player1.Id, 0, 1);
            result.Should().BeSuccess();
            game.GameState.Should().Be(GameState.DoneFirstMove);
            game.IsGameFinished().Should().BeFalse();
            
            nextPlayer = game.ChooseNextPlayer();
            nextPlayer.Should().BeSuccess();
            nextPlayer.Value.Id.Should().BeEquivalentTo(player1.Id);
            
            result = game.OpenCard(player1.Id, 0, 2);
            result.Should().BeSuccess();
            game.GameState.Should().Be(GameState.OpenedTwoNotEqualsCards);
            game.IsGameFinished().Should().BeFalse();
            
            nextPlayer = game.ChooseNextPlayer();
            nextPlayer.Should().BeSuccess();
            nextPlayer.Value.Id.Should().BeEquivalentTo(player2.Id);
            
            result = game.OpenCard(player2.Id, 0, 1);
            result.Should().BeSuccess();
            game.GameState.Should().Be(GameState.DoneFirstMove);
            game.IsGameFinished().Should().BeFalse();
            
            nextPlayer = game.ChooseNextPlayer();
            nextPlayer.Should().BeSuccess();
            nextPlayer.Value.Id.Should().BeEquivalentTo(player2.Id);
            
            result = game.OpenCard(player2.Id, 1, 1);
            result.Should().BeSuccess();
            game.GameState.Should().Be(GameState.OpenedTwoEqualCards);
            game.IsGameFinished().Should().BeFalse();
            
            nextPlayer = game.ChooseNextPlayer();
            nextPlayer.Should().BeSuccess();
            nextPlayer.Value.Id.Should().BeEquivalentTo(player2.Id);
            
            result = game.OpenCard(player2.Id, 0, 2);
            result.Should().BeSuccess();
            game.GameState.Should().Be(GameState.DoneFirstMove);
            game.IsGameFinished().Should().BeFalse();
            
            nextPlayer = game.ChooseNextPlayer();
            nextPlayer.Should().BeSuccess();
            nextPlayer.Value.Id.Should().BeEquivalentTo(player2.Id);
            
            result = game.OpenCard(player2.Id, 1, 2);
            result.Should().BeSuccess();
            game.GameState.Should().Be(GameState.OpenedTwoEqualCards);
            game.IsGameFinished().Should().BeTrue();
        }

        [Fact]
        public void ShouldFailToOpenCardNotInYourTurn()
        {
            var id = "1212312";
            var players = Utils.CreatePlayers(2);
            var board = Utils.CreatePrimitiveBoardTwoOnThree();
            var player1 = players[0];
            var player2 = players[1];

            var game = new Game(id, players, board);
            var nextPlayer = game.ChooseNextPlayer();
            nextPlayer.Should().BeSuccess();
            nextPlayer.Value.Id.Should().BeEquivalentTo(player1.Id);

            var result = game.OpenCard(player2.Id, 0, 0);
            result.Should().BeFailure();
            game.GameState.Should().Be(GameState.WaitingForFirstMove);
        }
    }
}