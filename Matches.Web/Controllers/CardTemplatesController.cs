using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CSharpFunctionalExtensions;
using Matches.Contracts.Dto;
using Matches.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Matches.Controllers
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
            var (_, isFailure, cardTemplate) = _gameManager.FindCardTemplate(templateId);
            if (isFailure)
            {
                return NotFound();
            }

            var response = _mapper.Map<CardTemplateDto>(cardTemplate);

            return Ok(response);
        }
    }
}