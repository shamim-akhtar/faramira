using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SplitTile : MonoBehaviour
{
    public Vector2Int mIndex;
    public SplitImage.Direction[] mDirections = new SplitImage.Direction[4]; //0 = UP, 1 = RIGHT, 2 = BOTTOM, 3 = LEFT

    private Vector3 offset;

    public Vector2 ShadowOffset = new Vector2(2.0f, -2.0f);
    public Material ShadowMaterial;
    SpriteRenderer shadowSpriteRenderer = null;
    public delegate void DelegateOnSetCorrectPosition(SplitTile tile);
    public DelegateOnSetCorrectPosition mOnSetCorrectPosition;


    private Vector3 GetCorrectPosition()
    {
        return new Vector3(mIndex.x * 100.0f, mIndex.y * 100.0f, 0.0f);
    }

    public SpriteRenderer mSpriteRenderer = null;

    // Start is called before the first frame update
    void Start()
    {
        //create a new gameobject to be used as drop shadow
        GameObject shadowGameobject = new GameObject("Shadow");
        shadowGameobject.transform.SetParent(this.transform);
        shadowGameobject.transform.localPosition = (Vector3)ShadowOffset;

        //create a new SpriteRenderer for Shadow gameobject
        shadowSpriteRenderer = shadowGameobject.AddComponent<SpriteRenderer>();

        //set the shadow gameobject's sprite to the original sprite
        shadowSpriteRenderer.sprite = mSpriteRenderer.sprite;
        //set the shadow gameobject's material to the shadow material we created
        shadowSpriteRenderer.material = ShadowMaterial;

        //update the sorting layer of the shadow to always lie behind the sprite
        shadowSpriteRenderer.sortingLayerName = "DropShadow";
        shadowSpriteRenderer.sortingOrder = mSpriteRenderer.sortingOrder;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject() || enabled == false)
        {
            return;
        }

        Jigsaw.sCameraPanning = false;

        Jigsaw.sTilesSorting.BringToTop(this);

        offset = transform.position - Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
    }

    void OnMouseDrag()
    {
        if (EventSystem.current.IsPointerOverGameObject() || enabled == false)
        {
            return;
        }
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }
    void OnMouseUp()
    {
        if (EventSystem.current.IsPointerOverGameObject() || enabled == false)
        {
            return;
        }
        Jigsaw.sCameraPanning = true;

        if (IsInCorrectPosition())
        {
            transform.position = GetCorrectPosition();
            mOnSetCorrectPosition?.Invoke(this);
        }
    }

    public bool IsInCorrectPosition()
    {
        float distsq = (transform.position - GetCorrectPosition()).sqrMagnitude;
        if (distsq < 500.0f)
        {
            return true;
        }
        return false;
    }

    public void SetRenderOrder(int order)
    {
        mSpriteRenderer.sortingOrder = order;
        if (shadowSpriteRenderer != null)
        {
            shadowSpriteRenderer.sortingOrder = order;
        }

        //set the z value so that selection/raycast selects the top sprite.
        Vector3 p = mSpriteRenderer.gameObject.transform.position;
        p.z = -order / 10.0f;
        mSpriteRenderer.gameObject.transform.position = p;
    }
}
