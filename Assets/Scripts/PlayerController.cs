using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;

    enum playerState {idle = 0, run = 1, jump = 2, standShoot = 3, runShoot = 4};
    bool isJumping, canDoubleJump, isShooting;
    public bool isGrounded;
    public Transform feet;
    public float feetRadius;
    public LayerMask whatIsGround;
    public float speedBoost = 6.0f;
    public float jumpSpeed = 600f;
    public float delayForDoubleJump;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded = Physics2D.OverlapCircle(feet.position, feetRadius, whatIsGround)){
            isJumping = false;
        }
        

        float playerSpeed = Input.GetAxisRaw("Horizontal"); // Returns -1, 0, or 1
        playerSpeed *= speedBoost; // Speed up the player
        if (playerSpeed != 0) {
            MoveHorizontal(playerSpeed);
        } else {
            StopMoving();
        }

        if (Input.GetButtonDown("Jump")) {
            Jump();
        }

        if (Input.GetButtonDown("Fire1")) {
            Shoot();
        }
        if (Input.GetButtonUp("Fire1")) {
            StopShooting();
        }
    }

    void OnDrawGizmos() 
    {
        Gizmos.DrawWireSphere(feet.position, feetRadius);
    }

    void MoveHorizontal(float playerSpeed)
    {
        rb.velocity = new Vector2(playerSpeed, rb.velocity.y);
        if (playerSpeed < 0) {
            sr.flipX = true;
        } else if (playerSpeed > 0) {
            sr.flipX = false;
        }
        if (!isJumping) {
            if (!isShooting) {
                anim.SetInteger("State", (int)playerState.run);
            } else {
                anim.SetInteger("State", (int)playerState.runShoot);
            }
        }
    }

    void StopMoving()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        if (!isJumping) {
            if (!isShooting) {
                anim.SetInteger("State", (int)playerState.idle);
            } else {
                anim.SetInteger("State", (int)playerState.standShoot);
            }
        }
        
    }

    void Jump()
    {
        if (isGrounded) {
            isJumping = true;
            rb.AddForce(new Vector2(0, jumpSpeed)); // Simply add an upwards force in the +ve y
            anim.SetInteger("State", (int)playerState.jump);

            Invoke("EnableDoubleJump", delayForDoubleJump);
        }

        if (canDoubleJump && !isGrounded) {
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(0, jumpSpeed)); // Simply add an upwards force in the +ve y
            anim.SetInteger("State", (int)playerState.jump);
            canDoubleJump = false;
        }
    }

    void EnableDoubleJump() {
        canDoubleJump = true;
    }

    void Shoot()
    {   
        isShooting = true;
        
    }

    void StopShooting()
    {
        isShooting = false;
    }

    //Primative ground detection. Replaced by feet object ans isGrounded.
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground")) {
            isJumping = false;
        }
    }
}
