using UnityEngine;
using System.Collections;

public class GuardCombat : MonoBehaviour
{

    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public float attackCooldown = 1.2f;
    public LayerMask enemyLayers;


    public Transform player;
    private float attackTimer = 0f;

    private GuardMovement guardMovement;
    private bool isAttacking = false;

    private void Awake()
    {
        guardMovement = GetComponent<GuardMovement>();
    }

    private void Update()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Enemy")?.transform;

        if (player == null || isAttacking) return;

        attackTimer += Time.deltaTime;

        guardMovement.ChaseTarget(player);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        if (hitEnemies.Length > 0 && attackTimer >= attackCooldown)
        {
            StartCoroutine(PerformAttack(hitEnemies));
        }

        // Projectile dodge
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, 1f, LayerMask.GetMask("Projectile"));
        if (hit.collider != null && guardMovement.IsGrounded())
        {
            guardMovement.Jump();
        }
    }

    private IEnumerator PerformAttack(Collider2D[] enemies)
    {
        isAttacking = true;
        attackTimer = 0f;

        animator.SetTrigger("attack");

        // Optional: Wait a bit before applying damage (like wind-up delay)
        yield return new WaitForSeconds(0.3f);

        foreach (Collider2D enemy in enemies)
        {
            Debug.Log("Guard hit " + enemy.name);
            //enemy.GetComponent<BossAI>()?.TakeDamage(10);
        }

        // Wait for the full animation length before attacking again
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength); // Or use fixed time like 1.5fvwrgwefgewewrfweewgewdngnienr
        
        isAttacking = false;
    }

    public void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Guard hit " + enemy.name);
            enemy.GetComponent<BossAI>()?.TakeDamage(10);
        }
    }

    private void Attack()
    {
        animator.SetTrigger("attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Guard hit " + enemy.name);
            enemy.GetComponent<BossAI>()?.TakeDamage(10); // Or whatever the guard hits
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
