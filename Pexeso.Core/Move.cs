namespace Pexeso.Core
{
    public class Move
    {
        public Card Card { get; }
        public int Row { get; }
        public int Column { get; }

        public Move(Card card, int row, int column)
        {
            Card = card;
            Row = row;
            Column = column;
        }
    }
}