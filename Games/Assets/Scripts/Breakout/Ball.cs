using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakout
{

    public class Ball : MonoBehaviour
    {
        public Game mGame;
        private Rigidbody2D mRb2D;

        // Start is called before the first frame update
        void Start()
        {
            mRb2D = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AddRandomForceToBall()
        {
            // random positive y and -1 to +1 x.
            float x = 0.0f; // Random.Range(-1.0f, 1.0f) * 10.0f;
            float y = 15.0f;
            mRb2D.AddForce(new Vector2(x, y), ForceMode2D.Impulse);

            mGame.SetState(GameState.StateID.PLAYING);
        }

        public void AddForceToBall(Vector2 f, ForceMode2D mode)
        {
            //Debug.Log("Added force: " + f.x + ", " + f.y);
            mRb2D.AddForce(f, mode);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Brick brick = collision.gameObject.GetComponent<Brick>();
            if (brick == null) return;

            mGame.BallHitBrick(brick);
        }
    }
}