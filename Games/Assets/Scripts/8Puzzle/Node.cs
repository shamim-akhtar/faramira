using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Puzzle
{

    // class Node
    /// <summary>
    /// A class to hold the State of the puzzle. This is used to create the solution.
    /// Node not only holds the state of the puzzle but also the cost associated with the specific state.
    /// </summary>
    public class Node
    {
        public State State
        {
            get;
        }

        public Node Parent
        {
            get;
            set;
        }

        public int Cost
        {
            get;
        }

        public int Depth
        {
            get;
        }

        public Node(State state, int depth = 0, Node parent = null)
        {
            State = state;
            Cost = /*_state.GethammingCost() + */State.GetManhattanCost() + depth;
            Parent = parent;
            Depth = depth;
        }

        // comparison operator based on the cost of the nodes. 
        // This can be used for sorting nodes based on the cost of 
        // the state that it holds.
        public static bool operator >(Node n1, Node n2)
        {
            return n1.Cost > n2.Cost;
        }

        public static bool operator <(Node n1, Node n2)
        {
            return n1.Cost < n2.Cost;
        }

        // print the node into the Debug log.
        public void Print(int lineNum)
        {
            StringWriter str = new StringWriter();

            str.Write(lineNum + " - ");
            str.Write("Node { ");
            for (int i = 0; i < State.Arr.Length; ++i)
            {
                str.Write(State.Arr[i]);
            }
            str.Write(" | D: " + Depth + ", MD: " + Cost + " }");
            //Debug.Log(str.ToString());
        }
    };
}