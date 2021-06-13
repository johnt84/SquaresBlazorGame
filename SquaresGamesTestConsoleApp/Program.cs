using System.Collections.Generic;
using System.Linq;
using static SquaresGamesTestConsoleApp.Enums;

namespace SquaresGamesTestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var gameBoard = new GameBoard();

            var firstRow = gameBoard.Rows.First();

            var firstBox = firstRow.Boxes.First();

            var line1 = firstBox.TopLine;

            gameBoard.DrawLine(line1);

            var line2 = firstBox.LeftLine;

            gameBoard.DrawLine(line2);

            var line3 = firstBox.BottomLine;

            gameBoard.DrawLine(line3);

            var line4 = firstBox.RightLine;

            gameBoard.DrawLine(line4);
        }
    }
    public class GameBoard
    {
        public Game Game = new Game(5, 4, 2);
        public string[,,] BoardStyles;
        public string[,] BoxStyles;
        public Player CurrentPlayer;
        public List<Row> Rows;
        public List<Line> Lines;
        //public List<Box> Boxes;
        public bool GameComplete;

        public GameBoard()
        {
            BoardStyles = new string[Game.NumberOfLineDirections, Game.NumberOfRows, Game.NumberOfColumns];
            BoxStyles = new string[Game.NumberOfRows, Game.NumberOfColumns];

            //for (int directionIndex = 0; directionIndex < Game.NumberOfLineDirections; directionIndex++)
            //{
            //    for (int rowIndex = 0; rowIndex < Game.NumberOfRows; rowIndex++)
            //    {
            //        for (int colIndex = 0; colIndex < Game.NumberOfColumns; colIndex++)
            //        {
            //            BoardStyles[directionIndex, rowIndex, colIndex] = GameColour.White.ToString().ToLower();
            //            BoxStyles[rowIndex, colIndex] = GameColour.White.ToString().ToLower();
            //        }
            //    }
            //}

            //int lineNumber = 0;

            //for (int directionIndex = 0; directionIndex < Game.NumberOfLineDirections; directionIndex++)
            //{
            //    for (int rowIndex = 0; rowIndex < Game.NumberOfRows; rowIndex++)
            //    {
            //        for (int colIndex = 0; colIndex < Game.NumberOfColumns; colIndex++)
            //        {
            //            var line = new Line(lineNumber, (LineDirection)directionIndex, GameColour.White);

            //            Lines.Add(line);

            //            lineNumber++;
            //        }
            //    }
            //}
            Rows = new List<Row>();
            Lines = new List<Line>();
            //Boxes = new List<Box>();

            int numberOfBoxes = (Game.NumberOfRows - 1) * (Game.NumberOfColumns - 1);

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
            if (line.LineClicked)
            {
                return;
            }

            line.GameColour = CurrentPlayer == Player.Player1 ? GameColour.Red : GameColour.Blue;
            line.LineClicked = true;

            var currentRow = Rows.Select(x => x.Boxes.Where(y => y.BoxNumber == line.BoxNumber)).FirstOrDefault();

            if(currentRow == null)
            {
                return;
            }

            var currentBox = currentRow.Where(x => x.BoxNumber == line.BoxNumber).FirstOrDefault();

            if (currentBox == null)
            {
                return;
            }

            if (currentBox != null)
            {
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
                else
                {
                    CurrentPlayer = CurrentPlayer == Player.Player1 
                                                        ? Player.Player2 
                                                        : Player.Player1; 
                }
            }
        }

    }

    public class Box
    {
        public int BoxNumber { get; set; }
        public GameColour PlayerColour { get; set; }
        public int LinesDrawn { get; set; } = 0;
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
        }

        public enum Player
        {
            Player1,
            Player2,
        }
    }

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

    public class Line
    {
        public int LineNumber { get; set; }
        public bool LineClicked { get; set; } = false;
        public LineDirection LineDirection { get; set; }
        public LineType LineType { get; set; }
        public GameColour GameColour { get; set; }

        public int BoxNumber { get; set; }

        //public Line(int lineNumber, LineDirection lineDirection, GameColour gameColour)
        public Line(LineDirection lineDirection, LineType lineType, GameColour gameColour, int boxNumber)
        {
            LineDirection = lineDirection;
            LineType = lineType;
            GameColour = gameColour;
            BoxNumber = boxNumber;
        }
    }

    public class Row
    {
        public int RowNumber { get; set; }
        public List<Box> Boxes { get; set; }

        public Row(int rowNumber, List<Box> boxes)
        {
            RowNumber = rowNumber;
            Boxes = boxes;
        }
    }
}
