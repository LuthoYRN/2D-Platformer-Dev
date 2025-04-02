using UnityEngine;

public class LightningBolt : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke("Deactivate", 1.5f);  // Disappear after 1.5 sec
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

}
