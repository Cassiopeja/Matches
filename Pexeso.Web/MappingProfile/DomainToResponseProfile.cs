using System.Collections.Generic;
using AutoMapper;
using Pexeso.Contracts.Dto;
using Pexeso.Core;

namespace Pexeso.MappingProfile
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Player, PlayerDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ConnectionId));

            CreateMap<IReadOnlyList<Player>, List<PlayerDto>>();
            CreateMap<CreatedGame, CreatedGameDto>();
            CreateMap<CardTemplate, CardTemplateDto>();
            CreateMap<Game, GameDto>();
        }
    }
}