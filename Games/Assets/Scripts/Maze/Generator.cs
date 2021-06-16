using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Maze
{

    public class Generator : MonoBehaviour
    {
        public int START_X = 0;
        public int START_Y = 0;

        public int rows = 15;
        public int cols = 11;
        public GameObject mCellPrefab;

        public float generationTimeStep = 0.01f;
        GameObject[,] mCellGameObjs;
        public GameObject mMouse;

        public Maze maze;

        Stack<Maze.Cell> _stack = new Stack<Maze.Cell>();
        int mousex = 0;
        int mousey = 0;

        public bool mMazeGenerated = false;

        // Start is called before the first frame update
        void Start()
        {
            //UnityEngine.Random.InitState(100);

            START_Y = -rows / 2;
            START_X = -cols / 2;
            maze = new Maze(rows, cols);
            mCellGameObjs = new GameObject[cols, rows];
            for (int i = 0; i < cols; ++i)
            {
                for (int j = 0; j < rows; ++j)
                {
                    GameObject obj = Instantiate(mCellPrefab);
                    obj.transform.parent = transform;
                    Maze.Cell cell = maze.GetCell(i, j);
                    cell.OnSetDirFlag = OnCellSetDirFlag;
                    obj.transform.position = new Vector3(START_X + cell.x, START_Y + cell.y, 1.0f);
                    mCellGameObjs[i, j] = obj;
                }
            }
            CreateNewMaze();
        }

        public void CreateNewMaze()
        {
            SetMousePosition(0, 0);

            _stack.Push(maze.GetCell(mousex, mousey));

            maze.RemoveCellWall(0, 0, Maze.Directions.LEFT);
            maze.RemoveCellWall(cols - 1, rows - 1, Maze.Directions.RIGHT);
            StartCoroutine(Coroutine_Generate(generationTimeStep));
        }

        public void HighlightCell(int i, int j, bool flag)
        {
            mCellGameObjs[i, j].transform.GetChild(8).gameObject.SetActive(flag);
        }

        public void RemoveAllHightlights()
        {

            for (int i = 0; i < cols; ++i)
            {
                for (int j = 0; j < rows; ++j)
                {
                    mCellGameObjs[i, j].transform.GetChild(8).gameObject.SetActive(false);
                }
            }
        }

        public void OnCellSetDirFlag(int x, int y, Maze.Directions dir, bool f)
        {
            mCellGameObjs[x, y].transform.GetChild((int)dir).gameObject.SetActive(f);
        }

        // coroutine to swap tiles smoothly
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


        void SetMousePosition(int x, int y)
        {
            //mMouse.transform.position = new Vector3(START_X + x, START_Y + y, 0.0f);
            Vector3 newPos = new Vector3(START_X + x, START_Y + y, 0.0f);
            IEnumerator coroutine = Coroutine_MoveOverSeconds(mMouse, newPos, 0.1f);
            StartCoroutine(coroutine);
            mousex = x;
            mousey = y;
        }

        bool GenerateStep()
        {
            if (_stack.Count == 0) return true;

            Maze.Cell c = _stack.Peek();

            var neighbours = maze.GetNeighboursNotVisited(c.x, c.y);

            if (neighbours.Count != 0)
            {
                var index = 0;
                if (neighbours.Count > 1)
                {
                    index = UnityEngine.Random.Range(0, neighbours.Count);
                }
                var item = neighbours[index];
                Maze.Cell neighbour = item.Item2;
                neighbour.visited = true;
                maze.RemoveCellWall(c.x, c.y, item.Item1);

                _stack.Push(neighbour);
            }
            else
            {
                _stack.Pop();
            }
            return false;
        }

        IEnumerator Coroutine_Generate(float t = 0.1f)
        {
            bool flag = false;
            while (!flag)
            {
                flag = GenerateStep();
                yield return null;
            }
            mMazeGenerated = true;
        }


        // Update is called once per frame
        void Update()
        {
        }
    }
}