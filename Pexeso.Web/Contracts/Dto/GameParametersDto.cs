namespace Pexeso.Contracts.Dto
{
    public class GameParametersDto
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public NewPlayerDto Player { get; set; }
        public string TemplateId { get; set; }
    }
}