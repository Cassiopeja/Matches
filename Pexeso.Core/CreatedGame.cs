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

        public CreatedGame(GameParameters parameters, Player player)
        {
            Id = Guid.NewGuid().ToString();
            _players = new List<Player> {player};
            _gameParameters = parameters;
        }

        public string Id { get; }
        public IReadOnlyList<Player> Players => _players.AsReadOnly();
        public GameParameters GameParameters => _gameParameters;

        public Result Join(Player player)
        {
            lock (_locker)
            {
                if (_players.Any(pl => pl.ConnectionId == player.ConnectionId))
                    return Result.Failure("The player already connected to the game");

                _players.Add(player);
            }

            return Result.Success();
        }

        public Result Leave(string playerId)
        {
            lock (_locker)
            {
                var player = _players.FirstOrDefault(pl => pl.ConnectionId == playerId);
                if (player == null)
                {
                    return Result.Failure("Player does not in the game"); 
                }
                
                _players.Remove(player);
            }

            return Result.Success();
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