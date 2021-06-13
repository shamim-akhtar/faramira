using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;

namespace Maze
{
    //
    public class MazePathFinder : MonoBehaviour
    {
        public GameObject mNpc;
        public float mSpeed = 2.0f;

        [HideInInspector]
        public Generator mGenerator;

        private StatusType mStatus = StatusType.NOT_STARTED;

        public StatusType Status { get { return mStatus; } }

        private void Start()
        {
            
        }

        private static bool IsCellInList(Maze.Cell state, List<Node<Maze.Cell>> li)
        {
            int i = 0;
            for (; i < li.Count; ++i)
            {
                if (state.x == li[i].Data.x && state.y == li[i].Data.y)
                    return true;
            }
            return false;
        }

        public enum StatusType
        {
            NOT_STARTED,
            RUNNING,
            SUCCESS,
            FAILURE,
        }

        public delegate void OnReachGoal(MazePathFinder pf);
        public OnReachGoal onReachGoal;

        public void FindPath(Maze.Cell start, Maze.Cell goal)
        {
            StartCoroutine(Coroutine_FindPath(start, goal));
        }

        #region PATH_FINDING
        private IEnumerator Coroutine_FindPath(Maze.Cell start, Maze.Cell goal)
        {
            Maze.Cell c = start;

            PriorityQueue<Maze.Cell> openlist = new PriorityQueue<Maze.Cell>();
            List<Node<Maze.Cell>> closedlist = new List<Node<Maze.Cell>>();

            Node<Maze.Cell> root = new Node<Maze.Cell>(start, 0, null);
            root.Parent = null;

            openlist.Add(root);
            closedlist.Add(root);

            List<Node<Maze.Cell>> astarSolution = new List<Node<Maze.Cell>>();

            mStatus = StatusType.RUNNING;
            while (openlist.Count > 0 && mStatus != StatusType.SUCCESS)
            {
                if(openlist.Count == 0)
                {
                    mStatus = StatusType.FAILURE;
                    break;
                }
                // sort the openlist and get the node with the least cost.
                // in our case the PriorityQueue data structure does the work for us.
                Node<Maze.Cell> current = openlist.GetAndRemoveTop();


                if (current.Data.x == goal.x && current.Data.y == goal.y)
                {
                    // fil the solution.
                    Node<Maze.Cell> s = current;
                    do
                    {
                        astarSolution.Add(s);
                        s = s.Parent;
                    } while (s != null);

                    mStatus = StatusType.SUCCESS;
                }

                List<Tuple<Maze.Directions, Maze.Cell>> neighbors = mGenerator.maze.GetNeighbours(current.Data.x, current.Data.y);

                foreach (Tuple<Maze.Directions, Maze.Cell> next in neighbors)
                {
                    if (!IsCellInList(next.Item2, closedlist))
                    {
                        int cost = 0;
                        if (current.Data.flag[(int)next.Item1])
                        {
                            // no go region.
                            cost = 1000;
                        }
                        else
                        {
                            int G = cost + current.Cost;
                            int H = Mathf.Abs(goal.x - next.Item2.x) + Mathf.Abs(goal.y - next.Item2.y);
                            int F = G + H;
                            Node<Maze.Cell> n = new Node<Maze.Cell>(next.Item2, F, current);

                            openlist.Add(n);
                            closedlist.Add(n);
                        }
                    }
                }
                yield return null;
            }
            yield return StartCoroutine(MoveToGoal(astarSolution, mNpc));
        }
        #endregion

        public IEnumerator Coroutine_MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
        {
            float elapsedTime = 0;
            Vector3 startingPos = objectToMove.transform.position;
            while (elapsedTime < seconds)
            {
                objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            objectToMove.transform.position = end;
        }

        private IEnumerator MoveToGoal(List<Node<Maze.Cell>> cells, GameObject obj)
        {
            for (int i = cells.Count - 1; i >= 0; --i)
            {
                yield return StartCoroutine(Coroutine_MoveOverSeconds(obj,
                    new Vector3(
                        cells[i].Data.x + mGenerator.START_X,
                        cells[i].Data.y + mGenerator.START_Y,
                        0.0f),
                    1.0f / mSpeed));
            }
            onReachGoal?.Invoke(this);
        }
    }
}
