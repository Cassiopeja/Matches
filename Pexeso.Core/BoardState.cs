namespace Pexeso.Core
{
    public record BoardState(int Rows, int Columns, int[] OpenedCardsIndexes, string BackImageUrl);
}