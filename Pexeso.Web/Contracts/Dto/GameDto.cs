using System.Collections.Generic;

namespace Pexeso.Contracts.Dto
{
    public class GameDto
    {
        public string Id { get; set; }
        public List<PlayerDto> Players { get; set; }
        public PlayerDto CurrentPlayer { get; set; }
        public BoardStateDto BoardState { get; set; }
        public MoveDto FirstMove { get; set; }
        public MoveDto SecondMove { get; set; }
        public GameStateDto GameState { get; set; }
        
    }
}