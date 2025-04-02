using Unity.Mathematics;
using UnityEngine;

public class FireBall_Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private float direction;
    private bool hit;
    private float lifetime;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private Vector3 originalScale; // Store the original scale

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        originalScale = transform.localScale; // Store the initial scale
    }

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    void Update()
    {
        if (hit) return;

        float movementSpeed = speed * Time.deltaTime * direction;

        // 🔹 Check if the projectile is about to hit an enemy or wall
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.right * direction, movementSpeed, LayerMask.GetMask("Player", "Ground"));

        if (hitInfo.collider != null)
        {
            Debug.Log($"Fireball hit: {hitInfo.collider.gameObject.name}");

            if (hitInfo.collider.CompareTag("Player"))
            {
                hitInfo.collider.gameObject.GetComponent<Health>().TakeDamage(1);
                PushEnemyBack(hitInfo.collider.gameObject);
            }

            Explode();
            return;
        }

        // 🔹 Move projectile normally if no obstacle is detected ahead
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > 5) gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        transform.localScale = new Vector3(Mathf.Abs(originalScale.x) * direction, originalScale.y, originalScale.z);
    }

    private void Explode()
    {
        hit = true;
        boxCollider.enabled = false;
        anim.SetTrigger("fireball_Explode");

        // 🔹 Push back any enemy caught in the explosion
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 2f, LayerMask.GetMask("Player"));

        foreach (Collider2D enemy in enemies)
        {
            PushEnemyBack(enemy.gameObject);
        }

        // 🔹 Delay disabling object
        Invoke(nameof(Deactivate), 0.5f);
    }

    private void PushEnemyBack(GameObject enemy)
    {
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb != null)
        {
            Vector2 forceDirection = (enemy.transform.position - transform.position).normalized;

            // 🔹 Only push horizontally, ignore downward force
            forceDirection.y = Mathf.Abs(forceDirection.y) * 0.2f; // Small vertical lift to avoid sinking
            forceDirection.x = Mathf.Sign(forceDirection.x); // Keep push direction fixed

            float explosionForce = 12f; // Adjust force for a proper push
            enemyRb.linearVelocity = Vector2.zero; // Reset velocity to prevent extreme movement
            enemyRb.AddForce(forceDirection * explosionForce, ForceMode2D.Impulse);

            // 🔹 Reduce gravity temporarily to avoid sinking
            enemyRb.gravityScale = 0.5f; // Lower gravity effect briefly
            Invoke(nameof(RestoreGravity), 0.5f);
        }
    }

    private void RestoreGravity()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject enemy in enemies)
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                enemyRb.gravityScale = 3f; // Restore normal gravity
            }
        }
    }

    public void SetDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        transform.parent = null;
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = originalScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3(localScaleX, originalScale.y, originalScale.z);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
