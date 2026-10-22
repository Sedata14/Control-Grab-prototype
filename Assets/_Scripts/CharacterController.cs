using UnityEngine;
using UnityEngine.InputSystem;
public class CharacterController : MonoBehaviour
{
    [SerializeField] private float Speed;
    [SerializeField] private float SpeedMultiplier;
    [SerializeField] private float WalkSpeed, SprintSpeed, LevitatingSpeed;
    [SerializeField] private float DampingLevitation;


    private PlayActions actions;

    [SerializeField] private Vector2 MovePos;

    bool isMoving = false;
    Rigidbody rb;

    [SerializeField] private Transform pivot;

    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isLevitating;
    [SerializeField] private bool isSprinting;
    [SerializeField] private float JumpHeight;
    [SerializeField] private bool HasLevitated; // used when levitated and still not touched the ground

    [SerializeField] private Transform GroundCheck;

 
    [SerializeField] private LayerMask PlayerLayerMask;

    [SerializeField] ParticleSystem JumpParticleSystem;


    [SerializeField] private float StartFallingTime;
    [SerializeField] private float CurrentFallingTime;
    private void Awake()
    {
        actions = new();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        actions.Movement.Enable();
        
        actions.Movement.Move.performed += Move;
        actions.Movement.Move.canceled += Move;

        actions.Movement.Jump.performed += Jump;

        
        actions.Movement.Sprint.performed += ctx => {  isSprinting = true; };
        actions.Movement.Sprint.canceled += ctx => {  isSprinting = false; };
    }

    private void OnDisable()
    {
        actions.Movement.Move.performed -= Move;
        actions.Movement.Move.canceled -= Move;
        actions.Movement.Jump.performed -= Jump;

        actions.Movement.Sprint.performed -= ctx => { isSprinting = true; };
        actions.Movement.Sprint.canceled -= ctx => { isSprinting = false; };


        actions.Movement.Disable();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.Raycast(GroundCheck.transform.position, -GroundCheck.transform.up, 0.1f, ~PlayerLayerMask);
                
        if(isGrounded) { isLevitating = false; rb.linearDamping = 0f; HasLevitated = false; CurrentFallingTime = StartFallingTime; }


        if (isLevitating)
        {
            Debug.Log("Is levitating");
            if (CurrentFallingTime > 0f)
            {
                Debug.Log("Timer ?");
                rb.useGravity = false;
                CurrentFallingTime -= Time.deltaTime;
            }
            else
            {
                rb.useGravity = true;
                rb.linearDamping = 2f;
            }
        }

        if (isMoving)
        {
            Vector3 dir = (pivot.transform.forward * MovePos.y) + (pivot.transform.right * MovePos.x);

            if (isGrounded)
            {
                if (isSprinting)
                {
                    Speed = SprintSpeed;
                }
                else
                {
                    Speed = WalkSpeed;
                }
            }
            else
            {
                if (isLevitating)
                {
                    Speed = LevitatingSpeed;
                    Debug.Log("Is levitating");
                    if(CurrentFallingTime > 0f)
                    {
                        Debug.Log("Timer ?");
                        rb.useGravity = false;
                        CurrentFallingTime -= Time.deltaTime;
                    }
                    else
                    {
                        rb.useGravity = true;
                        rb.linearDamping = 2f;
                    }
                }
                else Speed = WalkSpeed;
            }
            dir *= Speed * Time.deltaTime;
            rb.linearVelocity = new Vector3(dir.x, rb.linearVelocity.y, dir.z);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }

        
        Debug.DrawRay(GroundCheck.transform.position, -GroundCheck.transform.up * 0.1f, Color.red);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MovePos = context.ReadValue<Vector2>();


            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

    }


    public void Jump(InputAction.CallbackContext context)
    {
     
        if (context.performed )
        {

            if (!isGrounded && isLevitating)
            {
                isLevitating = false;
                rb.linearDamping = 0f; // tu remet la friction à 0 qd tu annules la lévitation
            }
            else if (!isGrounded && !isLevitating)
            {
                isLevitating = true; 
                rb.linearDamping = DampingLevitation;
                if (!HasLevitated)
                {
                    rb.AddForce(transform.up * 100, ForceMode.Impulse);
                }

                HasLevitated = true;
                
            }
            

            if (isGrounded)
            {
                rb.AddForce(transform.up * JumpHeight, ForceMode.Impulse);
                JumpParticleSystem.Play();
            }

        }
    }



}


