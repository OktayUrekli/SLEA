using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementManager : MonoBehaviour
{
    Rigidbody playerRB;
    Animator playerAnimator;
    TrailRenderer playerTrailRenderer;

    ThirdPersonPlayerMovement playerInputs; // ana karakterin hareketi i�in girdileri alg�layan unitynin new �nput sistemi  
    InputAction dashAction;

    [Header("Main Movement Variables")]
    [SerializeField] Vector2 movementInput;  // edit�rde test i�in serializefield yap�ld�, daha sonra kapat�lacak
    [SerializeField] Vector3 lookingDirection;
    [SerializeField] float movementSpeed = 5f; // h�z �arpan�
    [SerializeField] float rotationSpeed = 25f;

    [Header("Dash Ability Variables")]
    [SerializeField] float dashingAmount = 10f; // unity birim say�s�d�r 
    [SerializeField] float dashingCooldown = 1f; // dash atma s�re aral���
    bool canDash;

    private void Awake()
    {
        playerInputs = new ThirdPersonPlayerMovement();
        playerRB = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        playerTrailRenderer = GetComponentInChildren<TrailRenderer>();

        playerTrailRenderer.emitting = false;
        canDash = true;

        playerInputs.PlayerMovement.Movement.started += Move;
        playerInputs.PlayerMovement.Movement.performed += Move;
        playerInputs.PlayerMovement.Movement.canceled += Move;

        dashAction = playerInputs.PlayerMovement.Dashing;

    }

    public void Move(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Movement();

        if (dashAction.IsInProgress() && canDash)
        {
            StartCoroutine(Dashing());
        }
    }

    void Movement()
    {
        if (movementInput.x != 0 || movementInput.y != 0)  // e�er wasd tu�lar�ndan en az birine bas�l�yorsa hareket et
        {
            playerAnimator.SetFloat("Speed", movementInput.sqrMagnitude);

            lookingDirection = new Vector3(movementInput.x, 0, movementInput.y);
            Quaternion targetRotation = Quaternion.LookRotation(lookingDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            float xVelocity = movementInput.x * Time.deltaTime * movementSpeed;
            float zVelocity = movementInput.y * Time.deltaTime * movementSpeed;
            playerRB.MovePosition(playerRB.position + new Vector3(xVelocity, 0, zVelocity));
        }
        else{ playerAnimator.SetFloat("Speed", 0); }
    }

    IEnumerator Dashing() // roll butonu ile ili�kilendirilecek metod
    {
        canDash = false; // ardarda dash butonuna bas�lamamas� i�in false yap�ld�
        playerTrailRenderer.emitting = true;  // trail renderer sadece dash s�ras�nda g�z�kecek
        playerRB.MovePosition(transform.position + transform.forward * dashingAmount); 
        yield return new WaitForSeconds(0.1f); // trail renderer 0.1 sn lik g�r�necek
        playerTrailRenderer.emitting = false; 
        yield return new WaitForSeconds(dashingCooldown); 
        canDash = true; // oyuncu her (dashingCooldown+0.1) sn de bir dash atabilir
    }


    private void OnEnable()
    {
        playerInputs.PlayerMovement.Enable();
    }

    private void OnDisable()
    {
        playerInputs.PlayerMovement.Disable();        
    }
}
