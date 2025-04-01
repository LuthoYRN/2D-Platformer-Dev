using UnityEngine;

public class Powerup : MonoBehaviour
{
 
   private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Animator anim = collision.GetComponent<Animator>();
        PlayerAttack attack = collision.GetComponent<PlayerAttack>();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.powerup);

        if (CompareTag("PowerUpF"))
        {
            anim.SetTrigger("fire_up");
            attack.UnlockAttack("F");
        }
        else if (CompareTag("PowerUpG"))
        {
            anim.SetTrigger("levitate_up");
            attack.UnlockAttack("G");
        }
        else if (CompareTag("PowerUpI"))
        {
            anim.SetTrigger("ice_up");
            attack.UnlockAttack("I");
        }

        gameObject.SetActive(false);
    }
}
