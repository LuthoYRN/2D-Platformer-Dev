using TMPro.Examples;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
   private Transform currentCheckpoint; 
   private Health playerHealth;
   private AudioManager audioManager;
    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        currentCheckpoint = GameObject.FindGameObjectWithTag("Start").transform;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("checkpoint"))
        {
            currentCheckpoint = collision.transform;
            audioManager.PlaySFX(audioManager.checkpoint);
            collision.GetComponent<Collider2D>().enabled = false;
            collision.GetComponent<Animator>().SetTrigger("appear");
        }
    }
    public void P_Respawn(){
        transform.position =currentCheckpoint.position;
        playerHealth.Respawn();
    }
}
