using System.Collections.Generic;
using Matches.Core;

namespace Matches.Contracts.Dto
{
    public class CardTemplateDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int TotalCount { get; set; }
        public IReadOnlyList<Card> Cards { get; set; }
        public string BackCardImageUrl { get; set;  }
    }
}