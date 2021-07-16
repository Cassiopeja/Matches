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

        public async Task<CreatedGameDto> CreateGame(GameParametersDto parameters)
        {
            var template =
                _gameManager.CardTemplates.FirstOrDefault(cardTemplate => cardTemplate.Id == parameters.TemplateId);

            if (template == null)
                throw new HubException($"The card template with id {parameters.TemplateId} has not been found");

            var gameParameters = new GameParameters(parameters.Rows, parameters.Columns, template);
            var player = new Player(Context.ConnectionId, parameters.PlayerName);
            var (_, isFailure, game) = _gameManager.CreateNewGame(gameParameters, player);
            if (isFailure) throw new HubException("Can not create game");

            var createdGameDto = _mapper.Map<CreatedGameDto>(game);
            createdGameDto.StartedBy = player.Name;
            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
            await Clients.Others.GameCreated(createdGameDto);
            return createdGameDto;
        }

        public async Task JoinCreatedGame(string gameId, string name)
        {
            var game = FindCreatedGame(gameId);
            var player = new Player(Context.ConnectionId, name);
            var (_, isFailure, error) = game.Join(player);
            if (isFailure) throw new HubException(error);

            await Groups.AddToGroupAsync(player.ConnectionId, gameId);
            var playerDto = _mapper.Map<PlayerDto>(player);
            await Clients.OthersInGroup(gameId).PlayerJoinedCreatedGame(playerDto);
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
            var (_, isFailure, error) = game.Leave(Context.ConnectionId);
            if (isFailure) throw new HubException(error);

            await Clients.OthersInGroup(gameId).PlayerLeftCreatedGame(Context.ConnectionId);
            await Clients.Others.PlayerLeftCreatedGame(gameId, Context.ConnectionId);
        }

        public async Task StartGame(string gameId)
        {
            var result = _gameManager.StartGame(gameId, Context.ConnectionId);
            if (result.IsFailure) throw new HubException(result.Error);

            var gameDto = _mapper.Map<GameDto>(result.Value);
            await Clients.Group(gameId).GameStarted(gameDto);
            await Clients.Others.GameStarted(gameId);
        }
    }
}