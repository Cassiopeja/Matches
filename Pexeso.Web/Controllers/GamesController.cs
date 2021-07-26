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
    public class GamesController : ControllerBase
    {
        private readonly IGameManager _gameManager;
        private readonly IMapper _mapper;

        public GamesController(IGameManager gameManager, IMapper mapper)
        {
            _gameManager = gameManager;
            _mapper = mapper;
        }
        
        [HttpGet]
        [HttpGet("{gameId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GameDto> GetById(string gameId)
        {
            var gameResult = _gameManager.FindStartedGame(gameId);
            if (gameResult.IsFailure)
            {
                return NotFound();
            }

            var response = _mapper.Map<GameDto>(gameResult.Value);
            return Ok(response);
        }
        
    }
}