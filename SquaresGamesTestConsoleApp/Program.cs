using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SquaresGamesTestConsoleApp.Enums;

namespace SquaresGamesTestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadLine();
        }

        static async Task Run()
        {
            var gameEngine = new GameEngine();

            //var firstRow = gameBoard.Rows.First();

            var firstBox = gameEngine.GameBoard.Boxes.First();

            var line1 = new Line(LineDirection.Horizontal, GameColour.Red, 0, 0);

            await gameEngine.DrawLine(line1);

            var line2 = new Line(LineDirection.Horizontal, GameColour.Red, 0, 1);

            await gameEngine.DrawLine(line2);

            var line3 = new Line(LineDirection.Vertical, GameColour.Red, 0, 0);

            await gameEngine.DrawLine(line3);

            var line4 = new Line(LineDirection.Vertical, GameColour.Red, 0, 1);

            await gameEngine.DrawLine(line4);
        }
    }

    public class GameEngine
    {
        public GameBoard GameBoard { get; set; }
        public string GameStatusForDisplay => GameBoard.GameStatus.ToString().Replace("_", " ");
        public string GameResultForDisplay => GameBoard.GameResult.ToString().Replace("_", " ");
        public int HorizontalLineDirection => (int)LineDirection.Horizontal;
        public int VerticalLineDirection => (int)LineDirection.Vertical;
        private Box GetBox(int rowIndex, int colIndex) => GameBoard.Boxes
                                                            .Where(x => x.RowIndex == rowIndex && x.ColIndex == colIndex)
                                                            .FirstOrDefault();
        private int ComputerMoveTimeInMilliseconds = 500;

        public GameEngine()
        {
            ResetGame();
        }

        public void ResetGame()
        {
            GameBoard = new GameBoard();

            GameBoard.Game = new Game(5, 6);

            GameBoard.Lines = new Line[GameBoard.Game.NumberOfLineDirections, GameBoard.Game.NumberOfRows, GameBoard.Game.NumberOfColumns];
            GameBoard.Boxes = new List<Box>();

            int boxIndex = 0;

            for (int rowIndex = 0; rowIndex < GameBoard.Game.NumberOfRows; rowIndex++)
            {
                for (int colIndex = 0; colIndex < GameBoard.Game.NumberOfColumns; colIndex++)
                {
                    GameBoard.Lines[HorizontalLineDirection, rowIndex, colIndex] = new Line(LineDirection.Horizontal, GameColour.White, rowIndex, colIndex);
                    GameBoard.Lines[VerticalLineDirection, rowIndex, colIndex] = new Line(LineDirection.Vertical, GameColour.White, rowIndex, colIndex);

                    if (rowIndex < GameBoard.Game.NumberOfRows - 1 && colIndex < GameBoard.Game.NumberOfColumns - 1)
                    {
                        var box = new Box(boxIndex, GameColour.White, rowIndex, colIndex);

                        GameBoard.Boxes.Add(box);

                        boxIndex++;
                    }
                }
            }

            GameBoard.CurrentPlayer = Player.Player1;
            GameBoard.GameStatus = GameStatus.In_Progress;
            GameBoard.BoxCompleted = false;
            GameBoard.Player1BoxesFilled = 0;
            GameBoard.Player2BoxesFilled = 0;
            GameBoard.Player2IsComputerPlayer = true;
        }

        public async Task DrawLine(Line line)
        {
            if (GameBoard.GameStatus == GameStatus.Game_Over || line.LineClicked)
            {
                return;
            }

            if (GameBoard.BoxCompleted)
            {
                GameBoard.BoxCompleted = false;
            }

            line.GameColour = GameBoard.CurrentPlayer == Player.Player1 ? GameColour.Red : GameColour.Blue;
            line.LineClicked = true;

            GameBoard.Lines[(int)line.LineDirection, line.RowIndex, line.ColIndex] = line;

            Console.WriteLine($"\n{GameBoard.CurrentPlayer}");
            Console.WriteLine($"\n{line.LineDirection.ToString()}");
            Console.WriteLine($"\n{line.RowIndex.ToString()}");
            Console.WriteLine($"\n{line.ColIndex.ToString()}");

            var possibleBoxes = FindPossibleBoxesForLine(line);

            if (possibleBoxes == null)
            {
                return;
            }

            SelectLineOnMatchingBoxes(line, possibleBoxes);

            bool gameComplete = GameBoard.Boxes.All(x => x.BoxFilled);

            if (gameComplete)
            {
                GameBoard.GameStatus = GameStatus.Game_Over;

                SetGameResult();

                return;
            }

            if (!GameBoard.BoxCompleted)
            {
                GameBoard.CurrentPlayer = GameBoard.CurrentPlayer == Player.Player1 ? Player.Player2 : Player.Player1;
            }

            if (GameBoard.CurrentPlayer == Player.Player2)
            {
                await ComputerUserMove();
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

        private async Task ComputerUserMove()
        {
            await Task.Delay(ComputerMoveTimeInMilliseconds);

            var computerLineToSelect = FindBestMoveForComputer();

            await DrawLine(computerLineToSelect);
        }

        private Line FindBestMoveForComputer()
        {
            Line bestLineToSelect = null;

            var availableLinesToSelect = FindLinesNotYetSelected();

            var boxesCanBeFilled = new List<Box>();

            foreach (var line in availableLinesToSelect)
            {
                var possibleBoxesForLine = FindPossibleBoxesForLine(line);

                var boxesCanBeFilledForLine = possibleBoxesForLine
                                                .Where(x => x.LinesDrawn == Constants.LINES_IN_A_BOX - 1)
                                                .ToList();

                if(boxesCanBeFilledForLine.Count > 0)
                {
                    Console.WriteLine($"\nBoxes can be filled for line for computer count - {boxesCanBeFilledForLine.Count()}");

                    boxesCanBeFilled.Union(boxesCanBeFilledForLine);

                    boxesCanBeFilled.ForEach(x => x.LineToSelect = line);
                }
            }

            if (boxesCanBeFilled.Count > 0)
            {
                Console.WriteLine("\nBoxes can be filled for the computer");
                
                var boxesCanBeFilledArr = boxesCanBeFilled.ToArray();

                var boxesToSelectRnd = new Random();
                int boxesToSelectSelectedPostion = boxesToSelectRnd.Next(0, boxesCanBeFilled.Count);

                var selectedBox = boxesCanBeFilledArr[boxesToSelectSelectedPostion];

                return selectedBox.LineToSelect;
            }



            var availableLinesToSelectArr = availableLinesToSelect.ToArray();

            var rnd = new Random();
            int computerSelectionPostion = rnd.Next(0, availableLinesToSelect.Count);

            bestLineToSelect = availableLinesToSelectArr[computerSelectionPostion];


            //var boxesCanComplete = GameBoard
            //                        .Boxes
            //                        .Where(x => x.LinesDrawn == Constants.LINES_IN_A_BOX - 1);

            //if (boxesCanComplete.Count() > 0)
            //{
            //    if (boxesCanComplete.Count() > 1)
            //    {

            //    }
            //    else
            //    {
            //        var box = boxesCanComplete.First();

            //        if (box.TopLineClicked && box.LeftLineClicked && box.RightLineClicked)
            //        {
            //            bestLineToSelect = box.
            //        }
            //    }
            //}

            return bestLineToSelect;
        }

        private List<Line> FindLinesNotYetSelected()
        {
            var linesNotSelected = new List<Line>();

            for (int lineDirectionIndex = 0; lineDirectionIndex < GameBoard.Game.NumberOfLineDirections; lineDirectionIndex++)
            {
                for (int rowIndex = 0; rowIndex < GameBoard.Game.NumberOfRows; rowIndex++)
                {
                    for (int colIndex = 0; colIndex < GameBoard.Game.NumberOfColumns; colIndex++)
                    {
                        var currentLine = GameBoard.Lines[lineDirectionIndex, rowIndex, colIndex];

                        if (!currentLine.LineClicked)
                        {
                            linesNotSelected.Add(currentLine);
                        }
                    }
                }
            }

            return linesNotSelected;
        }

        private List<Box> FindPossibleBoxesForLine(Line line)
        {
            var possibleBoxes = new List<Box>();

            foreach (var box in GameBoard.Boxes)
            {
                if (line.LineDirection == LineDirection.Horizontal)
                {
                    bool topLine = line.RowIndex == 0;

                    bool isBottomLine = line.RowIndex == GameBoard.Game.NumberOfRows - 1;
                    bool bottomRowMatches = isBottomLine && box.RowIndex == line.RowIndex - 1;

                    bool topOrBottomLine = topLine || isBottomLine;

                    bool nonBottomRowMatch = box.RowIndex == line.RowIndex
                                                    || (!topOrBottomLine && box.RowIndex == line.RowIndex - 1);

                    bool rowMatches = bottomRowMatches || nonBottomRowMatch;

                    bool colMatches = box.ColIndex == line.ColIndex;

                    if (rowMatches && colMatches)
                    {
                        possibleBoxes.Add(box);
                    }
                }
                else
                {
                    bool rowMatches = box.RowIndex == line.RowIndex;
                    bool colMatches = box.ColIndex == line.ColIndex
                                                        || (box.ColIndex == line.ColIndex - 1);

                    if (rowMatches && colMatches)
                    {
                        possibleBoxes.Add(box);
                    }
                }
            }

            return possibleBoxes;
        }

        private void SelectLineOnMatchingBoxes(Line line, List<Box> possibleBoxes)
        {
            foreach (var possibleBox in possibleBoxes)
            {
                if (line.LineDirection == LineDirection.Horizontal)
                {
                    if (possibleBox.RowIndex == line.RowIndex)
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
                    if (possibleBox.ColIndex == line.ColIndex)
                    {
                        possibleBox.LeftLineClicked = true;
                    }
                    else
                    {
                        possibleBox.RightLineClicked = true;
                    }
                }

                possibleBox.LinesDrawn++;

                possibleBox.BoxFilled = possibleBox.TopLineClicked
                                            && possibleBox.LeftLineClicked
                                            && possibleBox.BottomLineClicked
                                            && possibleBox.RightLineClicked;

                if (possibleBox.BoxFilled)
                {
                    possibleBox.PlayerColour = line.GameColour;
                    GameBoard.BoxCompleted = true;

                    if (GameBoard.CurrentPlayer == Player.Player1)
                    {
                        GameBoard.Player1BoxesFilled++;
                    }
                    else
                    {
                        GameBoard.Player2BoxesFilled++;
                    }
                }

                int possibleBoxIndex = GameBoard.Boxes.IndexOf(possibleBox);
                GameBoard.Boxes[possibleBoxIndex] = possibleBox;
            }
        }

        private void SetGameResult()
        {
            if (GameBoard.Player1BoxesFilled > GameBoard.Player2BoxesFilled)
            {
                GameBoard.GameResult = Enums.GameResult.Player_1_Wins;
            }
            else if (GameBoard.Player2BoxesFilled > GameBoard.Player1BoxesFilled)
            {
                GameBoard.GameResult = Enums.GameResult.Player_2_Wins;
            }
            else
            {
                GameBoard.GameResult = Enums.GameResult.Draw;
            }
        }
    }
    public class GameBoard
    {
        public Game Game { get; set; }
        public bool Player2IsComputerPlayer { get; set; }
        public Player CurrentPlayer { get; set; }
        public Line[,,] Lines { get; set; }
        public List<Box> Boxes { get; set; }
        public Enums.GameStatus GameStatus { get; set; }
        public int Player1BoxesFilled { get; set; }
        public int Player2BoxesFilled { get; set; }
        public GameResult GameResult { get; set; }
        public bool BoxCompleted { get; set; }
    }

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

        public enum GameResult
        {
            Player_1_Wins,
            Player_2_Wins,
            Draw,
        }
    }

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

    public class Constants
    {
        public const int LINES_IN_A_BOX = 4;
    }
}
