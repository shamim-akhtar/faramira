using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
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

    public void AddForceToBall()
    {
        // random positive y and -1 to +1 x.
        float x = Random.Range(-1.0f, 1.0f) * 30.0f;
        float y = 30.0f;
        mRb2D.AddForce(new Vector2(x, y), ForceMode2D.Impulse);
    }
}
