using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickKeyboard : MonoBehaviour
{
    Vector3 movevector;
    bool keyboardmode = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            keyboardmode = !keyboardmode;
            Debug.Log("keyboard mode" + keyboardmode.ToString());

        }

        if(keyboardmode)
        {
            movevector = Vector3.zero;

            if (Input.GetKey(KeyCode.A))
            {
                movevector += Vector3.left;
            }

            if (Input.GetKey(KeyCode.W))
            {
                movevector += Vector3.forward;
            }

            if (Input.GetKey(KeyCode.D))
            {
                movevector += Vector3.right;
            }

            if (Input.GetKey(KeyCode.S))
            {
                movevector += Vector3.back;
            }

            if (movevector != Vector3.zero)
            {
                FightSceneController.Instance.MovePlayer(movevector, 0.9f);
            }
            else
            {
                FightSceneController.Instance.StopPlayer();
            }
        }
    }
}
