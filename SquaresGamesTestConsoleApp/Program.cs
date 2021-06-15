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

            //var firstRow = gameBoard.Rows.First();

            var firstBox = gameBoard.Boxes.First();

            var line1 = new Line(LineDirection.Horizontal, GameColour.Red, 4, 1);

            gameBoard.DrawLine(line1);

            var line2 = new Line(LineDirection.Horizontal, GameColour.Red, 3, 1);

            gameBoard.DrawLine(line2);

            var line3 = new Line(LineDirection.Vertical, GameColour.Red, 3, 1);

            gameBoard.DrawLine(line3);

            var line4 = new Line(LineDirection.Vertical, GameColour.Red, 3, 2);

            gameBoard.DrawLine(line4);
        }
    }

    public class GameBoard
    {
        public Game Game = new Game(5, 4, 2);
        public string[,,] BoardStyles;
        public string[,] BoxStyles;
        public Player CurrentPlayer;
        //public List<Row> Rows;
        //public List<Line> Lines;
        public Line[,,] Lines;
        public bool GameComplete;
        public List<Box> Boxes;
        public Enums.GameStatus GameStatus;

        public string GameStatusForDisplay => GameStatus.ToString().Replace("_", " ");
        private Box GetBox(int rowIndex, int colIndex) => Boxes
                                                            .Where(x => x.RowIndex == rowIndex && x.ColIndex == colIndex)
                                                            .FirstOrDefault();

        public GameBoard()
        {
            ResetGame();
        }

        public void ResetGame()
        {
            BoardStyles = new string[Game.NumberOfLineDirections, Game.NumberOfRows, Game.NumberOfColumns];
            BoxStyles = new string[Game.NumberOfRows, Game.NumberOfColumns];

            //Rows = new List<Row>();
            //Lines = new List<Line>();
            Lines = new Line[2, Game.NumberOfRows, Game.NumberOfColumns];
            Boxes = new List<Box>();

            int boxIndex = 0;

            for (int rowIndex = 0; rowIndex < Game.NumberOfRows; rowIndex++)
            {
                //var rowBoxes = new List<Box>();

                for (int colIndex = 0; colIndex < Game.NumberOfColumns; colIndex++)
                {
                    var box = new Box(boxIndex, GameColour.White, rowIndex, colIndex);

                    //rowBoxes.Add(box);

                    var horizontalLineType = rowIndex % 2 == 0 ? LineType.TopLine : LineType.BottomLine;
                    var verticalLineType = rowIndex % 2 == 0 ? LineType.LeftLine : LineType.RightLine;

                    //List<int> boxNumbers;

                    //if (rowIndex == 0 || rowIndex == Game.NumberOfRows - 1)
                    //{
                    //    boxNumbers = new List<int>(boxIndex);
                    //}
                    //else
                    //{

                    //}

                    Lines[0, rowIndex, colIndex] = new Line(LineDirection.Horizontal, GameColour.White, rowIndex, colIndex);
                    Lines[1, rowIndex, colIndex] = new Line(LineDirection.Vertical, GameColour.White, rowIndex, colIndex);

                    //if(rowIndex < Game.NumberOfRows - 1 && colIndex < Game.NumberOfColumns - 1)
                    //{
                    //    BoxStyles[rowIndex, colIndex] = GameColour.White.ToString().ToLower();
                    //}

                    Boxes.Add(box);

                    boxIndex++;
                }

                //var row = new Row(rowIndex, rowBoxes);

                //Rows.Add(row);
            }

            CurrentPlayer = Player.Player1;
            GameStatus = GameStatus.In_Progress;
        }

        public void DrawLine(Line line)
        {
            if (GameComplete || line.LineClicked)
            {
                return;
            }

            line.GameColour = CurrentPlayer == Player.Player1 ? GameColour.Red : GameColour.Blue;
            line.LineClicked = true;

            CurrentPlayer = CurrentPlayer == Player.Player1 ? Player.Player2 : Player.Player1;

            //var currentRow = Rows.Select(x => x.Boxes.Where(y => y.BoxNumber == line.BoxNumber)).FirstOrDefault();

            //if (currentRow == null)
            //{
            //    return;
            //}

            //var currentBox = currentRow.Where(x => x.BoxNumber == line.BoxNumber).FirstOrDefault();
            //var currentBox = Boxes.Where(x => x.BoxNumber == line.BoxNumber).FirstOrDefault();

            var possibleBoxes = new List<Box>();

            bool isTopLine = line.RowIndex == 0;
            bool isBottomLine = line.RowIndex == Game.NumberOfRows - 1;
            bool isTopOrBottomLine = isTopLine || isBottomLine;
            bool isLeftLine = line.ColIndex == 0;
            bool isRightLine = line.ColIndex == Game.NumberOfColumns - 1;
            bool isLeftOrRightLine = isLeftLine || isRightLine;

            foreach (var box in Boxes)
            {
                if (line.LineDirection == LineDirection.Horizontal)
                {
                    //bool topLine = line.RowIndex == 0;
                    //bool bottomLine = line.RowIndex == Game.NumberOfRows - 1;
                    bool topOrBottomLine = isTopLine || isBottomLine;
                    bool lineAbove = box.RowIndex == line.RowIndex - 1;
                    bool lineBelow = box.RowIndex == line.RowIndex + 1;
                    bool lineAboveOrBelow = lineAbove || lineBelow;
                    bool rowMatches = box.RowIndex == line.RowIndex 
                                                    || (!topOrBottomLine && lineAboveOrBelow);
                    bool colMatches = box.ColIndex == line.ColIndex;

                    if (rowMatches && colMatches)
                    {
                        possibleBoxes.Add(box);
                    }
                }
                else
                {
                    //bool leftLine = line.ColIndex == 0;
                    //bool rightLine = line.ColIndex == Game.NumberOfColumns - 1;
                    bool leftOrRightLine = isLeftLine || isRightLine;
                    //bool lineToTheLeft = box.ColIndex == line.ColIndex - 1;
                    //bool lineToTheRight = box.ColIndex == line.ColIndex + 1;
                    //bool lineToTheLeftOrRight = lineToTheLeft || lineToTheRight;
                    bool rowMatches = box.RowIndex == line.RowIndex || box.RowIndex == line.RowIndex + 1;
                    bool colMatches = box.ColIndex == line.ColIndex
                                                        || (box.ColIndex == line.ColIndex - 1); //!leftOrRightLine && 

                    if (rowMatches && colMatches)
                    {
                        possibleBoxes.Add(box);
                    }
                }
            }

            //var possibleBoxes = Boxes
            //                        .Where(x => (x.RowIndex == line.RowIndex || ((x.RowIndex > 0 && x.RowIndex < Game.NumberOfRows - 1) && x.RowIndex == line.RowIndex - 1))
            //                         && (x.ColIndex == line.ColIndex || ((x.ColIndex > 0 && x.ColIndex < Game.NumberOfColumns - 1) && x.ColIndex == line.ColIndex - 1)))
            //                        .ToList();

            if (possibleBoxes == null)
            {
                return;
            }

            foreach (var possibleBox in possibleBoxes)
            {
                if (line.LineDirection == LineDirection.Horizontal)
                {
                    //If the top of bottom line of the gris id clicked
                    if(isTopOrBottomLine)
                    {
                        if(isTopLine)
                        {
                            possibleBox.TopLineClicked = true;
                        }
                        else
                        {
                            possibleBox.BottomLineClicked = true;
                        }
                    }
                    else
                    {
                        if (possibleBox.RowIndex == line.RowIndex || possibleBox.RowIndex == line.RowIndex + 1)
                        {
                            possibleBox.TopLineClicked = true;
                        }
                        else
                        {
                            possibleBox.BottomLineClicked = true;
                        }
                    }
                }
                else
                {
                    //If the top of bottom line of the gris id clicked
                    if (isLeftOrRightLine)
                    {
                        if (isLeftLine)
                        {
                            possibleBox.LeftLineClicked = true;
                        }
                        else
                        {
                            possibleBox.RightLineClicked = true;
                        }
                    }
                    else
                    {
                        if (possibleBox.ColIndex == line.ColIndex)
                        {
                            possibleBox.LeftLineClicked = true;
                        }
                        else
                        {
                            possibleBox.RightLineClicked = true;
                        }
                    }
                }

                possibleBox.BoxFilled = possibleBox.TopLineClicked
                                            && possibleBox.LeftLineClicked
                                            && possibleBox.BottomLineClicked
                                            && possibleBox.RightLineClicked;

                if (possibleBox.BoxFilled)
                {
                    possibleBox.PlayerColour = line.GameColour;
                    //BoxStyles[line.] = currentBox.PlayerColour;
                }

                int possibleBoxIndex = Boxes.IndexOf(possibleBox);
                Boxes[possibleBoxIndex] = possibleBox;
            }

            //switch (line.LineType)
            //{
            //    case LineType.TopLine:
            //        currentBox.TopLine = line;
            //        break;
            //    case LineType.LeftLine:
            //        currentBox.LeftLine = line;
            //        break;
            //    case LineType.BottomLine:
            //        currentBox.BottomLine = line;
            //        break;
            //    case LineType.RightLine:
            //        currentBox.RightLine = line;
            //        break;
            //    default:
            //        break;
            //}



            //var boxes = Rows.Select(x => x.Boxes).ToList();

            //var allBoxes = new List<Box>();

            //foreach(var box in boxes)
            //{
            //    foreach(var box2 in box)
            //    {
            //        allBoxes.Add(box2);
            //    }
            //}

            //GameComplete = allBoxes.All(x => x.BoxFilled);
            GameComplete = Boxes.All(x => x.BoxFilled);

            if (GameComplete)
            {
                GameStatus = GameStatus.Game_Over;
            }

        }

        public string GetBoxColour(int rowIndex, int colIndex)
        {
            var box = GetBox(rowIndex, colIndex);

            return box != null
                                ? box.PlayerColour.ToString()
                                : GameColour.White.ToString();
        }

        public int GetBoxNumber(int rowIndex, int colIndex)
        {
            var box = GetBox(rowIndex, colIndex);

            return box != null
                                ? box.BoxNumber
                                : 0;
        }
    }

    public class Box
    {
        public int BoxNumber { get; set; }
        public GameColour PlayerColour { get; set; }
        public bool TopLineClicked { get; set; }
        public bool LeftLineClicked { get; set; }
        public bool BottomLineClicked { get; set; }
        public bool RightLineClicked { get; set; }
        //public List<Line> Lines { get; set; }
        public bool BoxFilled { get; set; }
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }

        public Box(int boxNumber, GameColour playerColour, int rowIndex, int colIndex)
        {
            BoxNumber = boxNumber;
            PlayerColour = playerColour;
            //TopLine = new Line(LineDirection.Horizontal, LineType.TopLine, playerColour, boxNumber);
            //LeftLine = new Line(LineDirection.Horizontal, LineType.LeftLine, playerColour, boxNumber);
            //BottomLine = new Line(LineDirection.Horizontal, LineType.BottomLine, playerColour, boxNumber);
            //RightLine = new Line(LineDirection.Horizontal, LineType.RightLine, playerColour, boxNumber);
            RowIndex = rowIndex;
            ColIndex = colIndex;
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

        public enum GameStatus
        {
            In_Progress,
            Game_Over,
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
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }

        //public Line(int lineNumber, LineDirection lineDirection, GameColour gameColour)
        public Line(LineDirection lineDirection, GameColour gameColour, int rowIndex, int colIndex) //, List<int> boxNumbersLineType lineType,
        {
            LineDirection = lineDirection;
            //LineType = lineType;
            GameColour = gameColour;
            //BoxNumbers = boxNumbers;
            RowIndex = rowIndex;
            ColIndex = colIndex;
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
