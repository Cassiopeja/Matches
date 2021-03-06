using System;
using System.Collections.Generic;
using System.Linq;

namespace Matches.Core
{
    public class Board
    {
        private readonly string _backImageUrl;
        private readonly Card[] _board;
        private readonly int _columns;
        private readonly int _rows;

        public Board(GameParameters parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            _rows = parameters.Rows;
            _columns = parameters.Columns;
            var pile = CreatePile(parameters.Rows * parameters.Columns, parameters.CardTemplate);
            _board = CreateBoard(pile);
            _backImageUrl = parameters.CardTemplate.BackCardImageUrl;
        }

        public Board(Card[,] cards, string cardBackImageUrl)
        {
            if (cards == null) throw new ArgumentNullException(nameof(cards));
            if (cards.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(cards));
            _board = ToOneDimensionBoard(cards);
            _backImageUrl = cardBackImageUrl ?? throw new ArgumentNullException(nameof(cardBackImageUrl));
            _rows = cards.GetLength(0);
            _columns = cards.GetLength(1);
        }

        public BoardState BoardState
        {
            get
            {
                return new(_rows, _columns,
                    _board.Select((card, index) => new {card, index}).Where(item => item.card == null)
                        .Select(item => item.index).ToArray(), 
                    _backImageUrl);
            }
        }

        private Card[] ToOneDimensionBoard(Card[,] cards)
        {
            var board = new Card[cards.Length];
            var rows = cards.GetLength(0);
            var columns = cards.GetLength(1);
            for (var i = 0; i < rows; i++)
            for (var j = 0; j < columns; j++)
                board[i * columns + j] = cards[i, j];

            return board;
        }

        private Card[] CreateBoard(IReadOnlyList<Card> pile)
        {
            var board = new Card[pile.Count];

            for (var i = 0; i < pile.Count; i++) board[i] = pile[i];

            return board;
        }

        private List<Card> CreatePile(int count, CardTemplate cardTemplate)
        {
            var pile = cardTemplate.Cards.Take(count / 2).ToList();
            pile.AddRange(pile);
            pile.Shuffle();
            return pile;
        }

        private void GuardIndex(int index)
        {
            if (index > _board.Length - 1 || index < 0) throw new ArgumentOutOfRangeException(nameof(index));
        }
        public void RemoveCard(int index)
        {
            GuardIndex(index);
            _board[index] = null;
        }

        public Card OpenCard(int index)
        {
            GuardIndex(index);
            return _board[index] ?? Card.NoCard;
        }

        public bool IsBoardEmpty()
        {
            return _board.All(t => t == null);
        }
    }
}