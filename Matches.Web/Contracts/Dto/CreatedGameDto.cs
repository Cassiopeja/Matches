using System;
using System.Collections.Generic;

namespace Matches.Contracts.Dto
{
    public class CreatedGameDto
    {
        public string Id { get; set; }
        public string CardTemplateId { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public IEnumerable<PlayerDto> Players { get; set; }
    }
}