using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    [SerializeField] public float speed;
    [SerializeField] public float jumpHeight; 
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public GameObject map;
    private float horizontalInput;
    public bool facingRight = true;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        GameObject start = GameObject.FindGameObjectWithTag("Start");
        transform.position = start.transform.position;
    } 

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // Movement

        // Flip the character when changing direction
        if (horizontalInput < 0f && facingRight)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            facingRight = false;
        }
        else if (horizontalInput > 0f && !facingRight)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            facingRight = true;
        }

        // Jump logic
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            Jump();
        }

        // Running animation
        anim.SetFloat("run", Mathf.Abs(horizontalInput));

        // Fall detection (only trigger falling when moving downward)
        if (IsGrounded())
        {
            anim.SetBool("jump", false);
            anim.SetBool("fall", false);
        }
        else if (body.linearVelocityY > 0.1f)
        {
            anim.SetBool("jump", true);
            anim.SetBool("fall", false);
        }
        else if (body.linearVelocityY < -0.1f)
        {
            anim.SetBool("jump", false);
            anim.SetBool("fall", true);
        }
        else
        {
            // At peak or hit ceiling
            anim.SetBool("jump", false);
            anim.SetBool("fall", true);
        }

    }

    void FixedUpdate()
    {
        // Move the character
        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocityY);
    }

    void Jump()
    {
        anim.SetBool("jump", true);
        body.linearVelocity = new Vector2(body.linearVelocityX, jumpHeight);
    }

    private bool IsGrounded(){
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size,0,Vector2.down,0.15f,groundLayer);
        return raycastHit.collider!=null;
    }
    public bool CanAttack(){
        return horizontalInput==0 && IsGrounded();
    }

    public bool GetDirection()
    {
        return facingRight;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("End"))
        {
            bool anyEnemyAlive = false;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {       
                if (enemy.activeInHierarchy)
                    anyEnemyAlive = true;
                    break;
            }
            if (anyEnemyAlive){
                Debug.Log("Clear all enemies first!");
            }
            else{
                endLevel();
            }
        }
    }

    //When player reaches the end of the level, this needs to be called and the levelsCompleted needs to increase by 1
    public void endLevel()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().Disable();
        int currentActiveScene = SceneManager.GetActiveScene().buildIndex;

        if (currentActiveScene == 6) {
            PlayerPrefs.SetInt("LastCompletedLevel", 0);
            PlayerPrefs.Save();
            Debug.Log("Loading main menu");
            SceneManager.LoadScene("Menu");
        } else
        {
            PlayerPrefs.SetInt("LastCompletedLevel", SceneManager.GetActiveScene().buildIndex);
            PlayerPrefs.Save();
            map.SetActive(true);
        }
      
    }
    public void Disable(){
        enabled = false;
        PlayerAttack playerAttack = this.gameObject.GetComponent<PlayerAttack>();
        playerAttack.enabled = false;
    }
    public void Enable(){
        enabled = true;
        PlayerAttack playerAttack = this.gameObject.GetComponent<PlayerAttack>();
       playerAttack.enabled = true;
        
    } 
    public void EnableMovement()
    {  
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;
        }
        if (GetComponent<PlayerMovement>()!=null)GetComponent<PlayerMovement>().enabled = true;
        if (TryGetComponent<PlayerAttack>(out var attack)) attack.enabled = true;
       
    }

    public void DisableMovement()
    {
        GetComponent<PlayerMovement>().enabled = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static; 
        }
        if (TryGetComponent<PlayerMovement>(out var move)) move.enabled = false;
        if (TryGetComponent<PlayerAttack>(out var attack)) attack.enabled = false;
               
    }

}