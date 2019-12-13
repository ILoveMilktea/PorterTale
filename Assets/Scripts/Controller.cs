using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{
    public bool controllable = true;

    public float speed = 7.0f;
    public float mouse_speed;
    public float jumpSpeed = 6.0f;
    public float gravity = 20.0f;


    public float smooth = 2.0F;
    public float tiltAngle = 30.0F;

    //Rotation Sensitivity

    public float minAngle = -90.0f;
    public float maxAngle = 90.0f;

    //Rotation Value
    public float yRotate = 0.0f;
    public float xRotate = 0.0f;

    // Zoom
    float minFov = 15f;
    float maxFov = 90f;
    public float zoom_speed = 20f;

   


    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;




    private GameObject cam;
    private float fov;

 

    // Use this for initialization
    void Start()
    {
       controller = GetComponent<CharacterController>();
       cam = GameObject.Find("Cam");
       fov = Camera.main.fieldOfView;

    


    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded && controllable)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;

            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        // Rotate Camera



        xRotate += Input.GetAxis("Mouse X")*mouse_speed;
        yRotate -= Input.GetAxis("Mouse Y")*mouse_speed;

        yRotate = Mathf.Clamp(yRotate, minAngle, maxAngle);


        controller.transform.eulerAngles = new Vector3(0, xRotate, 0.0f);

        cam.transform.eulerAngles = new Vector3(yRotate,xRotate, 0.0f);

     
        fov += Input.GetAxis("Mouse ScrollWheel") * zoom_speed;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;


        if (Input.GetMouseButtonDown(0))
            Cursor.lockState = CursorLockMode.Locked;

    }



}
