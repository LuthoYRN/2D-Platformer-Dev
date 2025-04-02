using UnityEngine;
using System.Collections; // Required for Coroutine

public class KingAttack : MonoBehaviour
{
    private bool isAlive = false;
    // ===========================
    // 🔹 Serialized Fields
    // ===========================
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint; // The king's firepoint

    // Attack pools for object reuse
    [SerializeField] private GameObject[] slashes;
    [SerializeField] private GameObject[] fireballs;
    [SerializeField] private Transform[] enemyFirePoints; // Array of enemy fire points
    [SerializeField] private GameObject[] lightningBolts; // Lightning bolt pool
    [SerializeField] private GameObject[] specialAttack; // Special attack pool


    [SerializeField] private GameObject[] phoenixes; //For the inactive pheonixes
    private int nextPhoenixIndex = 0;

    // Components
    private Animator anim;
    private KingMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    public int currentHP = 100;

    // Audio
    public AudioSource src;
    public AudioClip fsx1, fsx2;
    private Vector3 originalBeamScale; // Store original beam size

    public PhoenixController phoenix;

    // ===========================
    // 🔹 Initialization
    // ===========================
    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<KingMovement>();
        if (specialAttack.Length > 0)
        {
            originalBeamScale = specialAttack[0].transform.localScale;
        }
    }

    private void Start()
    {
        // Ensure all thunderbolts are disabled at the start
        foreach (GameObject thunderbolt in lightningBolts)
        {
            thunderbolt.SetActive(false);
        }

        Debug.Log($"Thunderbolt pool size: {lightningBolts.Length}");
    }

    private void Die()
    {
        isAlive = false;
        anim.SetTrigger("Death");
    }


    // ===========================
    // 🔹 Update Method - Input Handling
    // ===========================
    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown && playerMovement.canAttack())  //Close range attack
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.X) && cooldownTimer > attackCooldown && playerMovement.canAttack()) //Close range + Long range -> A traveling/propagating slash
        {
            Attack2();
        }

        if (Input.GetKeyDown(KeyCode.C))   //Fire ball attack long rangesd
        {
            Attack3();
        }

        if (Input.GetKeyDown(KeyCode.Z))   //Summoning lightning attack - Long ranged
        {
            src.clip = fsx1;
            src.Play();
            StartCoroutine(DelayedAnimationTrigger(0.9f));
        }

        if (Input.GetKeyDown(KeyCode.S))   //Special attack  - Long ranged
        {
            anim.SetTrigger("specialAttack");
            SpawnSpecialAttack();
        }


        if (Input.GetKeyDown(KeyCode.P))  //Summons phoenixes when health is extremely low
        {
            anim.SetTrigger("summon");

        }

        // For testing: press D to simulate damage
        if (Input.GetKeyDown(KeyCode.O))
        {
            TakeDamage(50);
        }
    }

    public void TakeDamage(int damage)
    {
        anim.SetTrigger("takeDamage");
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    // ===========================
    // 🔹 Delayed Animation Trigger
    // ===========================
    private IEnumerator DelayedAnimationTrigger(float delay)
    {
        yield return new WaitForSeconds(delay);
        anim.SetTrigger("lightningAttack");
    }

    // ===========================
    // 🔹 Lightning Attack Logic
    // ===========================
    public void SpawnLightning()
    {
        if (enemyFirePoints.Length == 0 || lightningBolts.Length == 0)
        {
            Debug.LogError("Lightning setup is missing firepoints or bolts!");
            return;
        }

        foreach (Transform firePoint in enemyFirePoints)
        {
            if (firePoint == null) continue;

            GameObject lightning = GetInactiveLightning();
            if (lightning != null)
            {
                lightning.transform.position = firePoint.position;
                lightning.SetActive(true);
                FindObjectOfType<ScreenFlash>().FlashScreen();

                StartCoroutine(DisableAfterStrike(lightning, 0.25f)); // Auto-disable after 0.25 sec
            }
        }

        src.clip = fsx2;
        src.Play();
    }

    private IEnumerator DisableAfterStrike(GameObject thunderbolt, float delay)
    {
        yield return new WaitForSeconds(delay);
        thunderbolt.SetActive(false);
    }


    public void SummonNextPhoenix()
    {
        if (nextPhoenixIndex >= phoenixes.Length) return;

        GameObject phoenixGO = phoenixes[nextPhoenixIndex];
        if (!phoenixGO.activeInHierarchy)
        {
            phoenixGO.SetActive(true);
            PhoenixController phoenix = phoenixGO.GetComponent<PhoenixController>();
           // phoenix.BeginRebirth();
        }

        nextPhoenixIndex++;
    }

    private GameObject GetInactiveLightning()
    {
        foreach (GameObject lightning in lightningBolts)
        {
            if (!lightning.activeInHierarchy) return lightning;
        }
        return null;
    }

    // ===========================
    // 🔹 Fireball Attack
    // ===========================
    public void SpawnFireball()
    {
        float direction = Mathf.Sign(transform.localScale.x);
        GameObject fireball = GetInactiveProjectile(fireballs);

        if (fireball == null) return;

        fireball.transform.position = firePoint.position;
        fireball.SetActive(true);
        fireball.GetComponent<FireBall_Projectile>().SetDirection(direction);

        // Flip direction if needed
        Vector3 fireballScale = fireball.transform.localScale;
        fireballScale.x = Mathf.Abs(fireballScale.x) * direction;
        fireball.transform.localScale = fireballScale;
    }

    // ===========================
    // 🔹 Special Attack (Beam)
    // ===========================
    public void SpawnSpecialAttack()
    {
        float direction = Mathf.Sign(transform.localScale.x);
        GameObject beam = GetInactiveProjectile(specialAttack);

        if (beam == null) return;

        beam.transform.position = firePoint.position;
        beam.SetActive(true);

        SpecialBeam beamScript = beam.GetComponent<SpecialBeam>();
        beamScript.Initialize(firePoint);  // 🔹 Beam follows firePoint
        beamScript.SetDirection(direction); // 🔹 Fix beam flip issue

        anim.SetTrigger("specialAttack");
        StartCoroutine(DisableAfterStrike(beam, 1.0f));
    }




    // ===========================
    // 🔹 Melee Attack (Slash)
    // ===========================
    private void Attack2()
    {
        anim.SetTrigger("attack2");
        cooldownTimer = 0;

        float direction = Mathf.Sign(transform.localScale.x);
        GameObject slash = GetInactiveProjectile(slashes);

        if (slash == null) return;

        slash.transform.position = firePoint.position;
        slash.SetActive(true);
        slash.GetComponent<Projectile>().SetDirection(direction);

        // Adjust direction
        Vector3 slashScale = slash.transform.localScale;
        slashScale.x = Mathf.Abs(slashScale.x) * direction;
        slash.transform.localScale = slashScale;
    }

    // ===========================
    // 🔹 General Attack Animations
    // ===========================
    private void Attack()
    {
        anim.SetTrigger("attack");
        cooldownTimer = 0;
    }

    private void Attack3()
    {
        anim.SetTrigger("magicAttack");
    }

    public void TriggerLightning()
    {
        SpawnLightning();
    }

    // ===========================
    // 🔹 Utility Methods
    // ===========================
    private GameObject GetInactiveProjectile(GameObject[] pool)
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy) return obj;
        }

        Debug.LogWarning("No inactive projectiles found! Consider increasing the pool size.");
        return null;
    }
}
