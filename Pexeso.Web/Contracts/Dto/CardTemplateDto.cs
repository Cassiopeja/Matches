using System.Collections.Generic;
using Pexeso.Core;

namespace Pexeso.Contracts.Dto
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