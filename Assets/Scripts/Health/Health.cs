using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    public float StartingHealth => startingHealth;
    [SerializeField]private Healthbar bar;
    [SerializeField]private EnemyHealthBar enemy_bar;
    private Animator anim;
    private bool dead;
    [Header("Components")]
    [SerializeField] private Behaviour[] components;
    private AudioManager audioManager;
    private bool isPlayer;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        isPlayer = CompareTag("Player");
        if (!isPlayer){enemy_bar.SetHealth(StartingHealth,startingHealth);       
}
    }

    public void TakeDamage(float _damage)
    {
        if (dead) return;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
        if (isPlayer){bar.fillAmount = bar.Map(currentHealth,0,startingHealth,0,1);}
        else{
            if(GetComponent<EnemyTag>().GetEnemyName()=="Frost_Guardian" || GetComponent<EnemyTag>().GetEnemyName()=="Demon_Slime"){
                MeleeEnemy boss = GetComponent<MeleeEnemy>();
                if (boss != null && !boss.isVulnerable)
                {
                    Debug.Log("Boss is invulnerable!");
                    currentHealth = Mathf.Clamp(currentHealth + _damage, 0, startingHealth);
                    return;
                }
            }
            enemy_bar.SetHealth(currentHealth,startingHealth);
        }
        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            if (isPlayer) audioManager.PlaySFX(audioManager.take_damage);
        }
        else
        {
            if (!dead)
            {
                if(isPlayer){
                    anim.SetBool("jump",false);
                    anim.SetBool("fall",false);
                    anim.ResetTrigger("hurt");
                    anim.SetTrigger("die");
                    audioManager.PlaySFX(audioManager.death);
                    Rigidbody2D rb = GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector2.zero;
                        rb.bodyType = RigidbodyType2D.Static; 
                    }
                    if (TryGetComponent<PlayerMovement>(out var move)) move.enabled = false;
                    if (TryGetComponent<PlayerAttack>(out var attack)) attack.enabled = false;
                }else{
                    anim.SetTrigger("die");
                    //enemy
                    foreach (Behaviour component in components)
                    {
                        component.enabled = false;
                    }
                    PlayDeathEffect();
                }
                  
                dead = true;
            }
        }
    }

    public void Respawn(){
        dead = false;
        AddHealth((float)startingHealth/2);
        anim.ResetTrigger("die");
        anim.Play("idle");
        WeaponWheelController.weaponID = 1;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;
        }
        if (GetComponent<PlayerMovement>()!=null)GetComponent<PlayerMovement>().enabled = true;
        if (TryGetComponent<PlayerAttack>(out var attack)) attack.enabled = true;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {       
            if (enemy.activeInHierarchy){
                Health enemyHealth = enemy.GetComponent<Health>();
                enemyHealth.AddHealth(enemyHealth.startingHealth);
            }
        } 
    }
    private void PlayDeathEffect()
    {
        string enemyName = GetComponent<EnemyTag>().GetEnemyName();
        switch (enemyName)
        {
            case "Skeleton": audioManager.PlaySFX(audioManager.skeleton_death);break;
            case "Goblin": audioManager.PlaySFX(audioManager.goblin_death);break;            
            case "FlyingEye": audioManager.PlaySFX(audioManager.goblin_death);break;
            case "Mushroom": audioManager.PlaySFX(audioManager.goblin_death);break;            
            case "Demon_Slime": audioManager.PlaySFX(audioManager.demon_death);break;
            case "Frost_Guardian": audioManager.PlaySFX(audioManager.demon_death);break;
        }
    }
    public void Deactivate(){
        gameObject.SetActive(false);
    }
    public void AddHealth(float _value){
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
        if(isPlayer) {bar.fillAmount = bar.Map(currentHealth,0,startingHealth,0,1);}
        else{
            enemy_bar.SetHealth(_value,_value);
        }
    }
}
