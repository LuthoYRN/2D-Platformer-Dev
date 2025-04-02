using UnityEngine;

public class PhoenixFlight : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float upwardForce = 0.1f;
    [SerializeField] private float maxHeight = 0.5f;
    [SerializeField] private Transform enemy; //The height of the enemy
    private PhoenixController phoenix;


    void Start()
    {
        phoenix = GetComponent<PhoenixController>();
    }



    private void Update()
    {
        //If it's not alive do nothing to the ashes please
        if (!phoenix.IsAlive()) { return; }

        if (rb.position.y < maxHeight)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // reset vertical speed
            rb.AddForce(Vector2.up * upwardForce, ForceMode2D.Impulse);
        }
        else
        {
            // STOP GOING UP
            Vector2 v = rb.linearVelocity;
            v.y = 0f;
            rb.linearVelocity = v;

            // Optional: freeze vertical movement
            rb.gravityScale = 0f;

            Debug.LogError("Clamped Height");
            Debug.LogError("Y Position: " + rb.position.y);
        }
    }

}
