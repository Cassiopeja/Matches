using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Pexeso.Core;

namespace Pexeso.Infrastructure
{
    public class GameManager : IGameManager
    {
        private readonly ConcurrentDictionary<string, CardTemplate> _cardTemplates;
        private readonly ConcurrentDictionary<string, CreatedGame> _createdGames;
        private readonly ConcurrentDictionary<string, Game> _startedGames;

        public GameManager()
        {
            _createdGames = new ConcurrentDictionary<string, CreatedGame>();
            _startedGames = new ConcurrentDictionary<string, Game>();
            _cardTemplates = new ConcurrentDictionary<string, CardTemplate>();
            var frontUrls = Enumerable.Range(1, 32).Select(index => $"front{index}").ToArray();
            var templateName = "test";
            _cardTemplates.TryAdd(templateName, new CardTemplate(templateName, frontUrls, "back"));
        }

        public IReadOnlyList<CreatedGame> CreatedGames => _createdGames.Values.ToList();

        public IReadOnlyList<Game> StartedGames => _startedGames.Values.ToList();

        public IReadOnlyList<CardTemplate> CardTemplates => _cardTemplates.Values.ToList();

        public Result<CreatedGame> CreateNewGame(GameParameters gameParameters, Player player)
        {
            var game = new CreatedGame(gameParameters, player);
            if (!_createdGames.TryAdd(game.Id, game)) Result.Failure("Can not create new game");

            return Result.Success(game);
        }

        public Result<Game> StartGame(string gameId, string playerId)
        {
            var result = FindCreatedGame(gameId);
            if (result.IsFailure) return Result.Failure<Game>(result.Error);

            var player = result.Value.Players.FirstOrDefault(pl => pl.ConnectionId == playerId);
            if (player == null) return Result.Failure<Game>("Player does not have permissions to start game");

            if (_createdGames.TryRemove(gameId, out var createdGame))
            {
                var startedGame = new Game(createdGame.Id, createdGame.GameParameters, createdGame.Players);
                if (_startedGames.TryAdd(startedGame.Id, startedGame)) return Result.Success(startedGame);

                _createdGames.TryAdd(createdGame.Id, createdGame);
            }

            return Result.Failure<Game>("Something happened");
        }

        public Result FinishGame(string gameId)
        {
            if (_startedGames.TryRemove(gameId, out _)) return Result.Success();

            return Result.Failure("Something happened");
        }

        public Result<CreatedGame> FindCreatedGame(string gameId)
        {
            if (_createdGames.TryGetValue(gameId, out var createdGame)) return Result.Success(createdGame);

            return Result.Failure<CreatedGame>($"The game with id {gameId} has not been found");
        }
    }
}