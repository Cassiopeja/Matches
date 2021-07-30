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
            return !_createdGames.TryAdd(game.Id, game) 
                ? Result.Failure<CreatedGame>("It is not possible to create a new game") 
                : Result.Success(game);
        }

        public Result<Game> StartGame(string gameId, string playerId)
        {
            var (_, isFailure, game, error) = FindCreatedGame(gameId);
            if (isFailure) return Result.Failure<Game>(error);

            var result = game.FindPlayer(playerId);
            if (result.IsFailure) return Result.Failure<Game>("Player does not have permissions to start game");

            if (_createdGames.TryRemove(gameId, out var createdGame))
            {
                var startedGame = new Game(createdGame.Id, createdGame.GameParameters, createdGame.Players);
                if (_startedGames.TryAdd(startedGame.Id, startedGame)) return Result.Success(startedGame);

                _createdGames.TryAdd(createdGame.Id, createdGame);
            }

            return Result.Failure<Game>("Something happened while starting the game");
        }

        public Result FinishGame(string gameId)
        {
            return _startedGames.TryRemove(gameId, out _)
                ? Result.Success()
                : Result.Failure("Something happened while finishing the game");
        }

        public Result<CreatedGame> FindCreatedGame(string gameId)
        {
            return _createdGames.TryGetValue(gameId, out var createdGame)
                ? Result.Success(createdGame)
                : Result.Failure<CreatedGame>($"The created game with id {gameId} has not been found");
        }

        public Result<Game> FindStartedGame(string gameId)
        {
            return _startedGames.TryGetValue(gameId, out var game)
                ? Result.Success(game)
                : Result.Failure<Game>($"The game with id {gameId} has not been found");
        }

        public Result<CardTemplate> FindCardTemplate(string cardTemplateId)
        {
            return _cardTemplates.TryGetValue(cardTemplateId, out var gameTemplate)
                ? Result.Success(gameTemplate)
                : Result.Failure<CardTemplate>($"The card template with id {cardTemplateId} has not been found");
        }

        public Result AddCardTemplate(CardTemplate cardTemplate)
        {
            if (cardTemplate == null) throw new ArgumentNullException(nameof(cardTemplate));

            return _cardTemplates.TryAdd(cardTemplate.Id, cardTemplate)
                ? Result.Success()
                : Result.Failure($"Card template with id {cardTemplate.Id} is already added");
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