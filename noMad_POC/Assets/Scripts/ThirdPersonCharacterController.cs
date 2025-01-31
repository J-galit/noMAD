using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


//this script is gonna be so fucking long
public class ThirdPersonCharacterController : MonoBehaviour
{
    [SerializeField] private GameObject adaptationsShop;
    [SerializeField] private GameObject jumpBoostButton;
    [SerializeField] private GameObject speedBoostButton;
    [SerializeField] private GameObject smallSizeButton;
    [SerializeField] private GameObject largeSizeButton;
    [SerializeField] private GameObject largeAttackButton;

    [SerializeField] private GameObject maxAdaptationErrorUI;

    private int maxAdaptations = 2;
    private int currentAdaptations;

    private UICurrency _UICurrency;

    //Adaptation costs
    private int jumpBoostCost = 100;
    private bool isJumpBoostOwned;
    private int speedBoostCost = 100;
    private bool isSpeedBoostOwned;
    private int smallerSizeCost = 100;
    private bool isSmallerSizeOwned;
    private int largerSizeCost = 100;
    private bool isLargerSizeOwned;
    private int largerAttackCost = 100;
    private bool isLargerAttackOwned;


    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;
    private float currentWalkSpeed;

    [Header("Speed Adaptation Parameters")]
    [SerializeField] private float speedBoostMultiplier;
    [SerializeField] private bool isSpeedBoostActive;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float coyoteTimeDuration = 0.2f; // Time window for coyote time
    private float coyoteTimeCounter;       // Timer for coyote time

    [Header("Jump Adaptation Parameters")]
    [SerializeField] private float jumpBoostMultiplier;
    [SerializeField] private bool isJumpBoostActive;

    [Header("Attack Parameters")]
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private bool isAttacking;

    [Header("Attack Adaptation Parameters")]
    [SerializeField] private float attackSizeMultiplier;
    [SerializeField] private bool isLargerAttackActive;

    [Header("Misc. Adaptations")]
    [SerializeField] private bool isSmallerSizeActive;
    [SerializeField] private float smallSizeMultiplier;
    [SerializeField] private bool isLargerSizeActive;
    [SerializeField] private float largeSizeMultiplier;
    [SerializeField] private int totalCurrency;
    [SerializeField] private bool isInDen; //only serialized for debugging
    private bool isAbleToShop = true;

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
        
        UnityEngine.Cursor.visible = false;
        _UICurrency = GameObject.Find("CurrencyText").GetComponent<UICurrency>();
    }

    private void Start()
    {
        inputHandler = PlayerInputHandler.Instance;
    }

    void Update()
    {
        HandleCoyoteTime();
        HandleMovement();
        HandleShopping();
    }

    void HandleMovement()
    {
        HandleJumping();


        float speed = walkSpeed;
        if (isSpeedBoostActive)
        {
            speed = walkSpeed * speedBoostMultiplier;
        }
        else
            speed = walkSpeed;

        Vector3 inputDirection = new Vector3(inputHandler.MoveInput.x, 0f, inputHandler.MoveInput.y).normalized; //MoveInput.y because it's a vector 2 so the y is actually the z

        if (inputDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            characterController.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (inputHandler.AttackTriggered)
        {
            OnAttack();
        }
        

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Den")
        {
            isInDen = true;
        }

        if (other.gameObject.tag == "Currency")
        {
            totalCurrency += 100;
            Debug.Log(totalCurrency);
            _UICurrency.UpdateCurrency(totalCurrency);
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Den")
        {
            isInDen = false;
            adaptationsShop.SetActive(false);
            isAbleToShop = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.CompareTag("EnemyAttack"))
        {
            Debug.Log("i took dmg");
        }
    }

    void HandleJumping()
    {

        if (characterController.isGrounded && coyoteTimeCounter > 0)
        {
            print("ground");
            currentVelocity.y = -2f;

            if (inputHandler.JumpTriggered)
            {
                print("Jumping");
                if (isJumpBoostActive == true)
                {
                    currentVelocity.y = jumpForce * jumpBoostMultiplier;
                }
                else
                    currentVelocity.y = jumpForce;
            }
            coyoteTimeCounter = 0; // Prevent multiple jumps in air

        }
        else
        {
            currentVelocity.y -= gravity * Time.deltaTime;
        }

        characterController.Move(currentVelocity * Time.deltaTime);
    }

    void HandleCoyoteTime()
    {
        if (characterController.isGrounded)
        {
            coyoteTimeCounter = coyoteTimeDuration; // Reset coyote time if grounded
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Decrease timer if not grounded
        }
    }


    private void OnAttack()
    {
        if (isAttacking == false)
        {
            Instantiate(attackPrefab, transform);
            isAttacking = true;
            StartCoroutine(AttackCooldownCoroutine());
        }
    }

    private IEnumerator AttackCooldownCoroutine()
    {
        //timer to prevent player from spamming attack
        yield return new WaitForSeconds(0.8f);
        isAttacking = false;
    }

    /*
     * SHOPPING AND CURRENCY CODE STARTED
     */


    private IEnumerator ShopCooldownCoroutine()
    {
        
        //timer to prevent player from spamming shops
        yield return new WaitForSeconds(0.2f);
        if (isAbleToShop == true)
        {
            isAbleToShop = false;
            
        }
        else
        {
            
            isAbleToShop = true;
            
        }
    }

    void HandleShopping()
    {
        if (inputHandler.ShopTriggered == true && isInDen)
        {
            if (adaptationsShop.activeSelf == false && isAbleToShop == true)
            {
                UnityEngine.Cursor.visible = true;
                gameObject.GetComponent<CharacterController>().enabled = false;
                adaptationsShop.SetActive(true);
                StartCoroutine(ShopCooldownCoroutine());
            }
            else if (adaptationsShop.activeSelf == true && isAbleToShop == false)
            {
                UnityEngine.Cursor.visible = false;
                adaptationsShop.SetActive(false);
                gameObject.GetComponent<CharacterController>().enabled = true;
                StartCoroutine(ShopCooldownCoroutine());
            }
        }
    }
    /*
    * ADAPTATIONS CODE STARTED
    */
    public void JumpBoostButtonHandler()
    {
        if (jumpBoostCost <= totalCurrency && isJumpBoostOwned == false && currentAdaptations < maxAdaptations)
        {
            totalCurrency -= jumpBoostCost;
            isJumpBoostOwned = true;
        }
        if (isJumpBoostOwned == true)
        {
            if (isJumpBoostActive == false)
            {
                isJumpBoostActive = true;
                jumpBoostButton.SetActive(true);
                currentAdaptations++;
            }
            else if (isJumpBoostActive == true)
            {
                isJumpBoostActive = false;
                jumpBoostButton.SetActive(false);
                totalCurrency += jumpBoostCost / 2;
                isJumpBoostOwned = false;
                currentAdaptations--;
            }
        }
        else if (currentAdaptations >= maxAdaptations)
        {
            StartCoroutine(MaxAdaptationCoroutine());
        }
        _UICurrency.UpdateCurrency(totalCurrency);

    }

    public void SpeedBoostButtonHandler()
    {
        if (speedBoostCost <= totalCurrency && isSpeedBoostOwned == false && currentAdaptations < maxAdaptations)
        {
            totalCurrency -= speedBoostCost;
            isSpeedBoostOwned = true;
        }
        if (isSpeedBoostOwned == true)
        {
            if (isSpeedBoostActive == false)
            {
                isSpeedBoostActive = true;
                speedBoostButton.SetActive(true);
                currentAdaptations++;
            }
            else if (isSpeedBoostActive == true)
            {
                isSpeedBoostActive = false;
                speedBoostButton.SetActive(false);
                totalCurrency += speedBoostCost / 2;
                isSpeedBoostOwned = false;
                currentAdaptations--;
            }
        }
        else if (currentAdaptations >= maxAdaptations)
        {
            StartCoroutine(MaxAdaptationCoroutine());
        }
        _UICurrency.UpdateCurrency(totalCurrency);

    }

    public void SmallerSizeButtonHandler()
    {
        if (smallerSizeCost <= totalCurrency && isSmallerSizeOwned == false && currentAdaptations < maxAdaptations)
        {
            totalCurrency -= smallerSizeCost;
            isSmallerSizeOwned = true;
        }

        if (isSmallerSizeOwned == true)
        {

            if (isSmallerSizeActive == false)
            {
                isSmallerSizeActive = true;
                smallSizeButton.SetActive(true);
                this.transform.localScale = transform.localScale * smallSizeMultiplier;
                currentAdaptations++;
            }
            else if (isSmallerSizeActive == true)
            {
                isSmallerSizeActive = false;
                smallSizeButton.SetActive(false);
                
                this.transform.localScale = transform.localScale /smallSizeMultiplier;
                totalCurrency += smallerSizeCost / 2;
                isSmallerSizeOwned = false;
                currentAdaptations--;
            }

        }
        else if(currentAdaptations >= maxAdaptations)
        {
            StartCoroutine(MaxAdaptationCoroutine());
        }
        _UICurrency.UpdateCurrency(totalCurrency);


    }

    public void LargerSizeButtonHandler()
    {
        if (largerSizeCost <= totalCurrency && isLargerSizeOwned == false && currentAdaptations < maxAdaptations)
        {
            totalCurrency -= largerSizeCost;
            isLargerSizeOwned = true;
        }

        if (isLargerSizeOwned == true)
        {

            if (isLargerSizeActive == false)
            {
                isLargerSizeActive = true;
                largeSizeButton.SetActive(true);
                this.transform.localScale = transform.localScale * largeSizeMultiplier;
                currentAdaptations++;
            }
            else if (isLargerSizeOwned == true)
            {
                isLargerSizeActive = false;
                largeSizeButton.SetActive(false);
                this.transform.localScale = transform.localScale/largeSizeMultiplier;
                totalCurrency += smallerSizeCost / 2;
                isLargerSizeOwned = false;
                currentAdaptations--;
            }

        }
        else if (currentAdaptations >= maxAdaptations)
        {
            StartCoroutine(MaxAdaptationCoroutine());
        }
        _UICurrency.UpdateCurrency(totalCurrency);


    }

    public void LargerAttackButtonHandler()
    {
        if (largerAttackCost <= totalCurrency && isLargerAttackOwned == false && currentAdaptations < maxAdaptations)
        {
            totalCurrency -= largerAttackCost;
            isLargerAttackOwned = true;
        }
        if (isLargerAttackOwned == true)
        {
            if (isLargerAttackActive == false)
            {
                isLargerAttackActive = true;
                largeAttackButton.SetActive(true);
                attackPrefab.transform.localScale = transform.localScale * attackSizeMultiplier;
                currentAdaptations++;
            }
            else if (isLargerAttackActive == true)
            {
                isLargerAttackActive = false;
                largeAttackButton.SetActive(false);
                attackPrefab.transform.localScale = transform.localScale / attackSizeMultiplier;
                totalCurrency += largerAttackCost/ 2;
                isLargerAttackOwned = false;
                currentAdaptations--;
            }
        }
        else if (currentAdaptations >= maxAdaptations)
        {
            StartCoroutine(MaxAdaptationCoroutine());
        }
        _UICurrency.UpdateCurrency(totalCurrency);
    }

    IEnumerator MaxAdaptationCoroutine()
    {
        maxAdaptationErrorUI.SetActive(true);
        yield return new WaitForSeconds(1f);
        maxAdaptationErrorUI.SetActive(false);

    }
}
