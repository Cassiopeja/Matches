using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Pexeso.Core;
using Pexeso.Services;

namespace Pexeso.Infrastructure
{
    public class GameManager : IGameManager
    {
        private readonly ConcurrentDictionary<string, CardTemplate> _cardTemplates;
        private readonly ConcurrentDictionary<string, CreatedGame> _createdGames;
        private readonly IDateTimeService _dateTimeService;
        private readonly ConcurrentDictionary<string, Game> _startedGames;

        public GameManager(IDateTimeService dateTimeService)
        {
            _dateTimeService = dateTimeService;
            _createdGames = new ConcurrentDictionary<string, CreatedGame>();
            _startedGames = new ConcurrentDictionary<string, Game>();
            _cardTemplates = new ConcurrentDictionary<string, CardTemplate>();
        }

        public IReadOnlyList<CreatedGame> CreatedGames => _createdGames.Values.ToList();

        public IReadOnlyList<Game> StartedGames => _startedGames.Values.ToList();

        public IReadOnlyList<CardTemplate> CardTemplates => _cardTemplates.Values.ToList();

        public Result<CreatedGame> CreateNewGame(GameParameters gameParameters, Player player)
        {
            var game = new CreatedGame(gameParameters, player, _dateTimeService.Now);
            if (!_createdGames.TryAdd(game.Id, game)) Result.Failure("Can not create new game");

            return Result.Success(game);
        }

        public Result<Game> StartGame(string gameId, string playerConnectionId)
        {
            var result = FindCreatedGame(gameId);
            if (result.IsFailure) return Result.Failure<Game>(result.Error);

            var player = result.Value.Players.FirstOrDefault(pl => pl.ConnectionId == playerConnectionId);
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

        public Result<Game> FindStartedGame(string gameId)
        {
            if (_startedGames.TryGetValue(gameId, out var game)) return Result.Success(game);

            return Result.Failure<Game>($"The game with id {gameId} has not been found");
        }

        public Result AddCardTemplate(CardTemplate cardTemplate)
        {
            if (cardTemplate == null) throw new ArgumentNullException(nameof(cardTemplate));
            if (_cardTemplates.ContainsKey(cardTemplate.Name))
                return Result.Failure("Card template with this name is already added");

            if (_cardTemplates.TryAdd(cardTemplate.Name, cardTemplate)) return Result.Success();

            return Result.Failure("Something happened while adding card template");
        }

        public Result<bool> CloseCreatedGameIfNoPlayers(string gameId)
        {
            var (_, isFailure, createdGame, error) = FindCreatedGame(gameId);
            if (isFailure) return Result.Failure<bool>(error);

            if (createdGame.Players.Count > 0) return Result.Success(false);

            return _createdGames.TryRemove(gameId, out createdGame)
                ? Result.Success(true)
                : Result.Failure<bool>("Something happened while closing game");
        }
    }
}