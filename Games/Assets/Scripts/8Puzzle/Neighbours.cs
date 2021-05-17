using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Puzzle
{
    // Neighbours class.
    /// this is the class that creates and holds the graph relationship for teh empty slot.
    /// note that for this simple configuration the graph is hardcoded.
    public class Neighbours
    {
        private Dictionary<int, int[]> _edges = new Dictionary<int, int[]>();
        private static Neighbours instance = null;

        public Neighbours()
        {
        }

        public static Neighbours Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Neighbours();
                }
                return instance;
            }
        }

        public int[] GetNeighbors(int id)
        {
            return _edges[id];
        }

        public void CreateGraphForNPuzzle(int rowsOrCols)
        {
            for(int i = 0; i < rowsOrCols; i++)
            {
                for(int j = 0; j < rowsOrCols; j++)
                {
                    int index = i * rowsOrCols + j;
                    List<int> li = new List<int>();
                    if (i - 1 >= 0) li.Add((i - 1)* rowsOrCols + j);
                    if (i + 1 < rowsOrCols) li.Add((i + 1)*rowsOrCols +j);
                    if (j - 1 >= 0) li.Add(i*rowsOrCols + j - 1);
                    if (j + 1 < rowsOrCols) li.Add(i* rowsOrCols + j + 1);

                    _edges[index] = li.ToArray();
                }
            }
        }
    };
}