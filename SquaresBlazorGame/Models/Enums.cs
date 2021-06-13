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

        public enum LineType
        {
            TopLine,
            LeftLine,
            BottomLine,
            RightLine,
        }

        public enum PlayerColour
        {
            Red,
            Blue,
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
            Player1,
            Player2,
        }
    }
}
