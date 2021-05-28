using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    namespace PathFinder
    {
        #region Class Coordinate
        /// <summary>
        /// This is a class that encapsulates simple 2D indices (x, y).
        /// </summary>
        public class Coordinate : IEquatable<Coordinate>
        {
            //private readonly Tuple<int, int> xy;
            private readonly int m_x;
            private readonly int m_y;

            public int X { get { return m_x; } }
            public int Y { get { return m_y; } }

            public Coordinate(int x, int y)
            {
                m_x = x;
                m_y = y;
            }

            public bool Equals(Coordinate other)
            {
                return m_x == other.m_x && m_y == other.m_y;
            }

            public override bool Equals(object other)
            {
                return Equals((Coordinate)other);
            }

            public override int GetHashCode()
            {
                // Why am I returning 0 for get hash code?
                // This is because it will be less efficient for such a simple
                // class with only two variables to compute the hash value.
                // we are just checking the individual x and y with the other.
                return 0;
            }
        }
        #endregion

        #region Class PathFinderMap
        public class PathFinderMap
        {
            #region Private Variables
            private int m_xNum;
            private int m_yNum;
            private float[,] m_costs;
            #endregion

            #region Properties
            public float[,] Costs
            {
                get
                {
                    return m_costs;
                }
                set
                {
                    m_costs = value;
                }
            }
            #endregion

            #region Constructors
            public PathFinderMap(int xNum, int yNum)
            {
                m_xNum = xNum;
                m_yNum = yNum;
                m_costs = new float[xNum, yNum];
                for (int i = 0; i < xNum; ++i)
                {
                    for (int j = 0; j < yNum; ++j)
                    {
                        m_costs[i, j] = 1.0f;
                    }
                }
            }
            #endregion

            #region Public Functions
            public bool WithinBounds(Coordinate loc)
            {
                return (loc.X >= 0) &&
                        (loc.X < m_xNum) &&
                        (loc.Y >= 0) &&
                        (loc.Y < m_yNum);
            }

            public bool CanPassThrough(Coordinate loc)
            {
                return WithinBounds(loc) && m_costs[loc.X, loc.Y] != -1;
            }
            public List<Coordinate> GetNeighbours(Coordinate loc)
            {
                List<Coordinate> neighbours = new List<Coordinate>();
                for (int i = 0; i < Directions.Length; ++i)
                {
                    Coordinate next = new Coordinate(loc.X + Directions[i].X, loc.Y + Directions[i].Y);
                    if (WithinBounds(next))
                    {
                        neighbours.Add(next);
                    }

                }
                return neighbours;
            }
            #endregion

            #region Static Variable Readonly
            public static readonly Coordinate[] Directions = new[]
            {
                new Coordinate(1, 0),
                new Coordinate(0, 1),
                new Coordinate(-1, 0),
                new Coordinate(0, -1)
            };
            #endregion
        }
        #endregion

        /// <summary>
        /// A simple class that holds the coordinate and the parent node from where it came.
        /// </summary>
        public class PathFinderNode
        {
            public PathFinderNode Parent { get; } = null;
            public Coordinate Location { get; }

            public PathFinderNode(Coordinate loc, PathFinderNode parent = null)
            {
                Parent = parent;
                Location = loc;
            }

            public bool IsGoal(Coordinate goal)
            {
                return Location.Equals(goal);
            }

            public static float GetManhattanDistance(Coordinate loc, Coordinate goal)
            {
                return Mathf.Abs(loc.X - goal.X) + Mathf.Abs(loc.Y - goal.Y);
            }
        }

        // class PriorityQueue
        // A priority queue is simply a list which is sorted
        // That means anytime I want I can get the lowest cost node/coordinate.
        public class PriorityQueue<T>
        {
            private List<Tuple<T, float>> m_items = new List<Tuple<T, float>>();

            public PriorityQueue()
            { }

            public void Add(T loc, float cost)
            {
                m_items.Add(new Tuple<T, float>(loc, cost));
            }


            //Tuple<T, float>
            public T GetAndRemove()
            {
                int index = 0;
                for (int i = 0; i < m_items.Count; ++i)
                {
                    if (m_items[i].Item2 < m_items[index].Item2)
                    {
                        index = i;
                    }
                }

                T item = m_items[index].Item1;
                m_items.RemoveAt(index);
                return item;
            }

            public int Count { get { return m_items.Count; } }
        }

        /// <summary>
        /// This is our AStar path finder class. This will find the 
        /// shortest path between two points (start, goal) using the
        /// a star algorithm.
        /// 
        /// 
        /// </summary>
        public class AStarPathFinder
        {
            private Coordinate m_start;
            private Coordinate m_goal;

            private PriorityQueue<PathFinderNode> openlist = new PriorityQueue<PathFinderNode>();
            private Dictionary<Coordinate, float> closedList = new Dictionary<Coordinate, float>();

            public Status m_status;


            private PathFinderNode m_currentNode = null;
            private PathFinderMap m_map;

            public PathFinderNode CurrentNode { get { return m_currentNode; } }

            public AStarPathFinder(Coordinate start, Coordinate goal, PathFinderMap map)
            {
                m_start = start;
                m_goal = goal;

                openlist.Add(new PathFinderNode(m_start), 0.0f);
                closedList[m_start] = 0.0f;
                m_map = map;
                m_status = Status.NOT_STARTED;
            }

            public enum Status
            {
                NOT_STARTED,
                RUNNING,
                SUCCESS,
                FAILURE,
            }

            /// <summary>
            /// One search step.
            /// 
            /// Usually in online tutorials this will be impemented as a complete search.
            /// But for us, since we are using Unity and we donot want the program to be stuck
            /// so we implement one step at a time.
            /// The main program must ensure that this function is repeatedly called 
            /// until the status returned is SUCCESS or FAILURE.
            /// </summary>
            /// <returns></returns>

            public Status CalculateNextStep()
            {
                if (openlist.Count > 0)
                {
                    m_currentNode = openlist.GetAndRemove();

                    if (m_currentNode.IsGoal(m_goal))
                    {
                        m_status = Status.SUCCESS;
                        return Status.SUCCESS;
                    }

                    List<Coordinate> neighbours = m_map.GetNeighbours(m_currentNode.Location);
                    for (int i = 0; i < neighbours.Count; ++i)
                    {
                        Coordinate n = neighbours[i];
                        // F = G + H
                        // G = cost so far
                        // H = estimated cost to Goal (Manhattan distance).
                        float G = closedList[m_currentNode.Location] + m_map.Costs[n.X, n.Y];

                        if (!closedList.ContainsKey(n) || G < closedList[n])
                        {
                            float H = PathFinderNode.GetManhattanDistance(n, m_goal);
                            float F = G + H;
                            closedList[n] = G;
                            openlist.Add(new PathFinderNode(n, m_currentNode), F);
                        }
                    }
                    m_status = Status.RUNNING;
                    return m_status;
                }
                // Total cost (F = G + H)
                // where G is the cost so far and H is the estimated cost to reach my goal.
                m_status = Status.FAILURE;
                return Status.FAILURE;
            }
        }
    }
}
