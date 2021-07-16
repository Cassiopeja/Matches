using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;

namespace Pexeso.Core
{
    public class Game
    {
        private readonly Board _board;
        private readonly object _locker = new();
        private readonly Queue<Player> _players;
        private Player _currentPlayer;
        private Move _firstMove;
        private Move _secondMove;

        public Game(string id, GameParameters parameters, IReadOnlyList<Player> players)
            : this(id, ShuffledPlayers(players), new Board(parameters))
        {
        }

        public Game(string id, List<Player> players, Board board)
        {
            if (players == null) throw new ArgumentNullException(nameof(players));
            if (board == null) throw new ArgumentNullException(nameof(board));
            if (players.Count == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(players));

            Id = id ?? throw new ArgumentNullException(nameof(id));
            _players = new Queue<Player>();
            players.ForEach(item => _players.Enqueue(item));
            _board = board;
            GameState = GameState.WaitingForFirstMove;
            _currentPlayer = _players.Dequeue();
        }

        public string Id { get; }
        public GameState GameState { get; private set; }

        public Result<Card> OpenCard(string playerId, int row, int column)
        {
            lock (_locker)
            {
                if (!IsPlayerTurn(playerId)) return Result.Failure<Card>("This is not your turn");
                var card = _board.OpenCard(row, column);
                if (card == Card.NoCard) return Result.Failure<Card>("You selected empty space");
                if (IsThisFirstMove())
                {
                    _firstMove = new Move(card, row, column);
                    GameState = GameState.DoneFirstMove;
                }
                else if (IsThisSecondMove())
                {
                    _secondMove = new Move(card, row, column);
                    GameState = _secondMove.Card.Equals(_firstMove.Card)
                        ? GameState.OpenedTwoEqualCards
                        : GameState.OpenedTwoNotEqualsCards;
                    if (GameState == GameState.OpenedTwoEqualCards)
                    {
                        _currentPlayer.Score++;
                        RemoveCard(_firstMove.Row, _firstMove.Column);
                        RemoveCard(_secondMove.Row, _secondMove.Column);
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

                if (IsThisFirstMove() || IsThisSecondMove()) return _currentPlayer;

                _firstMove = null;
                _secondMove = null;

                if (GameState != GameState.OpenedTwoEqualCards)
                {
                    _players.Enqueue(_currentPlayer);
                    _currentPlayer = _players.Dequeue();
                }

                GameState = GameState.WaitingForFirstMove;
                return _currentPlayer;
            }
        }

        public IReadOnlyCollection<Player> GetCurrentPlayers()
        {
            var players = _players.ToList();
            players.Add(_currentPlayer);

            return players.AsReadOnly();
        }

        private bool IsPlayerTurn(string playerId)
        {
            return _currentPlayer.ConnectionId == playerId;
        }

        private bool IsThisFirstMove()
        {
            return _firstMove == null;
        }

        private bool IsThisSecondMove()
        {
            return _firstMove != null && _secondMove == null;
        }

        private void RemoveCard(int row, int column)
        {
            _board.RemoveCard(row, column);
        }

        private static List<Player> ShuffledPlayers(IReadOnlyList<Player> players)
        {
            var shuffledPlayers = players.ToList();
            shuffledPlayers.Shuffle();
            return shuffledPlayers;
        }
    }
}