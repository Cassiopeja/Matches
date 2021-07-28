using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;

namespace Pexeso.Core
{
    public class CreatedGame
    {
        private readonly GameParameters _gameParameters;
        private readonly object _locker = new();
        private readonly List<Player> _players;

        public CreatedGame(GameParameters parameters, Player player, DateTimeOffset createdDate)
        {
            Id = Guid.NewGuid().ToString();
            _players = new List<Player> {player};
            _gameParameters = parameters;
            CreatedBy = player.Name;
            CreatedOn = createdDate;
        }

        public string Id { get; }
        public string CreatedBy { get; }
        public DateTimeOffset CreatedOn { get; }
        public IReadOnlyList<Player> Players => _players.AsReadOnly();
        public GameParameters GameParameters => _gameParameters;

        public Result Join(Player player)
        {
            lock (_locker)
            {
                if (_players.Any(pl => pl.Id == player.Id))
                    return Result.Failure("The player already connected to the game");

                _players.Add(player);
            }

            return Result.Success();
        }

        public Result<Player> Leave(string playerId)
        {
            lock (_locker)
            {
                var player = _players.FirstOrDefault(pl => pl.Id == playerId);
                if (player == null)
                {
                    return Result.Failure<Player>("Player is not in the game"); 
                }
                
                _players.Remove(player);
                return Result.Success(player);
            }
        }

        public Game Start()
        {
            lock (_locker)
            {
                
                return new Game(Id, _gameParameters, _players);
            }
        }
    }
}