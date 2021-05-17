using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Puzzle
{
    public class State
    {
        public int[] Arr
        {
            get;
        }
        public int NumRowsOrCols
        {
            get;
        }
        private int _emptyTileIndex;

        public int GetEmptyTileIndex()
        {
            return _emptyTileIndex;
        }

        public State(int rows_or_cols)
        {
            NumRowsOrCols = rows_or_cols;
            Arr = new int[NumRowsOrCols * NumRowsOrCols];
            for (int i = 0; i < Arr.Length; ++i)
            {
                Arr[i] = i;
            }
            //_emptyTileIndex = 0;
            _emptyTileIndex = Arr.Length - 1;
        }

        public State(int[] arr)
        {
            NumRowsOrCols = (int)System.Math.Sqrt(arr.Length);

            Arr = new int[NumRowsOrCols * NumRowsOrCols];
            for (int i = 0; i < Arr.Length; ++i)
            {
                Arr[i] = arr[i];
                if (arr[i] == (Arr.Length - 1)) _emptyTileIndex = i;
            }
        }

        public State(State other)
        {
            NumRowsOrCols = other.NumRowsOrCols;
            _emptyTileIndex = other._emptyTileIndex;
            Arr = new int[NumRowsOrCols * NumRowsOrCols];
            other.Arr.CopyTo(Arr, 0);
        }

        public static bool Equals(State a, State b)
        {
            for (int i = 0; i < a.Arr.Length; i++)
            {
                if (a.Arr[i] != b.Arr[i]) return false;
            }
            return true;
        }

        public int FindEmptyTileIndex()
        {
            for (int i = 0; i < Arr.Length; i++)
                if (Arr[i] == Arr.Length - 1) return i;
            return Arr.Length;
        }

        //public void Randomize()
        //{
        //    new System.Random(10).Shuffle(Arr);
        //    for (int i = 0; i < Arr.Length; i++)
        //    {
        //        if (Arr[i] == (Arr.Length - 1)) _emptyTileIndex = i;
        //    }
        //}

        public void SwapWithEmpty(int index)
        {
            int tmp = Arr[index];
            Arr[index] = Arr[_emptyTileIndex];
            Arr[_emptyTileIndex] = tmp;
            _emptyTileIndex = index;
        }

        public int GethammingCost()
        {
            int cost = 0;
            for (int i = 0; i < Arr.Length; ++i)
            {
                if (Arr[i] == Arr.Length - 1) continue;
                if (Arr[i] != i + 1) cost += 1;
            }
            return cost;
        }

        public int GetManhattanCost()
        {
            int cost = 0;
            for (int i = 0; i < Arr.Length; ++i)
            {
                int v = Arr[i];
                if (v == Arr.Length - 1) continue;

                // actual index of v should be v-1
                //v = v - 1; (only when empty tile is at index 0. Not appicable when empty slot is at the last index
                int gx = v % NumRowsOrCols;
                int gy = v / NumRowsOrCols;

                int x = i % NumRowsOrCols;
                int y = i / NumRowsOrCols;

                int mancost = System.Math.Abs(x - gx) + System.Math.Abs(y - gy);
                cost += mancost;
            }
            return cost;
        }
    };
}