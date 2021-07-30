using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Matches.Contracts.Dto;
using Matches.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Matches.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreatedGamesController: ControllerBase
    {
        private readonly IGameManager _gameManager;
        private readonly IMapper _mapper;

        public CreatedGamesController(IGameManager gameManager, IMapper mapper)
        {
            _gameManager = gameManager;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CreatedGameDto>> GetAll()
        {
            return Ok(_gameManager.CreatedGames.Select(game => _mapper.Map<CreatedGameDto>(game)).ToList());
        }
        
        [HttpGet("{createdGameId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CreatedGameDto> GetById(string createdGameId)
        {
            var gameResult = _gameManager.FindCreatedGame(createdGameId);
            if (gameResult.IsFailure)
            {
                return NotFound();
            }

            var response = _mapper.Map<CreatedGameDto>(gameResult.Value);
            return Ok(response);
        }
        
    }
}