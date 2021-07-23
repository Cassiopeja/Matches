using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pexeso.Contracts.Dto;
using Pexeso.Infrastructure;

namespace Pexeso.Controllers
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
            var game = _gameManager.CreatedGames.FirstOrDefault(t => t.Id == createdGameId);
            if (game == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<CreatedGameDto>(game);
            return Ok(response);
        }
        
    }
}