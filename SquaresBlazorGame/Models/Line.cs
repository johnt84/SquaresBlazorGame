using static SquaresBlazorGame.Models.Enums;

namespace SquaresBlazorGame.Models
{
    public class Line
    {
        public int LineNumber { get; set; }
        public bool LineClicked { get; set; } = false;
        public LineDirection LineDirection { get; set; }
        public LineType LineType { get; set; }
        public GameColour GameColour { get; set; }

        public int BoxNumber { get; set; }

        public Line(LineDirection lineDirection, LineType lineType, GameColour gameColour, int boxNumber)
        {
            LineDirection = lineDirection;
            LineType = lineType;
            GameColour = gameColour;
            BoxNumber = boxNumber;
        }
    }
}
