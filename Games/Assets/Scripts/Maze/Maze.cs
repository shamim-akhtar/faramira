using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Maze
{
    public enum Directions
    {
        UP,
        RIGHT,
        DOWN,
        LEFT,
        //NONE,
    };
    public class Cell
    {
        public int x;
        public int y;
        public bool visited = false;
        private bool []flag = { true, true, true, true };

        public delegate void DelegateSetDirFlag(int x, int y, Directions dir, bool f);
        public DelegateSetDirFlag OnSetDirFlag;

        public Cell(int c, int r)
        {
            x = c;
            y = r;
        }

        public void SetDirFlag(Directions dir, bool f)
        {
            //if (dir == Directions.NONE) return;
            flag[(int)dir] = f;
            OnSetDirFlag?.Invoke(x, y, dir, f);
        }
    }

    private int _rows;
    private int _cols;

    private Cell[,] _cells;

    public Maze(int rows, int cols)
    {
        _rows = rows;
        _cols = cols;

        // initiaze all the cells.
        _cells = new Cell[_cols, _rows];
        for (var i = 0; i < _cols; i++)
        {
            for (var j = 0; j < _rows; j++)
            {
                _cells[i, j] = new Cell(i, j);
            }
        }
    }

    public Cell GetCell(int i, int j)
    {
        return _cells[i, j];
    }

    public int GetCellCount()
    {
        return _rows * _cols;
    }

    public List<Tuple<Directions, Cell>> GetNeighbours(int cx, int cy)
    {
        List<Tuple<Directions, Cell>> neighbours = new List<Tuple<Directions, Cell>>();
        foreach(Directions dir in Enum.GetValues(typeof(Directions)))
        {
            int x = cx;
            int y = cy;

            switch (dir)
            {
                case Directions.DOWN:
                    if (y < _rows - 1)
                    {
                        ++y;
                        if (!_cells[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Directions, Cell>(
                              Directions.DOWN,
                              _cells[x, y])
                            );
                        }
                    }
                    break;
                case Directions.RIGHT:
                    if (x < _cols - 1)
                    {
                        ++x;
                        if (!_cells[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Directions, Cell>(
                              Directions.RIGHT,
                              _cells[x, y])
                            );
                        }
                    }
                    break;
                case Directions.UP:
                    if (y > 0)
                    {
                        --y;
                        if (!_cells[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Directions, Cell>(
                              Directions.UP,
                              _cells[x, y])
                            );
                        }
                    }
                    break;
                case Directions.LEFT:
                    if (x > 0)
                    {
                        --x;
                        if (!_cells[x, y].visited)
                        {
                            neighbours.Add(new Tuple<Directions, Cell>(
                              Directions.LEFT,
                              _cells[x, y])
                            );
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        return neighbours;
    }

    public void RemoveCellWall(int x, int y, Directions dir)
    {
        //if (dir != Directions.NONE)
        //{
            Cell cell = GetCell(x, y);
            cell.SetDirFlag(dir, false);
        //}

        Directions opp = Directions.UP;//= Directions.NONE;
        switch (dir)
        {
            case Directions.UP:
                if (y > 0)
                {
                    opp = Directions.DOWN;
                    --y;
                }
                break;
            case Directions.RIGHT:
                if (x < _cols - 1)
                {
                    opp = Directions.LEFT;
                    ++x;
                }
                break;
            case Directions.DOWN:
                if (y < _rows - 1)
                {
                    opp = Directions.UP;
                    ++y;
                }
                break;
            case Directions.LEFT:
                if (x > 0)
                {
                    opp = Directions.RIGHT;
                    --x;
                }
                break;
        }

        //if (opp != Directions.NONE)
        //{
            Cell cell1 = GetCell(x, y);
            cell1.SetDirFlag(opp, false);
        //}
    }
}
