namespace Pexeso.Core
{
    public class Player
    {
        public string Id { get; }
        public string Name { get; }
        public string Color { get; }
        public int Score { get; set; } = 0;

        public Player(string id, string name, string color)
        {
            Name = name;
            Id = id;
            Color = color;
        }
    }
}