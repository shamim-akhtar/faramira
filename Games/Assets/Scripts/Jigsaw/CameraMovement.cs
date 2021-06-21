using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    public Camera mCamera;
    private Vector3 mDragPos;

    void Update()
    {
        if (!Jigsaw.sCameraPanning) return;
        // save position is worldspace.
        if(Input.GetMouseButtonDown(0))
        {
            mDragPos = mCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if(Input.GetMouseButton(0))
        {
            Vector3 diff = mDragPos - mCamera.ScreenToWorldPoint(Input.mousePosition);
            mCamera.transform.position += diff;
        }
    }
}
