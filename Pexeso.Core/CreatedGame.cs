using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;

namespace Pexeso.Core
{
    public class CreatedGame
    {
        private readonly ConcurrentDictionary<string, Player> _players;

        public CreatedGame(GameParameters parameters, Player player, DateTimeOffset createdDate)
        {
            Id = Guid.NewGuid().ToString();
            _players = new ConcurrentDictionary<string, Player>();
            _players.TryAdd(player.Id, player);
            GameParameters = parameters;
            CreatedBy = player.Name;
            CreatedOn = createdDate;
        }

        public string Id { get; }
        public string CreatedBy { get; }
        public DateTimeOffset CreatedOn { get; }
        public IReadOnlyList<Player> Players => _players.Values.ToList();

        public GameParameters GameParameters { get; }

        public Result Join(Player player)
        {
            return _players.TryAdd(player.Id, player) 
                ? Result.Success() 
                : Result.Failure("The player already connected to the game");
        }

        public Result<Player> Leave(string playerId)
        {
            return _players.TryRemove(playerId, out var player)
                ? Result.Success(player)
                : Result.Failure<Player>("Player is not int the game");
        }

        public Game Start()
        {
            return new(Id, GameParameters, Players);
        }

        public Result<Player> FindPlayer(string playerId)
        {
            return _players.TryGetValue(playerId, out var player)
                ? Result.Success(player)
                : Result.Failure<Player>("Player is not found");
        }
    }
}