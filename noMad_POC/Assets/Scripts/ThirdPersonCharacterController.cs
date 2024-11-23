using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ThirdPersonCharacterController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;


    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravity = -9.81f;

    

    [Header("Camera Parameters")]
    public Transform cam;
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private CharacterController characterController;
    private Camera mainCamera;
    private PlayerInputHandler inputHandler;

    private Vector3 currentVelocity;
   

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        inputHandler = PlayerInputHandler.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

    }

    void HandleMovement()
    {
        float speed = walkSpeed;

        Vector3 inputDirection = new Vector3(inputHandler.MoveInput.x, 0f, inputHandler.MoveInput.y).normalized; //MoveInput.y because it's a vector 2 so the y is actually the z
        

        HandleJumping();

        if (inputDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;



            characterController.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }

    void HandleJumping()
    {

        if (characterController.isGrounded) 
        {
            
            currentVelocity.y = -2f;

            if (inputHandler.JumpTriggered)
            {

                currentVelocity.y = jumpForce;
            }
        
        }
        else
        {
            currentVelocity.y -= gravity * Time.deltaTime;
        }
        
        characterController.Move(currentVelocity * Time.deltaTime);
    }

   

}
