using static SquaresBlazorGame.Models.Enums;

namespace SquaresBlazorGame.Models
{
    public class Line
    {
        public bool LineClicked { get; set; } = false;
        public LineDirection LineDirection { get; set; }
        public GameColour GameColour { get; set; }
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }

        public Line(LineDirection lineDirection, GameColour gameColour, int rowIndex, int colIndex)
        {
            LineDirection = lineDirection;
            GameColour = gameColour;
            RowIndex = rowIndex;
            ColIndex = colIndex;
        }
    }
}
