using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossAI : MonoBehaviour
{
    private Transform player;

    [Header("Stats")]
    [SerializeField] private float currentMP = 200f;
    [SerializeField] private float maxMP = 200f;
    [SerializeField] private float currentHP = 100f;
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float mpRegenRate = 5f;
    public float moveSpeed = 3f;

    [Header("Ranges & Cooldowns")]
    [SerializeField] private float meleeRange = 2.5f;
    [SerializeField] private float magicRange = 30f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Components")]
    public EnemyHealthBar enemy_bar;
    private Animator anim;
    private KingMovement playerMovement;

    [Header("Visual & FX")]
    [SerializeField] private Transform firePoint;
    public AudioClip fsx1, fsx2;
    private Vector3 originalBeamScale;

    [Header("Attack Pools")]
    [SerializeField] private GameObject[] slashes;
    [SerializeField] private GameObject[] fireballs;
    [SerializeField] private GameObject[] lightningBolts;
    [SerializeField] private GameObject[] specialAttack;
    [SerializeField] private GameObject[] phoenixes;
    [SerializeField] private Transform[] enemyFirePoints;

    [Header("References")]
    public Transform king;
    public PhoenixController phoenix;

    private int nextPhoenixIndex = 0;
    private float cooldownTimer = Mathf.Infinity;
    private bool inLowMPMode = false;
    private static readonly System.Random random = new();

    private Queue<State> stateQueue = new();
    private bool isExecutingState = false;
    private enum State { Idle, Chase, MeleeAttack, MagicAttack, Retreat, Regenerate, MagicFury }
    private State currentState = State.Idle;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<KingMovement>();
        originalBeamScale = specialAttack.Length > 0 ? specialAttack[0].transform.localScale : Vector3.one;
        enemy_bar.SetHealth(maxHP, maxHP);
    }

    private void Start()
    {
        currentHP = maxHP;
        currentMP = maxMP;

        foreach (GameObject bolt in lightningBolts)
            bolt.SetActive(false);

        Debug.Log($"Thunderbolt pool size: {lightningBolts.Length}");
    }

    private void Update()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        FacePlayer();

        RegenerateMP();

        if (stateQueue.Count == 0 && !isExecutingState)
            DecideNextStates();

        if (!isExecutingState && stateQueue.Count > 0)
        {
            currentState = stateQueue.Dequeue();
            StartCoroutine(ExecuteState(currentState));
        }
    }

    private void FacePlayer()
    {
        float directionToPlayer = player.position.x - king.position.x;
        Vector3 scale = king.localScale;
        scale.x = Mathf.Sign(directionToPlayer) * Mathf.Abs(scale.x); // face opposite
        king.localScale = scale;
     }

    private void DecideNextStates()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        if (currentMP < 15f)
        {
            inLowMPMode = true;
            KingMovement.stopDistance = 5f;
            stateQueue.Enqueue(State.Chase);
            stateQueue.Enqueue(State.MeleeAttack);
            return;
        }

        if (dist <= meleeRange)
        {
            stateQueue.Enqueue(Random.value < 0.7f ? State.MeleeAttack : State.MagicAttack);
        }
        else if (dist <= magicRange)
        {
            if (currentHP > 70f && currentMP > 70f)
                stateQueue.Enqueue(State.MagicAttack);
            else
                stateQueue.Enqueue(State.MagicAttack);
        }
        else
        {
            stateQueue.Enqueue(State.Chase);
        }
    }

    private IEnumerator ExecuteState(State state)
    {
        isExecutingState = true;

        switch (state)
        {
            case State.MeleeAttack:
                Attack();
                Attack2();
                yield return new WaitForSeconds(2f);
                break;

            case State.MagicAttack:
                switch (random.Next(1, 4))
                {
                    case 1: Attack3(); break;
                    case 2: yield return StartCoroutine(DelayedAnimationTrigger(0.9f)); break;
                    case 3: anim.SetTrigger("specialAttack"); SpawnSpecialAttack(); break;
                }
                yield return new WaitForSeconds(0.8f);
                break;

            case State.MagicFury:
                Attack3(); Attack3();
                yield return new WaitForSeconds(1.5f);
                break;

            case State.Chase:
                playerMovement.ChaseEnemy(player);
                yield return new WaitForSeconds(2f);
                break;

            case State.Retreat:
                playerMovement.KeepDistance(player);
                yield return new WaitForSeconds(2f);
                break;

            case State.Idle:
                yield return new WaitForSeconds(2f);
                break;
        }

        isExecutingState = false;
    }
    //write code to increment currentHP
    public void incHP(){
        currentHP += 25;
        enemy_bar.SetHealth(currentHP,maxHP);
    }
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        anim.SetTrigger("takeDamage");
        enemy_bar.SetHealth(currentHP, maxHP);

        if (currentHP <= 0f)
        {
            anim.SetTrigger("Death");
        }
    }

    private void RegenerateMP()
    {
        if (currentMP < maxMP)
        {
            currentMP += mpRegenRate * Time.deltaTime;
            currentMP = Mathf.Min(currentMP, maxMP);
        }
    }
    private void Deactivate(){
        gameObject.SetActive(false);
    }
    private IEnumerator DelayedAnimationTrigger(float delay)
    {
        if (currentMP < 35f) yield break;

        AudioManager.Instance.PlaySFX(fsx1);
        currentMP -= 40f;
        yield return new WaitForSeconds(delay);
        anim.SetTrigger("lightningAttack");
    }

    public void SpawnLightning()
    {
        if (enemyFirePoints.Length == 0 || lightningBolts.Length == 0)
        {
            Debug.LogError("Lightning setup is missing firepoints or bolts!");
            return;
        }

        foreach (Transform point in enemyFirePoints)
        {
            if (point == null) continue;

            GameObject bolt = GetInactiveLightning();
            if (bolt == null) continue;

            bolt.transform.position = point.position;
            bolt.SetActive(true);

            GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().TakeDamage(50);
            GameObject.FindAnyObjectByType<ScreenFlash>()?.FlashScreen();

            StartCoroutine(DisableAfterStrike(bolt, 0.25f));
        }

        AudioManager.Instance.PlaySFX(fsx2);
    }

    private IEnumerator DisableAfterStrike(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    private GameObject GetInactiveLightning() => GetInactiveFromPool(lightningBolts);
    private GameObject GetInactiveProjectile(GameObject[] pool) => GetInactiveFromPool(pool);

    private GameObject GetInactiveFromPool(GameObject[] pool)
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy) return obj;
        }
        Debug.LogWarning("No inactive objects in pool. Consider increasing size.");
        return null;
    }

    private void Attack() => anim.SetTrigger("attack");

    private void Attack2()
    {
        anim.SetTrigger("attack2");
        GameObject slash = GetInactiveProjectile(slashes);
        if (slash == null) return;

        float dir = Mathf.Sign(transform.localScale.x);
        slash.transform.position = firePoint.position;
        slash.transform.localScale = new Vector3(Mathf.Abs(slash.transform.localScale.x) * dir, slash.transform.localScale.y, slash.transform.localScale.z);
        slash.SetActive(true);
        slash.GetComponent<Projectile>().SetDirection(dir);
    }

    private void Attack3()
    {
        if (currentMP < 20f) return;
        currentMP -= 20f;
        anim.SetTrigger("magicAttack");
    }

    public void SpawnFireball()
    {
        GameObject fireball = GetInactiveProjectile(fireballs);
        if (fireball == null) return;

        float dir = Mathf.Sign(transform.localScale.x);
        fireball.transform.position = firePoint.position;
        fireball.transform.localScale = new Vector3(Mathf.Abs(fireball.transform.localScale.x) * dir, fireball.transform.localScale.y, fireball.transform.localScale.z);
        fireball.SetActive(true);
        fireball.GetComponent<FireBall_Projectile>().SetDirection(dir);
    }

    public void SpawnSpecialAttack()
    {
        if (currentMP < 30f) return;
        currentMP -= 30f;

        GameObject beam = GetInactiveProjectile(specialAttack);
        if (beam == null) return;

        beam.transform.position = firePoint.position;
        beam.SetActive(true);

        SpecialBeam beamScript = beam.GetComponent<SpecialBeam>();
        beamScript.Initialize(firePoint);
        beamScript.SetDirection(Mathf.Sign(transform.localScale.x));

        StartCoroutine(DisableAfterStrike(beam, 1.0f));
    }

    public void TriggerLightning() => SpawnLightning();
}