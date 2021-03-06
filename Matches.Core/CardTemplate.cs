using System;
using System.Collections.Generic;
using System.Linq;

namespace Matches.Core
{
    public class CardTemplate
    {
        public CardTemplate(string name, IEnumerable<string> frontCardImages, string backCardImage)
        {
            Id = Guid.NewGuid().ToString();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            if (frontCardImages == null) throw new ArgumentNullException(nameof(frontCardImages));

            BackCardImageUrl = backCardImage ?? throw new ArgumentNullException(nameof(backCardImage));
            Cards = frontCardImages.Select((image, index) => new Card(index + 1, image)).ToList();
            if (Cards.Count == 0) throw new ArgumentException(nameof(frontCardImages));
        }

        public string Id { get; }
        public string Name { get; }
        public int TotalCount => Cards.Count;
        public IReadOnlyList<Card> Cards { get; }

        public string BackCardImageUrl { get; }
    }
}