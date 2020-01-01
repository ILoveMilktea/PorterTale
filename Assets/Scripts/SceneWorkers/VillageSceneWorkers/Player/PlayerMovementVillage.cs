using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementVillage : MonoBehaviour
{  
    public float moveSpeed = 5f;
    public float smoothSeped = 10.0f;

    private float screenWidth;

    // Start is called before the first frame update
    void Start()
    {       
        StartCoroutine(PlayerMove());

        //모바일 Touch로 조작할때
        screenWidth = Screen.width;        
    }   

    IEnumerator PlayerMove()
    {
        int i = 0;
        while(true)
        {
            //키보드 버전
            float tmp = Input.GetAxis("Horizontal");
            Debug.Log(tmp);
            Vector3 moveDirection = new Vector3(tmp, 0, 0);
            Move(moveDirection);

            //모바일 터치버전
            //if(Input.GetTouch(i).position.x>screenWidth/2)
            //{
            //    Move(new Vector3(1.0f,0,0));
            //}
            //else if(Input.GetTouch(i).position.x<screenWidth/2)
            //{
            //    Move(new Vector3(-1.0f, 0, 0));
            //}           

            yield return new WaitForSeconds(0.02f);
        }

    }

    public void Move(Vector3 direction)
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

   
}
