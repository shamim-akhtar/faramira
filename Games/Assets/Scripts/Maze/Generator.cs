using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Generator : MonoBehaviour
{
    private int START_X = 0;
    private int START_Y = 0;

    public int rows = 15;
    public int cols = 11;
    public GameObject mCellPrefab;
    public GameObject mMousePrefab;

    public float generationTimeStep = 0.01f;
    GameObject[,] mCellGameObjs;
    GameObject mMouse;
    Maze maze;

    Stack<Maze.Cell> _stack = new Stack<Maze.Cell>();
    int mousex = 0;
    int mousey = 0;
    // Start is called before the first frame update
    void Start()
    {

        START_Y = -rows / 2;
        START_X = -cols / 2;
        maze = new Maze(rows, cols);
        mCellGameObjs = new GameObject[cols, rows];
        for (int i = 0; i < cols; ++i)
        {
            for (int j = 0; j < rows; ++j)
            {
                GameObject obj = Instantiate(mCellPrefab);
                Maze.Cell cell = maze.GetCell(i, j);
                cell.OnSetDirFlag = OnCellSetDirFlag;
                obj.transform.position = new Vector3(START_X + cell.x, START_Y + cell.y, 1.0f);
                mCellGameObjs[i,j] = obj;
            }
        }

        mMouse = Instantiate(mMousePrefab);
        SetMousePosition(0, 0);
        //mMouse.transform.position = new Vector3(START_X, START_Y, 0.0f);

        _stack.Push(maze.GetCell(mousex, mousey));
        //StartCoroutine(Coroutine_Generate());

        maze.RemoveCellWall(0, 0, Maze.Directions.LEFT);
        maze.RemoveCellWall(cols-1, rows-1, Maze.Directions.RIGHT);
        StartCoroutine(Coroutine_Generate(generationTimeStep));
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
        //SetMousePosition(c.x, c.y);

        var neighbours = maze.GetNeighbours(c.x, c.y);

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
            //SetMousePosition(neighbour.x, neighbour.y);
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
        while(!flag)
        {
            flag = GenerateStep();
            //yield return new WaitForSeconds(t);
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartCoroutine(Coroutine_Generate(generationTimeStep));
        //    //GenerateStep();
        //}
    }
}
