using System.Collections.Concurrent;
using Pexeso.Core;

namespace Pexeso.Infrastructure
{
    public interface IGameManager
    {
        ConcurrentBag<CreatedGame> CreatedGames { get; }
        ConcurrentBag<Game> StartedGames { get; }
        ConcurrentBag<CardTemplate> CardTemplates { get; }
    }
}