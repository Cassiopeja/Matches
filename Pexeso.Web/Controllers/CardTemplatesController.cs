using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Pexeso.Contracts.Dto;
using Pexeso.Infrastructure;

namespace Pexeso.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardTemplatesController : ControllerBase
    {
        private readonly IGameManager _gameManager;
        private readonly IMapper _mapper;

        public CardTemplatesController(IGameManager gameManager, IMapper mapper)
        {
            _gameManager = gameManager;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CardTemplateDto>> GetAll()
        {
            return Ok(_gameManager.CardTemplates.Select(template => _mapper.Map<CardTemplateDto>(template)).ToList());
        }
    }
}