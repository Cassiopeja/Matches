using System;

namespace Pexeso.Core
{
    public class GameParameters
    {
        public CardTemplate CardTemplate { get; }
        public int Rows { get; }
        public int Columns { get; }

        public GameParameters(int rows, int columns, CardTemplate cardTemplate)
        {
            if (cardTemplate == null) throw new ArgumentNullException(nameof(cardTemplate));
            if (rows <= 0) throw new ArgumentOutOfRangeException(nameof(rows));
            if (columns <= 0) throw new ArgumentOutOfRangeException(nameof(columns));
            if (rows * columns % 2 != 0)
                throw new ArgumentException("Can't create board for specified rows and columns",
                    nameof(rows) + ' ' + nameof(columns));
            if (rows * columns > cardTemplate.TotalCount * 2)
                throw new ArgumentException(
                    "Can't create game board for specified rows and columns. Not enough cards in template.",
                    nameof(rows) + ' ' + nameof(columns));
            Rows = rows;
            Columns = columns;
            CardTemplate = cardTemplate;
        }
    }
}