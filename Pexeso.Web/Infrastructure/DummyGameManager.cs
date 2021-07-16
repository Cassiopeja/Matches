using System.Collections.Concurrent;
using System.Linq;
using Pexeso.Core;

namespace Pexeso.Infrastructure
{
    public class DummyGameManager : IGameManager
    {
       public ConcurrentBag<CreatedGame> CreatedGames { get; }
       public ConcurrentBag<Game> StartedGames { get; }
       public ConcurrentBag<CardTemplate> CardTemplates { get; }

       public DummyGameManager()
       {
           CreatedGames = new ConcurrentBag<CreatedGame>();
           StartedGames = new ConcurrentBag<Game>();
           var frontUrls = Enumerable.Range(1, 32).Select(index => $"front{index}").ToArray();
           CardTemplates = new ConcurrentBag<CardTemplate>()
           {
              new CardTemplate("Test", frontUrls, "back") 
           };
       }
    }
}