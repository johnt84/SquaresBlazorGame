using System.Collections.Generic;
using static SquaresBlazorGame.Models.Enums;

namespace SquaresBlazorGame.Models
{
    public class GameBoard
    {
        public Game Game { get; set; }
        public bool Player2IsComputerPlayer { get; set; }
        public Player CurrentPlayer { get; set; }
        public Line[,,] Lines { get; set; }
        public List<Box> Boxes { get; set; }
        public Enums.GameStatus GameStatus { get; set; }
        public int Player1BoxesFilled { get; set; }
        public int Player2BoxesFilled { get; set; }
        public GameResult GameResult { get; set; }
        public bool BoxCompleted { get; set; }
        public bool ComputerBoxCompleted { get; set; }
        public Player Player1Name { get; set; }
        public Player Player2Name { get; set; }
    }
}
