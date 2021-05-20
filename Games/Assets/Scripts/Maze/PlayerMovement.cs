using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject mPlayer;
    public FixedTouchField mTouchField;
    public FixedJoystick mJoystick;
    public Generator mGenerator; 

    public enum MoveDirection
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        NONE,
    }
    MoveDirection moveDirection;

    Rigidbody2D rb;
    float mx, my;
    //float jx, jy;
    public float mSpeed = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = mPlayer.GetComponent<Rigidbody2D>();
        mx = 0.0f;
        my = 0.0f;
        //jx = 0.0f;
        //jy = 0.0f;

    }

    // Update is called once per frame
    void Update()
    {
        //mx = mTouchField.TouchDist.x * Time.deltaTime;
        //my = mTouchField.TouchDist.y * Time.deltaTime;
        mx = mJoystick.Horizontal * Time.deltaTime;
        my = mJoystick.Vertical * Time.deltaTime;

        //mx = mJoystick.Direction.x;
        //my = mJoystick.Direction.y;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Coroutine_Print(1.0f));
        }
    }

    private bool player_moving = false;
    // coroutine to swap tiles smoothly
    public IEnumerator Coroutine_MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        player_moving = true;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        player_moving = false;
        objectToMove.transform.position = end;
    }

    private void FixedUpdate()
    {
        float x = mPlayer.transform.position.x;
        float y = mPlayer.transform.position.y;

        int i = (int)Mathf.Floor(x + 0.5f) - mGenerator.START_X;
        int j = (int)Mathf.Floor(y + 0.5f) - mGenerator.START_Y;

        if (i < 0 || j < 0 || i > mGenerator.cols - 1 || j > mGenerator.rows - 1)
            return;

        Maze.Cell cell = mGenerator.maze.GetCell(i, j);

        if (player_moving) return;

        // note that UP and DOWN are opposite. Need to fix this issue.
        // althogh it is working, we need to be consistent.
        if (!cell.flag[0])
        {
            // can go down.
            if (j > 0 && my < 0.0f/* && Mathf.Abs(my) > Mathf.Abs(mx)*/)
            {
                Vector3 a = mPlayer.transform.position;
                Vector3 b = new Vector3(i + mGenerator.START_X, j + mGenerator.START_Y - 1, 0.0f);

                Debug.Log(a + ", " + b);
                StartCoroutine(Coroutine_MoveOverSeconds(mPlayer, 
                    new Vector3(
                        i + mGenerator.START_X,
                        j + mGenerator.START_Y - 1, 
                        0.0f), 
                    1.0f/mSpeed));
            }
        }
        if (!cell.flag[1])
        {
            // can go right.
            if (i < mGenerator.cols - 1 && mx > 0.0f/* && Mathf.Abs(mx) > Mathf.Abs(my)*/)
            {
                StartCoroutine(Coroutine_MoveOverSeconds(mPlayer, new Vector3(i + mGenerator.START_X + 1,
                    j + mGenerator.START_Y, 0.0f), 1.0f / mSpeed));
            }
        }
        if (!cell.flag[2])
        {
            // can go up
            if (j < mGenerator.rows - 1 && my > 0.0f/* && Mathf.Abs(my) > Mathf.Abs(mx)*/)
            {
                StartCoroutine(Coroutine_MoveOverSeconds(mPlayer, new Vector3(i + mGenerator.START_X,
                    j + mGenerator.START_Y + 1, 0.0f), 1.0f / mSpeed));
            }
        }
        if (!cell.flag[3])
        {
            // can go left.
            if (i > 0 && mx < 0.0f/* && Mathf.Abs(mx) > Mathf.Abs(my)*/)
            {
                StartCoroutine(Coroutine_MoveOverSeconds(mPlayer, new Vector3(i + mGenerator.START_X - 1,
                    j + mGenerator.START_Y, 0.0f), 1.0f / mSpeed));
            }
        }

        if(cell.x == mGenerator.cols - 1 && cell.y == mGenerator.rows - 1)
        {
            Debug.Log("Win");

            StartCoroutine(Coroutine_MoveOverSeconds(mPlayer, new Vector3(i + mGenerator.START_X + 1,
                j + mGenerator.START_Y, 0.0f), 1.0f / mSpeed));
        }
    }


    IEnumerator Coroutine_Print(float t)
    {
        while (true)
        {
            mGenerator.RemoveAllHightlights();

            float x = mPlayer.transform.position.x + 0.5f;
            float y = mPlayer.transform.position.y + 0.5f;
            int i = (int)Mathf.Floor(x) - mGenerator.START_X;
            int j = (int)Mathf.Floor(y) - mGenerator.START_Y;


            if (!(i < 0 || j < 0 || i > mGenerator.cols - 1 || j > mGenerator.rows - 1))
            {
                Maze.Cell cell = mGenerator.maze.GetCell(i, j);
                Debug.Log(i + ", " + j + " | " + cell.flag[0] + ", " + cell.flag[1] + ", " + cell.flag[2] + ", " + cell.flag[3]);
                //Debug.Log(i + ", " + j);

                mGenerator.HighlightCell(i, j, true);
            }

            yield return new WaitForSeconds(t);
        }
    }
}
