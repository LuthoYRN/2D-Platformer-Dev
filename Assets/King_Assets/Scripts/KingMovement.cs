using UnityEngine;

public class KingMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;
    private Animator anim;
    private float targetInput = 0f;
    private bool isCurrentlyRunning = false;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    public bool left = true;
    public static float stopDistance = 22f;

    public bool useAI = false;
    public Transform aiTarget;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (!useAI) return;

        if (aiTarget != null)
        {
            float distance = aiTarget.position.x - transform.position.x;

            if (Mathf.Abs(distance) > stopDistance + 0.1f)
                targetInput = Mathf.Sign(distance);
            else if (Mathf.Abs(distance) < stopDistance - 0.1f)
                targetInput = 0f;

            // Smooth horizontalInput towards targetInput (feel free to increase smoothing factor)
            horizontalInput = Mathf.MoveTowards(horizontalInput, targetInput, Time.deltaTime * 10f);
        }

        // Flip sprite based on movement direction
        if (horizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        }

        // Smooth running logic (only update if changed)
        bool shouldRun = Mathf.Abs(horizontalInput) > 0.1f;
        if (shouldRun != isCurrentlyRunning)
        {
            isCurrentlyRunning = shouldRun;
            anim.SetBool("run", isCurrentlyRunning);
        }

        anim.SetBool("grounded", isGrounded());

        // Move
        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);
    }

    public bool KeepDistance(Transform target)
    {
        useAI = true;
        aiTarget = target;

        float distance = target.position.x - transform.position.x;
        horizontalInput = -Mathf.Sign(distance); // 🔄 Move away from target

        if (NearWall(horizontalInput))
        {
            horizontalInput = 0f; // stop movement
            return true; // ✅ Stuck, signal AI to attack
        }

        return false; // ✅ Still able to move away
    }


    private bool NearWall(float direction)
    {
        Vector2 checkDir = new Vector2(direction, 0);
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            checkDir,
            0.1f,
            wallLayer
        );

        return hit.collider != null;
    }




    private void Jump()
    {
        if (isGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            anim.SetTrigger("jump");
        }
        else if (onWall() && !isGrounded())
        {
            if (horizontalInput == 0)
            {
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x) * 6, 6, 1);
            }
            else
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);

            wallJumpCooldown = 0;
        }
    }


    public void ChaseEnemy(Transform target)
    {
        useAI = true;
        aiTarget = target;
    }

    public void JumpAI()
    {
        if (isGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            anim.SetTrigger("jump");
        }
    }

    private bool isGrounded(){
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size,0,Vector2.down,0.15f,groundLayer);
        Debug.Log(raycastHit.collider!=null);
        return raycastHit.collider!=null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
    public void Disable(){
        enabled = false;
        BossAI kingAttack = this.gameObject.GetComponent<BossAI>();
        kingAttack.enabled = false;
    }
    public void Enable(){
        enabled = true;
        BossAI kingAttack = this.gameObject.GetComponent<BossAI>();
        kingAttack.enabled = true;
        
    } 
   
}

