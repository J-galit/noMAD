using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ThirdPersonCharacterController : MonoBehaviour
{
    [SerializeField] private GameObject adaptationsShop;
    [SerializeField] private GameObject jumpBoostButton;
    [SerializeField] private GameObject speedBoostButton;
    [SerializeField] private GameObject smallSizeButton;

    [SerializeField] private GameObject maxAdaptationErrorUI;


    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Speed Adaptation Parameters")]
    [SerializeField] private float speedBoostMultiplier;
    [SerializeField] private bool isSpeedBoostActive;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Jump Adaptation Parameters")]
    [SerializeField] private float jumpBoostMultiplier;
    [SerializeField] private bool isJumpBoostActive;

    [Header("Misc. Adaptations")]
    [SerializeField] private bool isSmallerSizeActive;
    

    [Header("Camera Parameters")]
    public Transform cam;
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private CharacterController characterController;
    private Camera mainCamera;
    private PlayerInputHandler inputHandler;

    private int maxAdaptations = 2;
    private int currentAdaptations;

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
        if (isSpeedBoostActive)
        {
            speed = walkSpeed * speedBoostMultiplier;
        }
        else
            speed = walkSpeed;

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

        HandleShopping();
    }

    void HandleJumping()
    {

        if (characterController.isGrounded) 
        {
            
            currentVelocity.y = -2f;

            if (inputHandler.JumpTriggered)
            {
                if(isJumpBoostActive == true)
                {
                    currentVelocity.y = jumpForce * jumpBoostMultiplier;
                }
                else
                    currentVelocity.y = jumpForce;
            }
        
        }
        else
        {
            currentVelocity.y -= gravity * Time.deltaTime;
        }
        
        characterController.Move(currentVelocity * Time.deltaTime);
    }

   void HandleShopping()
    {

        if (inputHandler.ShopTriggered == true)
        {
            if (adaptationsShop.activeSelf == false)
            {
                adaptationsShop.SetActive(true);
            }
            else if (adaptationsShop.activeSelf == true)
            {
                adaptationsShop.SetActive(false);
            }
        }
    }

    public void JumpBoostButtonHandler()
    {
        if(isJumpBoostActive == false && currentAdaptations < maxAdaptations)
        {
            isJumpBoostActive = true;
            jumpBoostButton.SetActive(true);
            currentAdaptations++;
        }
        else if(isJumpBoostActive == true)
        {
            isJumpBoostActive = false;
            jumpBoostButton.SetActive(false);
            currentAdaptations--;
        }
        else
        {
            StartCoroutine(MaxAdaptationCoroutine());
        }
    }

    public void SpeedBoostButtonHandler()
    {
        if (isSpeedBoostActive == false && currentAdaptations < maxAdaptations)
        {
            isSpeedBoostActive = true;
            speedBoostButton.SetActive(true);
            currentAdaptations++;
        }
        else if (isSpeedBoostActive == true)
        {
            isSpeedBoostActive = false;
            speedBoostButton.SetActive(false);
            currentAdaptations--;
        }
        else
        {
            StartCoroutine(MaxAdaptationCoroutine());
        }
    }

    public void SmallerSizeButtonHandler()
    {
        if (isSmallerSizeActive == false && currentAdaptations < maxAdaptations)
        {
            isSmallerSizeActive = true;
            smallSizeButton.SetActive(true);
            this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            currentAdaptations++;
        }
        else if (isSmallerSizeActive == true)
        {
            isSmallerSizeActive = false;
            smallSizeButton.SetActive(false);
            this.transform.localScale = Vector3.one;
            currentAdaptations--;
        }
        else
        {
            StartCoroutine(MaxAdaptationCoroutine());
        }
    }

    IEnumerator MaxAdaptationCoroutine()
    {
        maxAdaptationErrorUI.SetActive(true);
        yield return new WaitForSeconds(1f);
        maxAdaptationErrorUI.SetActive(false);

    }
}
