using System;
using System.Collections.Generic;

namespace Pexeso.Contracts.Dto
{
    public class CreatedGameDto
    {
        public string Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public IEnumerable<PlayerDto> Players { get; set; }
    }
}