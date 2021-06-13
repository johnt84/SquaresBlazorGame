namespace SquaresBlazorGame.Models
{
    public class Game
    {
        public int NumberOfRows { get; set; }
        public int NumberOfColumns { get; set; }
        public int NumberOfLineDirections { get; set; }

        public Game(int numberOfRows, int numberOfColumns, int numberOfLineDirections)
        {
            NumberOfRows = numberOfRows;
            NumberOfColumns = numberOfColumns;
            NumberOfLineDirections = numberOfLineDirections;
        }
    }
}
