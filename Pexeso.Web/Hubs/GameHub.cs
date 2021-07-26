using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.SignalR;
using Pexeso.Contracts.Dto;
using Pexeso.Core;
using Pexeso.Infrastructure;

namespace Pexeso.Hubs
{
    public class GameHub : Hub<IGameClient>
    {
        private readonly IGameManager _gameManager;
        private readonly IMapper _mapper;

        public GameHub(IGameManager gameManager, IMapper mapper)
        {
            _gameManager = gameManager;
            _mapper = mapper;
        }

        public async Task<CreatedGameDto> CreateGame(GameParametersDto parameters, NewPlayerDto newPlayerDto)
        {
            var template =
                _gameManager.CardTemplates.FirstOrDefault(cardTemplate => cardTemplate.Id == parameters.TemplateId);

            if (template == null)
                throw new HubException($"The card template with id {parameters.TemplateId} has not been found");

            var gameParameters = new GameParameters(parameters.Rows, parameters.Columns, template);
            var player = new Player(newPlayerDto.Id, Context.ConnectionId, newPlayerDto.Name, newPlayerDto.Color);
            var (_, isFailure, game) = _gameManager.CreateNewGame(gameParameters, player);
            if (isFailure) throw new HubException("Can not create game");

            var createdGameDto = _mapper.Map<CreatedGameDto>(game);
            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
            await Clients.Others.GameCreated(createdGameDto);
            return createdGameDto;
        }

        public async Task JoinCreatedGame(string gameId, NewPlayerDto newPlayerDto)
        {
            var game = FindCreatedGame(gameId);
            var player = new Player(newPlayerDto.Id, Context.ConnectionId, newPlayerDto.Name, newPlayerDto.Color);
            var (_, isFailure, error) = game.Join(player);
            if (isFailure) throw new HubException(error);

            await Groups.AddToGroupAsync(player.ConnectionId, gameId);
            var playerDto = _mapper.Map<PlayerDto>(player);
            await Clients.OthersInGroup(gameId).GroupPlayerJoinedCreatedGame(playerDto);
            await Clients.Others.PlayerJoinedCreatedGame(gameId, playerDto);
        }

        private CreatedGame FindCreatedGame(string gameId)
        {
            var result = _gameManager.FindCreatedGame(gameId);
            if (result.IsFailure) throw new HubException(result.Error);

            return result.Value;
        }

        public async Task LeaveCreatedGame(string gameId)
        {
            var game = FindCreatedGame(gameId);
            var result = game.Leave(Context.ConnectionId);
            if (result.IsFailure) throw new HubException(result.Error);
            var playerDto = _mapper.Map<PlayerDto>(result.Value);

            await Clients.OthersInGroup(gameId).GroupPlayerLeftCreatedGame(playerDto);
            await Clients.Others.PlayerLeftCreatedGame(gameId, playerDto);
            var isClosedResult = _gameManager.CloseCreatedGameIfNoPlayers(gameId);
            if (isClosedResult.IsFailure)
            {
                // TODO: Log
            }
            else
            {
                if (isClosedResult.Value) await Clients.All.CreatedGameIsClosed(gameId);
            }
        }

        public async Task<GameDto> StartGame(string gameId)
        {
            var result = _gameManager.StartGame(gameId, Context.ConnectionId);
            if (result.IsFailure) throw new HubException(result.Error);

            var gameDto = _mapper.Map<GameDto>(result.Value);
            await Clients.Group(gameId).GroupGameStarted(gameDto);
            await Clients.Others.GameStarted(gameId);
            return gameDto;
        }
    }
}