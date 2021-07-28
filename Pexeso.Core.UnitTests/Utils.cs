using System.Collections.Generic;

namespace Pexeso.Core.UnitTests
{
    public static class Utils
    {
        public static CardTemplate CreateValidCardTemplate(int length)
        {
            var urls = new List<string>();
            for (int i = 0; i < length; i++)
            {
               urls.Add($"front{i}"); 
            }
            return new("test", urls, "back");
        }

        public static List<Player> CreatePlayers(int num)
        {
            var players = new List<Player>();
            for (int i = 0; i < num; i++)
            {
                var name = $"player{i}";
                players.Add(new Player(i.ToString(), name, name, "red"));
            }

            return players;
        }

        public static Board CreateRandomBoard(int rows, int columns)
        {
            var template = CreateValidCardTemplate(rows * columns / 2);
            return new Board(new GameParameters(rows, columns, template));
        }

        public static Board CreatePrimitiveBoardTwoOnThree()
        {
            var template = CreateValidCardTemplate(3);
            var cards = new Card[2, 3];
            for (var index = 0; index < template.Cards.Count; index++)
            {
                cards[0, index] = template.Cards[index];
                cards[1, index] = template.Cards[index];
            }

            return new Board(cards, template.BackCardImageUrl);
        }
        public static Board CreatePrimitiveBoardTwoOnTwo()
        {
            var template = CreateValidCardTemplate(2);
            var cards = new Card[2, 2];
            for (var index = 0; index < template.Cards.Count; index++)
            {
                cards[0, index] = template.Cards[index];
                cards[1, index] = template.Cards[index];
            }

            return new Board(cards, template.BackCardImageUrl);
        }
    }
}