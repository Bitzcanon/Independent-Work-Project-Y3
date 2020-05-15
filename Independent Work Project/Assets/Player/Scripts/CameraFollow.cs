using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private FocusArea focusArea;

    private float currentLookAheadX;
    private float targetLookAheadX;
    private float lookAheadDirectionX;
    private float smoothLookVelocityX;
    private float smoothVelocityY;

    public PlayerController2D target;
    public Vector2 focusAreaSize;

    public float verticalOffset;
    public float lookAheadDistanceX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;

    struct FocusArea
    {
        public Vector2 center;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x * 0.5f;
            right = targetBounds.center.x + size.x * 0.5f;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            center = new Vector2((left + right) * 0.5f, (top + bottom) * 0.5f);
        }
        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            bottom += shiftY;
            top += shiftY;
            center = new Vector2((left + right) * 0.5f, (top + bottom) * 0.5f); //Update center location
            velocity = new Vector2(shiftX, shiftY);
        }
    }

    void Start()
    {
        focusArea = new FocusArea(target.GetComponent<BoxCollider2D>().bounds, focusAreaSize);
    }

    void LateUpdate()
    {
        focusArea.Update(target.GetComponent<BoxCollider2D>().bounds);

        Vector3 focusPosition = focusArea.center + Vector2.up * verticalOffset;

        if (focusArea.velocity.x != 0)
        {
            lookAheadDirectionX = Mathf.Sign(focusArea.velocity.x);
        }

        targetLookAheadX = lookAheadDirectionX * lookAheadDistanceX;
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
        focusPosition += Vector3.right * currentLookAheadX;
        transform.position = focusPosition + Vector3.forward * -10;
    }

    private void OnDrawGizmos() //For Debugging
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }
}
