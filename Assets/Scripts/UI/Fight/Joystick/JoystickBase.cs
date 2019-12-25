using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum TouchState
{
    None,
    Begin,
    Stay,
    Drag,
    End,
}

public class JoystickBase : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    [SerializeField]
    protected RectTransform border;
    [SerializeField]
    protected RectTransform handle;
    protected Canvas canvas;

    protected Vector2 touchPos;
    protected float handleMoveRange;

    protected TouchState state;

    protected bool handleLock;

    protected virtual void Start()
    {
        canvas = GetComponentInParent<Canvas>();

        touchPos = Vector2.zero;
        handleMoveRange = border.rect.width * 0.5f;

        handleLock = false;
    }

    // Gameobject touch
    public virtual void OnPointerDown(PointerEventData data)
    {
        touchPos = data.position;
        state = TouchState.Begin;
    }

    // Gameobject drag
    public virtual void OnDrag(PointerEventData data)
    {
        touchPos = data.position;
        Vector2 zeroPoint = new Vector2(border.position.x, border.position.y);
        Vector2 moveVector = touchPos - zeroPoint;
        
        MoveHandle(moveVector);
        state = TouchState.Drag;
    }

    // End touch on this gameobject
    public virtual void OnPointerUp(PointerEventData data)
    {
        touchPos = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        state = TouchState.End;
    }

    private void MoveHandle(Vector2 moveVector)
    {
        // canvas.scaleFactor -> Consider the aspect ratio in personal phone.
        if (moveVector.magnitude > handleMoveRange * canvas.scaleFactor)
        {
            moveVector.Normalize();
            handle.anchoredPosition = moveVector * handleMoveRange;
        }
        else
        {
            handle.position = touchPos;
        }
    }


    public void LockJoystick()
    {
        handleLock = true;
    }
    public void UnLockJoystick()
    {
        handleLock = false;
    }
}
