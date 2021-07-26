using System.Collections.Generic;
using System.Linq;

namespace Pexeso.Core
{
    public class PlayersQueue
    {
        private readonly List<Player> _players;
        private readonly Queue<Player> _playersQueue;

        public PlayersQueue(IEnumerable<Player> players)
        {
            _players = players.ToList();
            _playersQueue = new Queue<Player>();
            _players.ForEach(item => _playersQueue.Enqueue(item));
            CurrentPlayer = _playersQueue.Dequeue();
        }

        public IReadOnlyList<Player> Players => _players;
        public Player CurrentPlayer { get; private set; }

        public void NextPlayer()
        {
            _playersQueue.Enqueue(CurrentPlayer);
            CurrentPlayer = _playersQueue.Dequeue();
        }

        public bool IsPlayerTurn(string playerId)
        {
            return CurrentPlayer.Id == playerId;
        }

        public void IncrementCurrentPlayerScore()
        {
            CurrentPlayer.Score++;
        }
    }
}