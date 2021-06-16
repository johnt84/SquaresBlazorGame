namespace SquaresBlazorGame.Models
{
    public class Game
    {
        public int NumberOfLineDirections { get; } = 2;
        public int NumberOfRows { get; set; }
        public int NumberOfColumns { get; set; }

        public Game(int numberOfRows, int numberOfColumns)
        {
            NumberOfRows = numberOfRows;
            NumberOfColumns = numberOfColumns;
        }
    }
}
