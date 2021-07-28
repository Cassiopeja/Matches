using System.Threading.Tasks;
using Pexeso.Contracts.Dto;

namespace Pexeso.Hubs
{
    public interface IGameClient
    {
        Task GameCreated(CreatedGameDto createdGame);
        Task PlayerJoinedCreatedGame(string gameId, PlayerDto playerDto);
        Task GameStarted(string gameId);
        Task PlayerLeftCreatedGame(string gameId, PlayerDto playerDto);
        Task CreatedGameIsClosed(string gameId);
        Task GroupPlayerJoinedCreatedGame(PlayerDto playerDto);
        Task GroupPlayerLeftCreatedGame(PlayerDto playerDto);
        Task GroupGameStarted();
        Task GroupPlayerOpenedCard(PlayerDto player, MoveDto move);
        Task GroupPlayerOpenedTwoEqualsCards(PlayerDto player, int[] openedIndexes);
        Task GroupNextPlayer(PlayerDto player);
        Task GroupGameIsFinished(PlayerDto[] orderedByScorePlayers, PlayerDto[] winners);
    }
}