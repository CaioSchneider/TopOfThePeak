using System.Collections;
using UnityEngine;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerController : MonoBehaviour
{
    private float fireTimer;
   
    private PlayerStateList pState;
    public static bool hasFire = true;
    public static bool hasPick = false;
    Animator anim;
    public static PlayerController Instance;
    [Header("Movement Settings")]
    private float xAxis;
    private float yAxis;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float coyoteTime;

    [Header("Jump Settings")]
    private float jumpForce = 20.0f;
    [SerializeField] private int jumpBufferFrames;
    private int jumpBufferCounter = 0;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float wallJumpingDuration;

    [Header("Dash Settings")]
    private float dragForce;
    private Vector2 dashDirection;
    private Rigidbody2D rb;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    private float gravity;
    
    [Header("WallJump Settings")]
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Vector2 wallJumpingPower;
    float wallJumpingDirection;
    bool isWallSliding;
    bool isWallJumping;
    
    [Header("Health Settings")]
    [SerializeField] public int maxHealth;
    public int health;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;
    
    //[SerializeField] private ScreenFilter screenFilter;
    
    [Header("Ground Settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.2f;
    [SerializeField] private LayerMask whatIsGround;
    public static Vector2 lastGround;
    public static Vector2 lastCheckPoint;


    public void Heal()
    {
        health++;
        ClampHealth();
        if (onHealthChangedCallback != null)
        {
            onHealthChangedCallback.Invoke();
        }
    }


    
 
   
    private void Awake()
    {
        if(Instance !=null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        health = maxHealth;
    }
 

   
  //  public void IncreaseHealth()
   // {
    //    maxHealth++;
  //  }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>(); 
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale;
        dragForce = rb.linearDamping;
    }
    private void FixedUpdate()
    {
        if (pState.dashing) return;
    }


    // Update is called once per frame
    void Update()
    {

        GetInputs();
          
        UpdateJumpVariables();
        StartDash();
        if (hasFire)
            anim.SetBool("Fire", true);


        if (!isWallJumping)
        {
            Jump();
            Flip();
            if (!pState.dashing)
            {
                Move();
            }
        }
        WallSlide();
        WallJump();
        Freeze();

    }
 public void TakeDamage()
    {

        health -= 1;
        fireTimer = 0f;
        ClampHealth();
        if (health <= 0)
        {
            transform.position = lastCheckPoint;
            health = maxHealth;
        }
        else
        {
            transform.position = lastGround;
        }
        if (onHealthChangedCallback != null)
        {
            onHealthChangedCallback.Invoke();
        }      
    }
    
    void Freeze()
    {
        if (!hasFire)
        {
            //screenFilter.SetFilter();
            fireTimer += Time.deltaTime; 
            if (fireTimer >= 5f)
            {
                health -= 1;
                if (health <= 0)
                {
                    transform.position = lastCheckPoint;
                    health = maxHealth;
                }
                ClampHealth();


                if (onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
                fireTimer = 0f; 
            }
        }
        else
        {
            fireTimer = 0f;
         //   if (screenFilter != null)
             //  screenFilter.ClearFilter();
        }
    }


    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
    }

    void ClampHealth()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
    }
    private void Move()
    {
        rb.linearVelocity = new Vector2(walkSpeed * xAxis, rb.linearVelocity.y);
        anim.SetBool("Walking", rb.linearVelocity.x != 0 && Grounded());
    }

    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && hasFire)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        hasFire = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        anim.SetBool("Fire", false);
        rb.linearDamping = 0;
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
        dashDirection = new Vector2(xAxis, yAxis);
        if (dashDirection == Vector2.zero)
        {
            dashDirection = new Vector2(Mathf.Sign(transform.localScale.x), 0);
        }
        rb.linearVelocity = dashSpeed * dashDirection.normalized;

        yield return new WaitForSeconds(dashTime);
        rb.linearDamping = dragForce;
        rb.gravityScale = gravity;
        pState.dashing = false;
    }




    private void Jump()
    {
        if (Input.GetButtonUp("Jump")&& rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            pState.jumping = false;
        }

        if (!pState.jumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter>0)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce);
                pState.jumping = true;
            }
        }

        
        anim.SetBool("Jumping", !Grounded());

    }
    void Flip()
    {
        Vector3 scale = transform.localScale;

        if (xAxis < 0)
            scale.x = Mathf.Abs(scale.x) * -1; // inverte o sinal
        else if (xAxis > 0)
            scale.x = Mathf.Abs(scale.x); // garante que está virado pra direita

        transform.localScale = scale;

    }

 
    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround) ||
            Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround) ||
            Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {

            return true;
        }
        else
        {
            return false;
        }
    }
    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; 
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }
    private bool Walled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }
    void WallSlide()
    {
        if(Walled() &&!Grounded() && xAxis != 0 &&hasPick)
        {
            isWallSliding = true;
            rb.linearVelocity = rb.linearVelocity = new Vector2(rb.linearVelocityX, Mathf.Clamp(rb.linearVelocityY, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }
    void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = xAxis;
            CancelInvoke(nameof(StopWallJumping));
        }
        if (Input.GetButtonDown("Jump") && isWallSliding)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }

    }
    void StopWallJumping()
    {
        isWallJumping = false;
    }
}
