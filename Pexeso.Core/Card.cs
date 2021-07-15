using System;

namespace Pexeso.Core
{
    public class Card : IEquatable<Card>
    {
        public static readonly Card NoCard = new(0, string.Empty);

        public Card(int id, string faceImageUrl)
        {
            Id = id;
            FaceImageUrl = faceImageUrl ?? throw new ArgumentNullException(nameof(faceImageUrl));
        }

        public int Id { get; }
        public string FaceImageUrl { get; }

        public bool Equals(Card other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public static bool operator ==(Card card1, Card card2)
        {
            if (card1 is not null) return card1.Equals(card2);
            return card2 is null;
        }

        public static bool operator !=(Card card1, Card card2)
        {
            return !(card1 == card2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Card) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}