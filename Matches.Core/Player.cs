namespace Matches.Core
{
    public class Player
    {
        public string Id { get; }
        public string Name { get; }
        public string Color { get; }
        public int Score { get; set; }

        public Player(string id, string name, string color)
        {
            Name = name;
            Id = id;
            Color = color;
        }
    }
}