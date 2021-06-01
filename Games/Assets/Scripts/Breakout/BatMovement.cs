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
        void Start()
        {
        }

        private void Update()
        {
            mx = mTouchField.TouchDist.x * Time.deltaTime;
            mVelocity.x += mx;
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
        }
    }
}
