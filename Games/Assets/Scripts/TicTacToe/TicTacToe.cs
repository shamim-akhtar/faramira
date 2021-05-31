using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe
{
    public class CellIndex
    {
        public CellIndex()
        {
        }

        public CellIndex(int r, int c)
        {
            this.Row = r;
            this.Col = c;
        }

        public int Row { get; set; }
        public int Col { get; set; }
    };

    public class TicTacToe
    {
        private char[,] mBoard =
        {
            { ' ', ' ', ' ' },
            { ' ', ' ', ' ' },
            { ' ', ' ', ' ' }
        };
        private char computer = 'x', player = 'o';

        public int WinID = -1; // 0 == up row 1 = middle row and so on

        public TicTacToe()
        {
        }
        
        public bool IsAnyMoveLeft()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (mBoard[i, j] == ' ')
                        return true;
            return false;
        }

        int Evaluate(char[,] b)
        {
            // Checking for Rows for X or O victory.
            for (int row = 0; row < 3; row++)
            {
                if (b[row, 0] == b[row, 1] &&
                    b[row, 1] == b[row, 2])
                {
                    if (b[row, 0] == computer)
                        return +10;
                    else if (b[row, 0] == player)
                        return -10;
                }
            }

            // Checking for Columns for X or O victory.
            for (int col = 0; col < 3; col++)
            {
                if (b[0, col] == b[1, col] &&
                    b[1, col] == b[2, col])
                {
                    if (b[0, col] == computer)
                        return +10;

                    else if (b[0, col] == player)
                        return -10;
                }
            }

            // Checking for Diagonals for X or O victory.
            if (b[0, 0] == b[1, 1] && b[1, 1] == b[2, 2])
            {
                if (b[0, 0] == computer)
                    return +10;
                else if (b[0, 0] == player)
                    return -10;
            }

            if (b[0, 2] == b[1, 1] && b[1, 1] == b[2, 0])
            {
                if (b[0, 2] == computer)
                    return +10;
                else if (b[0, 2] == player)
                    return -10;
            }

            // Else if none of them have won then return 0
            return 0;
        }

        // This is the minimax function. It considers all
        // the possible ways the game can go and returns
        // the value of the board
        int Minimax(char[,] board, int depth, bool isMax)
        {
            int score = Evaluate(board);

            // If Maximizer has won the game
            // return his/her evaluated score
            if (score == 10)
                return score;

            // If Minimizer has won the game
            // return his/her evaluated score
            if (score == -10)
                return score;

            // If there are no more moves and
            // no winner then it is a tie
            if (IsAnyMoveLeft() == false)
                return 0;

            // If this maximizer's move
            if (isMax)
            {
                int best = -1000;

                // Traverse all cells
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        // Check if cell is empty
                        if (board[i, j] == ' ')
                        {
                            // Make the move
                            board[i, j] = computer;

                            // Call minimax recursively and choose
                            // the maximum value
                            best = Mathf.Max(best, Minimax(board, depth + 1, !isMax));

                            // Undo the move
                            board[i, j] = ' ';
                        }
                    }
                }
                return best;
            }

            // If this minimizer's move
            else
            {
                int best = 1000;

                // Traverse all cells
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        // Check if cell is empty
                        if (board[i, j] == ' ')
                        {
                            // Make the move
                            board[i, j] = player;

                            // Call minimax recursively and choose
                            // the minimum value
                            best = Mathf.Min(best, Minimax(board, depth + 1, !isMax));

                            // Undo the move
                            board[i, j] = ' ';
                        }
                    }
                }
                return best;
            }
        }

        public void SetMove(int r, int c, bool is_computer)
        {
            if (is_computer)
            {
                mBoard[r, c] = computer;
            }
            else
            {
                mBoard[r, c] = player;
            }
        }

        public void Reset()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    mBoard[i, j] = ' ';
                }
            }
            WinID = -1;
        }
        private bool DiagonalCrossed(char[,] board)
        {
            if (board[0, 0] == board[1, 1] &&
                board[1, 1] == board[2, 2] &&
                board[0, 0] != ' ')
            {
                WinID = 7;
                return (true);
            }

            if (board[0, 2] == board[1, 1] &&
                board[1, 1] == board[2, 0] &&
                board[0, 2] != ' ')
            {
                WinID = 6;
                return (true);
            }

            return (false);
        }
        private bool RowCrossed(char[,] board)
        {
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] == board[i, 1] &&
                    board[i, 1] == board[i, 2] &&
                    board[i, 0] != ' ')
                {
                    WinID = i;
                    return (true);
                }
            }
            return (false);
        }
        
        private bool ColumnCrossed(char[,] board)
        {
            for (int i = 0; i < 3; i++)
            {
                if (board[0, i] == board[1, i] &&
                    board[1, i] == board[2, i] &&
                    board[0, i] != ' ')
                {
                    WinID = 3 + i;
                    return (true);
                }
            }
            return (false);
        }
        bool IsGameOver(char [,] board)
        {
            return (RowCrossed(board) || ColumnCrossed(board) || DiagonalCrossed(board));
        }
        public bool IsGameOver()
        {
            return IsGameOver(mBoard);
        }

        public CellIndex FindNextMove()
        {
            int bestVal = -1000;
            CellIndex bestMove = new CellIndex();
            bestMove.Row = -1;
            bestMove.Col = -1;

            // Traverse all cells, evaluate minimax function
            // for all empty cells. And return the cell
            // with optimal value.
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    // Check if cell is empty
                    if (mBoard[i, j] == ' ')
                    {
                        // Make the move
                        mBoard[i, j] = computer;

                        // compute evaluation function for this
                        // move.
                        int moveVal = Minimax(mBoard, 0, false);

                        // Undo the move
                        mBoard[i, j] = ' ';

                        // If the value of the current move is
                        // more than the best value, then update
                        // best/
                        if (moveVal > bestVal)
                        {
                            bestMove.Row = i;
                            bestMove.Col = j;
                            bestVal = moveVal;
                        }
                    }
                }
            }

            Debug.Log("The value of the best Move is: " + bestVal);

            return bestMove;
        }
    }
}