using System;

namespace Pexeso.Core
{
    public class Player
    {
        public string ConnectionId { get; }
        public string Name { get; }
        public int Score { get; set; } = 0;

        public Player(string connectionId, string name)
        {
            Name = name;
            ConnectionId = connectionId;
        }
    }
}