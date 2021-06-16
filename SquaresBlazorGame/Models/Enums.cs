namespace SquaresBlazorGame.Models
{
    public class Enums
    {
        public enum Shape
        {
            Dot,
            Box,
        }
        
        public enum LineDirection
        {
            Horizontal,
            Vertical,
        }

        public enum GameColour
        {
            Red,
            Blue,
            White,
            Grey,
        }

        public enum Player
        {
            Player_1,
            Player_2,
            Player,
            Computer,
        }

        public enum GameStatus
        {
            In_Progress,
            Game_Over,
        }

        public enum GameResult
        {
            Player_1_Wins,
            Player_2_Wins,
            Draw,
            Congratulations_you_win,
            Unlucky_the_computer_wins
        }
    }
}
