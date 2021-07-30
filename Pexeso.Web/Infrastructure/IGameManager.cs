using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Pexeso.Core;

namespace Pexeso.Infrastructure
{
    public interface IGameManager
    {
        IReadOnlyList<CreatedGame> CreatedGames { get; }
        IReadOnlyList<Game> StartedGames { get; }
        IReadOnlyList<CardTemplate> CardTemplates { get; }
        Result<CreatedGame> CreateNewGame(GameParameters gameParameters, Player player);
        Result<Game> StartGame(string gameId, string playerConnectionId);
        Result FinishGame(string gameId);
        Result<CreatedGame> FindCreatedGame(string gameId);
        Result<Game> FindStartedGame(string gameId);

        Result<CardTemplate> FindCardTemplate(string cardTemplateId);

        Result AddCardTemplate(CardTemplate cardTemplate);
        Result<bool> CloseCreatedGameIfNoPlayers(string gameId);
    }
}