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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<CardTemplateDto>> GetAll()
        {
            return Ok(_gameManager.CardTemplates.Select(template => _mapper.Map<CardTemplateDto>(template)).ToList());
        }

        [HttpGet("{templateId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CardTemplateDto> GetById(string templateId)
        {
            var template = _gameManager.CardTemplates.FirstOrDefault(t => t.Id == templateId);
            if (template == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<CardTemplateDto>(template);

            return Ok(response);
        }
    }
}