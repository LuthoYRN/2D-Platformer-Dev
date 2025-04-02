using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 8f;
    private Transform target;
    private bool active = false;
    [SerializeField] private float spriteOffset = 90f;

    private Animator animator;
    private Rigidbody2D rb;

    private bool isExploding = false;
    private bool hit = false;

    [SerializeField] private float detectionRadius = 0.5f;
    [SerializeField] private LayerMask playerLayer; // Assign this in Inspector to "Player"

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Transform enemy)
    {
        target = enemy;
        active = true;
        isExploding = false;
        hit = false;
        GetComponent<Collider2D>().enabled = true;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!active || target == null || isExploding || hit)
            return;

        // Move fireball toward enemy
        Vector2 direction = (target.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Rotate fireball
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float directionOffset = transform.localScale.x < 0 ? -spriteOffset : spriteOffset;
        transform.rotation = Quaternion.AngleAxis(angle + directionOffset, Vector3.forward);

        // Manual collision detection
        Collider2D hitInfo = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (hitInfo != null)
        {
            Debug.Log("🔥 Fireball hit player: " + hitInfo.name);
            animator.SetTrigger("Explode");
            Explode();
        }

        

    }

    private void Explode()
    {
        hit = true;
        isExploding = true;
        animator.SetTrigger("Explode");

        active = false;
        rb.linearVelocity = Vector2.zero;

        GetComponent<Collider2D>().enabled = false;
    }

    private void Deactivate()
    {
        active = false;
        gameObject.SetActive(false);
    }

    // Optional: see the collision detection radius in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
