using UnityEngine;
using System.Collections.Generic;

public class PhoenixController : MonoBehaviour
{

    public Animator animator;
    public Transform enemyTarget;
    public float verticalSpeed = 2f;
    public float horizontalAcceleration = 1f;
    public float maxSpeed = 5f;
    public float cooldownTime = 20f;
    private Vector2 velocity;
    private bool isAlive = false;
    private bool isActivated = false;
    private int health = 50;
    private int maxHealth = 50;
    
    private int respawnCount = 0;
    private Rigidbody2D rb;
    public static bool alive = false;


    public Transform firePoint;
    public GameObject fireballPrefab;
    public int poolSize = 20;
    public float fireballCooldown = 2f;
    public EnemyHealthBar enemy_bar;
    private float fireballTimer = 0f;
    private List<GameObject> fireballs = new List<GameObject>();

    private void Awake()
    {
        enemy_bar.SetHealth(health,maxHealth);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject fb = Instantiate(fireballPrefab);
            fb.SetActive(false);
            fireballs.Add(fb);
        }

        if (rb != null)
        {
            rb.gravityScale = 0f; // 🔥 disables gravity!
            rb.freezeRotation = true;
        }

        transform.rotation = Quaternion.identity;
        isAlive = true;
        alive = true;

        GameObject enemy = GameObject.FindWithTag("Player");
        if (enemy != null)
            enemyTarget = enemy.transform;
    }


    void Update()
    {
        if (!isAlive) return;
        if (isAlive)
        {
            Fly();
            TrackEnemy();
        }

        fireballTimer += Time.deltaTime;
        FireAtEnemy();
    }




    // Makes the phoenix trigger the flight animation
    public void BeginFlight()
    {
        isAlive = true;
        alive = true;
        animator.SetTrigger("Fly");
    }

    public void FireAtEnemy()
    {
        if (enemyTarget == null) return;

        foreach (GameObject fb in fireballs)
        {
            if (!fb.activeInHierarchy)
            {
                fb.transform.position = firePoint.position;

                // Determine direction Phoenix is facing
                float zRotation = transform.localScale.x > 0 ? 125f : 45f;

                // Apply rotation
                fb.transform.rotation = Quaternion.Euler(0f, 0f, zRotation);

                // Launch toward enemy
                fb.GetComponent<Fireball>().Launch(enemyTarget);

                break;
            }
        }
    }

 
    private void Fly()
    {
        if (enemyTarget == null) return;

        // Target height = 2x enemy Y
        float targetY = enemyTarget.position.y * 2f;

        // Get horizontal distance to enemy
        float distanceX = enemyTarget.position.x - transform.position.x;

        // Add a horizontal tolerance — only move if we're not close enough
        float stopDistanceX = 0.2f;

        if (Mathf.Abs(distanceX) > stopDistanceX)
        {
            // Move toward enemy at full speed
            float directionX = Mathf.Sign(distanceX);
            velocity.x = directionX * maxSpeed;
        }
        else
        {
            // We're close enough — stop horizontal movement
            velocity.x = 0f;
        }

        // Handle vertical movement
        velocity.y = (transform.position.y < targetY) ? verticalSpeed : 0f;

        // Apply movement
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }



    private void TrackEnemy()
    {
        if (enemyTarget == null) return;

        Vector3 dir = (enemyTarget.position - transform.position).normalized;

        // Flip sprite on X axis to face enemy ...... 
        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(dir.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        // Optional: reset rotation every frame just in case ............
        transform.rotation = Quaternion.identity;
    }


    //For the phoenix to take demage........
    public void TakeDamage(int damage)
    {
        if (!isAlive) return;
        animator.SetTrigger("Damage");
        health -= damage;
        enemy_bar.SetHealth(health, maxHealth);

        if (health <= 0)
        {
            Die();
        }
    }

    //When the phoenix dies.......
    private void Die()
    {
        isAlive = false;
        alive = false;
        animator.SetTrigger("Death");

        // Let gravity pull it down like falling ashes
        rb.gravityScale = 1f;
        rb.linearVelocity = Vector2.zero; // ✅ FIXED HERE
        rb.isKinematic = false;

        Invoke(nameof(Respawn), cooldownTime);
    }


    public bool IsAlive()
    {
        return isAlive;
    }



    //When the phoenix awakens once more!!!
    private void Respawn()
    {
        respawnCount++;
        health = 50 + (respawnCount * 10);
        velocity = Vector2.zero;

        // Lift slightly up to simulate rising
      //  transform.position = new Vector2(transform.position.x, transform.position.y + 2f);

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero; // ✅ FIXED HERE

        animator.SetTrigger("isReborn");
        isAlive = true;
        alive = true ;    
    }

}
