using SquaresBlazorGame.Models;
using System.Collections.Generic;
using System.Linq;
using static SquaresBlazorGame.Models.Enums;

namespace SquaresBlazorGame.Data
{
    public class GameBoard
    {
        public Game Game = new Game(5, 4, 2);
        public string[,,] BoardStyles;
        public string[,] BoxStyles;
        public Player CurrentPlayer;
        public List<Row> Rows;
        public List<Line> Lines;
        public bool GameComplete;

        public GameBoard()
        {
            ResetGame();
        }

        public void ResetGame()
        {
            BoardStyles = new string[Game.NumberOfLineDirections, Game.NumberOfRows, Game.NumberOfColumns];
            BoxStyles = new string[Game.NumberOfRows, Game.NumberOfColumns];

            Rows = new List<Row>();
            Lines = new List<Line>();

            int boxIndex = 0;

            for (int rowIndex = 0; rowIndex < Game.NumberOfRows - 1; rowIndex++)
            {
                var rowBoxes = new List<Box>();

                for (int colIndex = 0; colIndex < Game.NumberOfColumns - 1; colIndex++)
                {
                    var box = new Box(boxIndex, GameColour.White, rowIndex, colIndex);

                    rowBoxes.Add(box);

                    boxIndex++;
                }

                var row = new Row(rowIndex, rowBoxes);

                Rows.Add(row);
            }

            CurrentPlayer = Player.Player1;
        }

        public void DrawLine(Line line)
        {
            if(GameComplete || line.LineClicked)
            {
                return;
            }
            
            line.GameColour = CurrentPlayer == Player.Player1 ? GameColour.Red : GameColour.Blue;
            line.LineClicked = true;

            CurrentPlayer = CurrentPlayer == Player.Player1 ? Player.Player2 : Player.Player1;

            var currentRow = Rows.Select(x => x.Boxes.Where(y => y.BoxNumber == line.BoxNumber)).FirstOrDefault();

            if (currentRow == null)
            {
                return;
            }

            var currentBox = currentRow.Where(x => x.BoxNumber == line.BoxNumber).FirstOrDefault();

            if (currentBox == null)
            {
                return;
            }

            switch (line.LineType)
            {
                case LineType.TopLine:
                    currentBox.TopLine = line;
                    break;
                case LineType.LeftLine:
                    currentBox.LeftLine = line;
                    break;
                case LineType.BottomLine:
                    currentBox.BottomLine = line;
                    break;
                case LineType.RightLine:
                    currentBox.RightLine = line;
                    break;
                default:
                    break;
            }

            currentBox.BoxFilled = currentBox.TopLine.LineClicked
                                        && currentBox.LeftLine.LineClicked
                                        && currentBox.BottomLine.LineClicked
                                        && currentBox.RightLine.LineClicked;

            if (currentBox.BoxFilled)
            {
                currentBox.PlayerColour = line.GameColour;
            }

            var boxes = Rows.Select(x => x.Boxes).ToList();

            List<Box> allBoxes = null;

            foreach(var box in boxes)
            {
                foreach(var box2 in box)
                {
                    allBoxes.Add(box2);
                }
            }

            GameComplete = allBoxes.All(x => x.BoxFilled);
        }

    }
}
