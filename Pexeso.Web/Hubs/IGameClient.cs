using System.Threading.Tasks;
using Pexeso.Contracts.Dto;

namespace Pexeso.Hubs
{
    public interface IGameClient
    {
        Task GameCreated(CreatedGameDto createdGame);

        Task PlayerJoinedCreatedGame(string gameId, PlayerDto playerDto);
        Task PlayerJoinedCreatedGame(PlayerDto playerDto);
        Task PlayerLeftCreatedGame(string gameId, string playerId);
        Task PlayerLeftCreatedGame(string playerId);
        Task GameStarted(GameDto game);
        Task GameStarted(string gameId);
    }
}