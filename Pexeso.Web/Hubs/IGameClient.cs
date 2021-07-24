using System.Threading.Tasks;
using Pexeso.Contracts.Dto;

namespace Pexeso.Hubs
{
    public interface IGameClient
    {
        Task GameCreated(CreatedGameDto createdGame);

        Task PlayerJoinedCreatedGame(string gameId, PlayerDto playerDto);
        Task GameStarted(string gameId);
        Task PlayerLeftCreatedGame(string gameId, string playerId);
        Task GroupPlayerJoinedCreatedGame(PlayerDto playerDto);
        Task GroupPlayerLeftCreatedGame(string playerId);
        Task GroupGameStarted(GameDto game);
    }
}