using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private int _damage;
    [SerializeField] private float range;
    
    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;
    
    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;
    
    //references
    private Animator anim;
    private Health playerHealth;
    private EnemyPatrol enemyPatrol;
    private string enemyName;
    public bool isVulnerable { get; private set; }
    private SpriteRenderer sprite;
    private Color originalColor;

    private void Awake(){
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        enemyName = GetComponent<EnemyTag>().GetEnemyName();
        sprite = GetComponent<SpriteRenderer>();
        originalColor = sprite.color;
    }

    private void Update()
    {
        cooldownTimer+= Time.deltaTime;
        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown){
                cooldownTimer = 0;
                int attack = Random.Range(0, 2);
                PlayMeeleSound(attack);
                if ((attack==0) || HasOneAttack())
                {
                    anim.SetTrigger("meeleAttack1");
                }
                else{
                    anim.SetTrigger("meeleAttack2");
                }
            }   
        }
        if (enemyPatrol!=null){
            enemyPatrol.enabled = !PlayerInSight();
        }
    }
    public void SetBossVulnerable(){
        SetVulnerable(1f);
    }
    public void SetVulnerable(float duration)
    {
        isVulnerable = true;
        sprite.color = Color.red; // flash red
        Invoke(nameof(ClearVulnerability), duration);
    }

    private void ClearVulnerability()
    {
        sprite.color = originalColor; // reset if you want white base
        isVulnerable = false;
    }

    private bool HasOneAttack()
    {
        return enemyName =="Demon_Slime" || enemyName == "Frost_Guardian";
    }

    private bool PlayerInSight(){
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center+transform.right*range*transform.localScale.x*colliderDistance,
        new Vector3(boxCollider.bounds.size.x*range,boxCollider.bounds.size.y,boxCollider.bounds.size.z)
        ,0,Vector2.left,0,playerLayer);

        if (hit.collider != null){
            playerHealth = hit.transform.GetComponent<Health>();
        }
        return hit.collider!=null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center+transform.right*range*transform.localScale.x*colliderDistance,
        new Vector3(boxCollider.bounds.size.x*range,boxCollider.bounds.size.y,boxCollider.bounds.size.z));
    }

    private void DamagePlayer(){
        if (PlayerInSight()){
            playerHealth.TakeDamage(_damage);
        }
    }
    private void PlayMeeleSound(int attack){
        if (enemyName=="Skeleton")
        {
            switch (attack)
            {
                case 0: AudioManager.Instance.PlaySFX(AudioManager.Instance.sword_attack1);break;
                case 1: AudioManager.Instance.PlaySFX(AudioManager.Instance.sword_attack2);break;
            }   
        }else if (enemyName=="Goblin"){
            switch (attack)
            {
                case 0: AudioManager.Instance.PlaySFX(AudioManager.Instance.sword_attack2);break;
                case 1: AudioManager.Instance.PlaySFX(AudioManager.Instance.goblin_attack2);break;
            }
        }
        else if (enemyName=="FlyingEye"){
            switch (attack)
            {
                case 0: AudioManager.Instance.PlaySFX(AudioManager.Instance.goblin_attack2);break;
                case 1: AudioManager.Instance.PlaySFX(AudioManager.Instance.sword_attack2);break;
            }
        }
        else if (enemyName=="Mushroom"){
            AudioManager.Instance.PlaySFX(AudioManager.Instance.goblin_attack1);
        }
        else if (enemyName=="Demon_Slime"){
            AudioManager.Instance.PlaySFX(AudioManager.Instance.demon_attack);
        }else if (enemyName =="Frost_Guardian"){
            AudioManager.Instance.PlaySFX(AudioManager.Instance.demon_attack);
        }
    }
}
