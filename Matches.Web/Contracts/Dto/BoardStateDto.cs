namespace Matches.Contracts.Dto
{
    public record BoardStateDto(int Rows, int Columns, int[] OpenedCardsIndexes, string BackImageUrl);
}