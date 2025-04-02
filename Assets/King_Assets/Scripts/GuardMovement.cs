using UnityEngine;

public class GuardMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpPower = 12f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D col; // ✅ Changed to BoxCollider2D

    private float moveDir = 0f;
    private bool isFacingRight = true;
    public float stopDistance = 0.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>(); // ✅ Get BoxCollider2D instead
    }

    private void Update()
    {
        // Manual jump test (press J)
        if (Input.GetKeyDown(KeyCode.J))
        {
            Jump();
        }

        rb.linearVelocity = new Vector2(moveDir * speed, rb.linearVelocity.y); // ✅ Used velocity

        // Flip sprite
        if (moveDir > 0 && !isFacingRight)
            Flip();
        else if (moveDir < 0 && isFacingRight)
            Flip();

        anim.SetBool("run", Mathf.Abs(moveDir) > 0.01f);
        anim.SetBool("grounded", IsGrounded());
    }

    public void ChaseTarget(Transform target)
    {
        float distance = target.position.x - transform.position.x;

        if (Mathf.Abs(distance) > stopDistance)
            moveDir = Mathf.Sign(distance);
        else
            moveDir = 0f;
    }

    public void Jump()
    {
        if (IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower); // ✅ Use velosuguyguggyuf
            anim.SetTrigger("jump");
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
