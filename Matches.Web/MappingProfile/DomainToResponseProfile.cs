using AutoMapper;
using Matches.Contracts.Dto;
using Matches.Core;

namespace Matches.MappingProfile
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Player, PlayerDto>();
            CreateMap<CreatedGame, CreatedGameDto>()
                .ForMember(dest => dest.CardTemplateId, opt => opt.MapFrom(src => src.GameParameters.CardTemplate.Id))
                .ForMember(dest => dest.Rows, opt => opt.MapFrom(src => src.GameParameters.Rows))
                .ForMember(dest => dest.Columns, opt => opt.MapFrom(src => src.GameParameters.Columns));
            CreateMap<CardTemplate, CardTemplateDto>();
            CreateMap<Move, MoveDto>()
                .ForMember(dest => dest.CardImageUrl, opt => opt.MapFrom(src => src.Card.FaceImageUrl));
            CreateMap<BoardState, BoardStateDto>();
            CreateMap<GameState, GameStateDto>();
            CreateMap<Game, GameDto>()
                .ForMember(dest=>dest.Players, opt=>opt.MapFrom(src=>src.Players));
        }
    }
}