using System;
using System.Collections.Generic;
using System.Linq;

namespace Pexeso.Core
{
    public class Board
    {
        private readonly Card[,] _board;

        public Board(GameParameters parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            var pile = CreatePile(parameters.Rows * parameters.Columns, parameters.CardTemplate);
            _board = CreateBoard(parameters.Rows, parameters.Columns, pile);
            CardBackImageUrl = parameters.CardTemplate.BackCardImageUrl;
        }

        public Board(Card[,] cards, string cardBackImageUrl)
        {
            _board = cards ?? throw new ArgumentNullException(nameof(cards));
            if (cards.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(cards));
            CardBackImageUrl = cardBackImageUrl ?? throw new ArgumentNullException(nameof(cardBackImageUrl));
        }

        public string CardBackImageUrl { get; }
        public int Rows => _board.GetLength(0);
        public int Columns => _board.GetLength(1);

        private Card[,] CreateBoard(int rows, int columns, List<Card> pile)
        {
            var board = new Card[rows, columns];

            var index = 0;
            for (var i = 0; i < rows; i++)
            for (var j = 0; j < columns; j++)
            {
                board[i, j] = pile[index];
                index++;
            }

            return board;
        }

        private List<Card> CreatePile(int count, CardTemplate cardTemplate)
        {
            var pile = cardTemplate.Cards.Take(count / 2).ToList();
            pile.AddRange(pile);
            pile.Shuffle();
            return pile;
        }

        public void RemoveCard(int row, int column)
        {
            if (row > Rows - 1 || row < 0) throw new ArgumentOutOfRangeException(nameof(row));

            if (column > Columns - 1 || column < 0) throw new ArgumentOutOfRangeException(nameof(column));

            _board[row, column] = null;
        }

        public Card OpenCard(int row, int column)
        {
            if (row > Rows - 1 || row < 0) throw new ArgumentOutOfRangeException(nameof(row));

            if (column > Columns - 1 || column < 0) throw new ArgumentOutOfRangeException(nameof(column));

            return _board[row, column] ?? Card.NoCard;
        }

        public bool IsBoardEmpty()
        {
            for (var i = 0; i < Rows; i++)
            for (var j = 0; j < Columns; j++)
                if (_board[i, j] != null)
                    return false;
            return true;
        }
    }
}