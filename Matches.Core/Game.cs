using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;

namespace Matches.Core
{
    public class Game
    {
        private readonly Board _board;
        private readonly object _locker = new();
        private readonly PlayersQueue _playersQueue;
        private Move _firstMove;
        private Move _secondMove;

        public Game(string id, GameParameters parameters, IReadOnlyList<Player> players)
            : this(id, ShuffledPlayers(players), new Board(parameters))
        {
        }

        public Game(string id, List<Player> players, Board board)
        {
            if (players == null) throw new ArgumentNullException(nameof(players));
            if (players.Count == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(players));
            Id = id ?? throw new ArgumentNullException(nameof(id));
            _board = board ?? throw new ArgumentNullException(nameof(board));
            _playersQueue = new PlayersQueue(players);
            GameState = GameState.WaitingForFirstMove;
        }

        public string Id { get; }
        public GameState GameState { get; private set; }
        public IReadOnlyList<Player> Players => _playersQueue.Players;
        public Player CurrentPlayer => _playersQueue.CurrentPlayer;
        public BoardState BoardState => _board.BoardState;
        public Move FirstMove => _firstMove;
        public Move SecondMove => _secondMove;

        public Result<Card> OpenCard(string playerId, int index)
        {
            lock (_locker)
            {
                if (!_playersQueue.IsPlayerTurn(playerId)) return Result.Failure<Card>("This is not your turn");
                if (GameState == GameState.DoneFirstMove && index == _firstMove.Index)
                {
                    return Result.Failure<Card>("You can not select card twice in one round");
                }
                var card = _board.OpenCard(index);
                if (card == Card.NoCard) return Result.Failure<Card>("You selected empty space");
                if (IsThisFirstMove())
                {
                    _firstMove = new Move(card, index);
                    GameState = GameState.DoneFirstMove;
                }
                else if (IsThisSecondMove())
                {
                    _secondMove = new Move(card, index);
                    GameState = _secondMove.Card.Equals(_firstMove.Card)
                        ? GameState.OpenedTwoEqualCards
                        : GameState.OpenedTwoNotEqualsCards;
                    if (GameState == GameState.OpenedTwoEqualCards)
                    {
                        _playersQueue.IncrementCurrentPlayerScore();
                        RemoveCard(_firstMove.Index);
                        RemoveCard(_secondMove.Index);
                    }
                }

                else
                {
                    return Result.Failure<Card>("You already did all moves");
                }

                return Result.Success(card);
            }
        }

        public bool IsGameFinished()
        {
            lock (_locker)
            {
                if (GameState == GameState.Finished) return true;

                if (!_board.IsBoardEmpty()) return false;
                GameState = GameState.Finished;

                return true;
            }
        }

        public Result<Player> ChooseNextPlayer()
        {
            lock (_locker)
            {
                if (GameState == GameState.Finished) return Result.Failure<Player>("Game is over");

                if (IsThisFirstMove() || IsThisSecondMove()) return _playersQueue.CurrentPlayer;

                _firstMove = null;
                _secondMove = null;

                if (GameState != GameState.OpenedTwoEqualCards)
                {
                    _playersQueue.NextPlayer();
                }

                GameState = GameState.WaitingForFirstMove;
                return _playersQueue.CurrentPlayer;
            }
        }

        public IReadOnlyList<Player> GetWinners()
        {
            if (Players.Count == 0)
            {
                return new List<Player>();
            }
            
            var maxScore = Players.Max(pl => pl.Score);
            return Players.Where(pl => pl.Score == maxScore).ToList();
        }

        private bool IsThisFirstMove()
        {
            return _firstMove == null;
        }

        private bool IsThisSecondMove()
        {
            return _firstMove != null && _secondMove == null;
        }

        private void RemoveCard(int index)
        {
            _board.RemoveCard(index);
        }

        private static List<Player> ShuffledPlayers(IEnumerable<Player> players)
        {
            var shuffledPlayers = players.ToList();
            shuffledPlayers.Shuffle();
            return shuffledPlayers;
        }
    }
}