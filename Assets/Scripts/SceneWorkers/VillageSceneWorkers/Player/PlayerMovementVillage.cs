using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementVillage : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Transform player;
    public float moveSpeed = 5f;
    public float smoothSeped = 10.0f;

    private float screenWidth;

    protected Vector2 touchPos;
    protected TouchState state;

    // Start is called before the first frame update
    void Start()
    {       
        StartCoroutine(PlayerMove());

        //모바일 Touch로 조작할때
        screenWidth = Screen.width;        
    }   

    IEnumerator PlayerMove()
    {
        while(true)
        {
            //키보드 버전
            //float tmp = Input.GetAxis("Horizontal");
            //Debug.Log(tmp);
            //Vector3 moveDirection = new Vector3(tmp, 0, 0);
            //Move(moveDirection);

            //if (Input.GetTouch(0).position.x > screenWidth / 2)
            //{
            //    Move(new Vector3(1.0f, 0, 0));
            //}
            //else if (Input.GetTouch(0).position.x < screenWidth / 2)
            //{
            //    Move(new Vector3(-1.0f, 0, 0));
            //}
            if(state == TouchState.Begin || state == TouchState.Drag)
            {
                if(touchPos.x > screenWidth/2)
                {
                    Move(new Vector3(1.0f, 0, 0));
                }
                else
                {
                    Move(new Vector3(-1.0f, 0, 0));
                }
            }

            yield return new WaitForSeconds(0.02f);
        }

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
        state = TouchState.Drag;
    }

    // End touch on this gameobject
    public virtual void OnPointerUp(PointerEventData data)
    {
        touchPos = Vector2.zero;
        state = TouchState.End;
    }

    public void Move(Vector3 direction)
    {
        player.Translate(direction * moveSpeed * Time.deltaTime);
    }

   
}
