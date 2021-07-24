namespace Pexeso.Core
{
    public class Move
    {
        public Card Card { get; }
        public int Index { get; }

        public Move(Card card, int index)
        {
            Card = card;
            Index = index;
        }
    }
}