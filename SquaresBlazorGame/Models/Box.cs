using static SquaresBlazorGame.Models.Enums;

namespace SquaresBlazorGame.Models
{
    public class Box
    {
        public int BoxNumber { get; set; }
        public GameColour PlayerColour { get; set; }
        public bool TopLineClicked { get; set; }
        public bool LeftLineClicked { get; set; }
        public bool BottomLineClicked { get; set; }
        public bool RightLineClicked { get; set; }
        public bool BoxFilled { get; set; }
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
        public int LinesDrawn { get; set; }
        public Line LineToSelect { get; set; }

        public Box(int boxNumber, GameColour playerColour, int rowIndex, int colIndex)
        {
            BoxNumber = boxNumber;
            PlayerColour = playerColour;
            RowIndex = rowIndex;
            ColIndex = colIndex;
        }
    }
}
