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
        public Line[,,] Lines;
        public bool GameComplete;
        public List<Box> Boxes;
        public Enums.GameStatus GameStatus;
        public int Player1BoxesFilled;
        public int Player2BoxesFilled;
        public GameResult GameResult;

        public string GameStatusForDisplay => GameStatus.ToString().Replace("_", " ");
        public string GameResultForDisplay => GameResult.ToString().Replace("_", " ");
        private Box GetBox(int rowIndex, int colIndex) => Boxes
                                                            .Where(x => x.RowIndex == rowIndex && x.ColIndex == colIndex)
                                                            .FirstOrDefault();

        private bool BoxCompleted;

        public GameBoard()
        {
            ResetGame();
        }

        public void ResetGame()
        {
            BoardStyles = new string[Game.NumberOfLineDirections, Game.NumberOfRows, Game.NumberOfColumns];
            BoxStyles = new string[Game.NumberOfRows, Game.NumberOfColumns];

            Lines = new Line[2, Game.NumberOfRows, Game.NumberOfColumns];
            Boxes = new List<Box>();

            int boxIndex = 0;

            for (int rowIndex = 0; rowIndex < Game.NumberOfRows; rowIndex++)
            {
                for (int colIndex = 0; colIndex < Game.NumberOfColumns; colIndex++)
                {
                    Lines[0, rowIndex, colIndex] = new Line(LineDirection.Horizontal, GameColour.White, rowIndex, colIndex);
                    Lines[1, rowIndex, colIndex] = new Line(LineDirection.Vertical, GameColour.White, rowIndex, colIndex);

                    if(rowIndex < Game.NumberOfRows -1 && colIndex < Game.NumberOfColumns - 1)
                    {
                        var box = new Box(boxIndex, GameColour.White, rowIndex, colIndex);

                        Boxes.Add(box);

                        boxIndex++;
                    }
                }
            }

            CurrentPlayer = Player.Player1;
            GameStatus = GameStatus.In_Progress;
            GameComplete = false;
            BoxCompleted = false;
            Player1BoxesFilled = 0;
            Player2BoxesFilled = 0;
        }

        public void DrawLine(Line line)
        {
            if(GameComplete || line.LineClicked)
            {
                return;
            }

            if(BoxCompleted)
            {
                BoxCompleted = false;
            }
            
            line.GameColour = CurrentPlayer == Player.Player1 ? GameColour.Red : GameColour.Blue;
            line.LineClicked = true;

            var possibleBoxes = new List<Box>();

            foreach (var box in Boxes)
            {
                if (line.LineDirection == LineDirection.Horizontal)
                {
                    bool topLine = line.RowIndex == 0;

                    bool isBottomLine = line.RowIndex == Game.NumberOfRows - 1;
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

            if (possibleBoxes == null)
            {
                return;
            }

            foreach(var possibleBox in possibleBoxes)
            {
                bool isBottomRowBox = possibleBox.RowIndex == Game.NumberOfRows - 1;

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

                possibleBox.BoxFilled = possibleBox.TopLineClicked
                                            && possibleBox.LeftLineClicked
                                            && possibleBox.BottomLineClicked
                                            && possibleBox.RightLineClicked;

                if (possibleBox.BoxFilled)
                {
                    possibleBox.PlayerColour = line.GameColour;
                    BoxCompleted = true;

                    if(CurrentPlayer == Player.Player1)
                    {
                        Player1BoxesFilled++;
                    }
                    else
                    {
                        Player2BoxesFilled++;
                    }
                }

                int possibleBoxIndex = Boxes.IndexOf(possibleBox);
                Boxes[possibleBoxIndex] = possibleBox;
            }

            GameComplete = Boxes.All(x => x.BoxFilled);

            if(GameComplete)
            {
                GameStatus = GameStatus.Game_Over;

                if(Player1BoxesFilled > Player2BoxesFilled)
                {
                    GameResult = GameResult.Player_1_Wins;
                }
                else if (Player2BoxesFilled > Player1BoxesFilled)
                {
                    GameResult = GameResult.Player_2_Wins;
                }
                else
                {
                    GameResult = GameResult.Draw;
                }
            }
            else if(!BoxCompleted)
            {
                CurrentPlayer = CurrentPlayer == Player.Player1 ? Player.Player2 : Player.Player1;
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
}
