namespace Pexeso.Core
{
    public class Player
    {
        public string Id { get; }
        public string ConnectionId { get; }
        public string Name { get; }
        
        public string Color { get; }
        public int Score { get; set; } = 0;

        public Player(string id, string connectionId, string name, string color)
        {
            Name = name;
            ConnectionId = connectionId;
            Id = id;
            Color = color;
        }
    }
}