using System;
using System.Collections.Generic;
using System.Linq;

namespace Pexeso.Core
{
    public class PreparedGame
    {
        private readonly List<Player> _players;
        private readonly GameParameters _gameParameters;
        private readonly object _locker = new();

        public PreparedGame(GameParameters parameters, Player player)
        {
            Id = new Guid().ToString();
            _players = new List<Player> {player};
            _gameParameters = parameters;
        }

        public string Id { get; }

        public bool ConnectTo(Player player)
        {
            lock (_locker)
            {
                if (_players.Any(pl => pl.ConnectionId == player.ConnectionId)) return false;

                _players.Add(player);
                return true;
            }
        }

        public Game Start()
        {
            lock (_locker)
            {
                return new(Id, _gameParameters, _players);
            }
        }
    }
}