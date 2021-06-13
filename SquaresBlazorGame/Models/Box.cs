using static SquaresBlazorGame.Models.Enums;

namespace SquaresBlazorGame.Models
{
    public class Box
    {
        public int BoxNumber { get; set; }
        public GameColour PlayerColour { get; set; }
        public Line TopLine { get; set; }
        public Line LeftLine { get; set; }
        public Line BottomLine { get; set; }
        public Line RightLine { get; set; }
        public bool BoxFilled { get; set; }
        public int RowNumber { get; set; }
        public int ColNumber { get; set; }

        public Box(int boxNumber, GameColour playerColour, int rowNumber, int colNumber)
        {
            BoxNumber = boxNumber;
            PlayerColour = playerColour;
            TopLine = new Line(LineDirection.Horizontal, LineType.TopLine, playerColour, boxNumber);
            LeftLine = new Line(LineDirection.Horizontal, LineType.LeftLine, playerColour, boxNumber);
            BottomLine = new Line(LineDirection.Horizontal, LineType.BottomLine, playerColour, boxNumber);
            RightLine = new Line(LineDirection.Horizontal, LineType.RightLine, playerColour, boxNumber);
            RowNumber = rowNumber;
            ColNumber = colNumber;
        }
    }
}
