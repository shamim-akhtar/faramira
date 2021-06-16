using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakout
{

    public class BatMovement : MonoBehaviour
    {
        public FixedTouchField mTouchField;
        public FixedJoystick mJoystick;

        private Vector2 mVelocity = new Vector2(0.0f, 0.0f);
        private float mx = 0.0f;
        private float width = 0.0f;

        public Ball mBall;

        void Start()
        {
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            width = 2.0f * col.bounds.extents.x;
        }

        private void Update()
        {
            //mx = mTouchField.TouchDist.x * Time.deltaTime;
            //mVelocity.x += mx;
            //GetComponent<Rigidbody2D>().MovePosition(mVelocity);

            Vector3 pos = transform.position;
            pos.x += mx;

            if (pos.x >= 4.0f) pos.x = 4.0f;
            if (pos.x <= -4.0f) pos.x = -4.0f;

            transform.position = pos;
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            //collision.gameObject.GetComponent<Rigidbody2D>();
            ContactPoint2D cp = collision.GetContact(0);
            float diff = 5.0f * (cp.point.x - transform.position.x) / width;
            //Debug.Log("Diff: " + diff);

            if(collision.gameObject.tag == "Ball")
            {
                mBall.AddForceToBall(new Vector2(diff, 0.0f), ForceMode2D.Impulse);
            }
        }
        public Vector3 offset = Vector3.zero;
        public bool playing = false;
        void OnMouseDown()
        {
            if (playing)
            {
                offset = transform.position - Camera.main.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, 0.0f, 0.0f));
            }
        }

        void OnMouseDrag()
        {
            if (playing)
            {
                Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, 0.0f, 0.0f);
                Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
                transform.position = curPosition;
            }
        }
        void OnMouseUp()
        {
            //float distsq = (transform.position - GetCorrectPosition()).sqrMagnitude;
            ////Debug.Log("Dist Sqr: " + distsq.ToString());
            //if (distsq < 400.0f)
            //{
            //    transform.position = GetCorrectPosition();
            //}
            //mSpriteRenderer.sortingOrder = 0;
        }
    }
}
