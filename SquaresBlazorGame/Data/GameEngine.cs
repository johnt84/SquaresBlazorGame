using SquaresBlazorGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SquaresBlazorGame.Models.Enums;

namespace SquaresBlazorGame.Data
{
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

                    if(rowIndex < GameBoard.Game.NumberOfRows -1 && colIndex < GameBoard.Game.NumberOfColumns - 1)
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
            GameBoard.ComputerBoxCompleted = false;
            GameBoard.Player1BoxesFilled = 0;
            GameBoard.Player2BoxesFilled = 0;
            GameBoard.Player2IsComputerPlayer = true;
        }

        public async Task DrawLine(Line line)
        {
            if(GameBoard.GameStatus == GameStatus.Game_Over || line.LineClicked)
            {
                return;
            }

            if(GameBoard.BoxCompleted)
            {
                GameBoard.BoxCompleted = false;
            }

            if(GameBoard.ComputerBoxCompleted)
            {
                GameBoard.ComputerBoxCompleted = false;
            }

            line.GameColour = GameBoard.CurrentPlayer == Player.Player1 ? GameColour.Red : GameColour.Blue;
            line.LineClicked = true;

            GameBoard.Lines[(int)line.LineDirection, line.RowIndex, line.ColIndex] = line;

            var possibleBoxes = FindPossibleBoxesForLine(line);

            if (possibleBoxes == null)
            {
                return;
            }

            SelectLineOnMatchingBoxes(line, possibleBoxes);

            bool gameComplete = GameBoard.Boxes.All(x => x.BoxFilled);

            if(gameComplete)
            {
                GameBoard.GameStatus = GameStatus.Game_Over;

                SetGameResult();

                return;
            }
            
            if(!GameBoard.BoxCompleted)
            {
                GameBoard.CurrentPlayer = GameBoard.CurrentPlayer == Player.Player1 ? Player.Player2 : Player.Player1;
            }

            if (GameBoard.Player2IsComputerPlayer && GameBoard.CurrentPlayer == Player.Player2)
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
            if(!GameBoard.ComputerBoxCompleted)
            {
                await Task.Delay(ComputerMoveTimeInMilliseconds);
            }
            
            var computerLineToSelect = FindBestMoveForComputer();

            await DrawLine(computerLineToSelect);
        }

        private Line FindBestMoveForComputer()
        {
            var availableLines = FindLinesNotYetSelected();

            var boxesCanBeCompleted = FindBoxesWhichCanBeCompleted(availableLines);

            if(boxesCanBeCompleted.Count > 0)
            {
                var boxesCanBeCompletedArr = boxesCanBeCompleted.ToArray();

                var boxesToSelectRnd = new Random();
                int boxesToSelectSelectedPostion = boxesToSelectRnd.Next(0, boxesCanBeCompleted.Count);

                var boxToComplete = boxesCanBeCompletedArr[boxesToSelectSelectedPostion];

                return boxToComplete.LineToSelect;
            }

            var availableLinesToSelectArr = availableLines.ToArray();

            var rnd = new Random();
            int computerSelectionPostion = rnd.Next(0, availableLines.Count);

            return availableLinesToSelectArr[computerSelectionPostion];
        }

        private List<Line> FindLinesNotYetSelected()
        {
            var linesNotSelected = new List<Line>();

            for (int lineDirectionIndex = 0; lineDirectionIndex < GameBoard.Game.NumberOfLineDirections; lineDirectionIndex++)
            {
                int rowOffset = lineDirectionIndex == (int)LineDirection.Horizontal 
                                                            ? 0 
                                                            : 1;

                int rowsWithOffset = GameBoard.Game.NumberOfRows - rowOffset;

                int colOffset = lineDirectionIndex == (int)LineDirection.Horizontal
                                                            ? 1
                                                            : 0;

                int colsWithOffset = GameBoard.Game.NumberOfColumns - colOffset;


                for (int rowIndex = 0; rowIndex < rowsWithOffset; rowIndex++)
                {
                    for (int colIndex = 0; colIndex < colsWithOffset; colIndex++)
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
                        
                        if(GameBoard.Player2IsComputerPlayer)
                        {
                            GameBoard.ComputerBoxCompleted = true;
                        }
                    }
                }

                int possibleBoxIndex = GameBoard.Boxes.IndexOf(possibleBox);
                GameBoard.Boxes[possibleBoxIndex] = possibleBox;
            }
        }
        
        private List<Box> FindBoxesWhichCanBeCompleted(List<Line> availableLines)
        {
            var boxesCanBeCompleted = new List<Box>();

            foreach (var line in availableLines)
            {
                var possibleBoxesForLine = FindPossibleBoxesForLine(line);

                var boxesCanBeCompletedForLine = possibleBoxesForLine
                                                    .Where(x => x.LinesDrawn == Constants.LINES_IN_A_BOX - 1)
                                                    .ToList();

                if (boxesCanBeCompletedForLine.Count > 0)
                {
                    boxesCanBeCompleted.AddRange(boxesCanBeCompletedForLine);

                    boxesCanBeCompleted.ForEach(x => x.LineToSelect = line);
                }
            }

            return boxesCanBeCompleted;
        }

        private void SetGameResult()
        {
            if (GameBoard.Player1BoxesFilled > GameBoard.Player2BoxesFilled)
            {
                GameBoard.GameResult = GameResult.Player_1_Wins;
            }
            else if (GameBoard.Player2BoxesFilled > GameBoard.Player1BoxesFilled)
            {
                GameBoard.GameResult = GameResult.Player_2_Wins;
            }
            else
            {
                GameBoard.GameResult = GameResult.Draw;
            }
        }
    }
}
