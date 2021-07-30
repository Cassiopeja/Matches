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
            var templateResult =
                _gameManager.FindCardTemplate(parameters.TemplateId);

            if (templateResult.IsFailure)
                throw new HubException(templateResult.Error);

            var gameParameters = new GameParameters(parameters.Rows, parameters.Columns, templateResult.Value);
            var player = new Player(newPlayerDto.Id, newPlayerDto.Name, newPlayerDto.Color);
            var (_, isFailure, game, error) = _gameManager.CreateNewGame(gameParameters, player);
            if (isFailure) throw new HubException(error);

            var createdGameDto = _mapper.Map<CreatedGameDto>(game);
            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
            await Clients.Others.GameCreated(createdGameDto);
            return createdGameDto;
        }

        public async Task JoinCreatedGame(string gameId, NewPlayerDto newPlayerDto)
        {
            var game = FindCreatedGame(gameId);
            var player = new Player(newPlayerDto.Id, newPlayerDto.Name, newPlayerDto.Color);
            if (game.FindPlayer(player.Id).IsSuccess)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
                return;
            }
            
            var (_, isFailure, error) = game.Join(player);
            if (isFailure) throw new HubException(error);

            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
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

        private Game FindStartedGame(string gameId)
        {
            var result = _gameManager.FindStartedGame(gameId);
            if (result.IsFailure) throw new HubException(result.Error);

            return result.Value;
        }

        public async Task LeaveCreatedGame(string gameId, string playerId)
        {
            var game = FindCreatedGame(gameId);
            var result = game.Leave(playerId);
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

        public async Task StartGame(string gameId, string playerId)
        {
            var result = _gameManager.StartGame(gameId, playerId);
            if (result.IsFailure) throw new HubException(result.Error);
            await Clients.OthersInGroup(gameId).GroupGameStarted();
            await Clients.Others.GameStarted(gameId);
        }

        public async Task PlayerOpenedCard(string gameId, PlayerDto player, int cardIndex)
        {
            var game = FindStartedGame(gameId);
            var result = game.OpenCard(player.Id, cardIndex);
            if (result.IsFailure) throw new HubException(result.Error);
            var move = game.GameState == GameState.DoneFirstMove ? game.FirstMove : game.SecondMove;
            var moveDto = _mapper.Map<MoveDto>(move);
            await Clients.Group(gameId).GroupPlayerOpenedCard(player, moveDto);
            switch (game.GameState)
            {
                case GameState.DoneFirstMove:
                    return;
                case GameState.OpenedTwoEqualCards:
                    await Clients.Group(gameId)
                        .GroupPlayerOpenedTwoEqualsCards(player, new[] {game.FirstMove.Index, game.SecondMove.Index});
                    break;
            }

            if (game.IsGameFinished())
            {
                var orderedPlayers = game.Players.Select(pl => _mapper.Map<PlayerDto>(pl)).OrderByDescending(pl => pl.Score)
                    .ToArray();
                var winners = game.GetWinners().Select(pl=>_mapper.Map<PlayerDto>(pl)).ToArray();

                await Clients.Group(gameId).GroupGameIsFinished(orderedPlayers, winners);
                _gameManager.FinishGame(gameId);
                return;
            }

            var nextPlayerResult = game.ChooseNextPlayer();
            if (result.IsFailure) throw new HubException(nextPlayerResult.Error);
            var nextPlayerDto = _mapper.Map<PlayerDto>(nextPlayerResult.Value);
            await Clients.Group(gameId).GroupNextPlayer(nextPlayerDto);
        }

        public async Task ConnectToGame(string gameId)
        {
            FindStartedGame(gameId);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }
        
        public async Task ConnectToCreatedGame(string gameId)
        {
            FindCreatedGame(gameId);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }
    }
}