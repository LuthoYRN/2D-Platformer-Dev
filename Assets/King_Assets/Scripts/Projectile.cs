using UnityEngine;

public class Projectile : MonoBehaviour
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
    private void Update()
    {
        if (hit) return;

        float movementSpeed = speed * Time.deltaTime * direction;
        // Move projectile normally if no wall is detected ahead
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > 5) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {    
            Debug.Log("hit");
            hit = true;
            boxCollider.enabled = false;
            if (collision.CompareTag("Player"))collision.gameObject.GetComponent<Health>().TakeDamage(20);
            Explode();
            return;
        }
    }
    private void LateUpdate()
    {
        transform.localScale = new Vector3(Mathf.Abs(originalScale.x) * direction, originalScale.y, originalScale.z);
    }
 
    private void Explode()
    {
        transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
        anim.SetTrigger("explode");
        // Delay resetting scale and disabling the object
        Invoke(nameof(ResetScale), 0.5f);
        Invoke(nameof(Deactivate), 0.5f);
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

    private void ResetScale()
    {
        transform.localScale = originalScale; // Reset the scale after explosion
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
