using UnityEngine;
using System.Collections;

public class ScreenFlash : MonoBehaviour
{
    private Camera cam;
    private Color originalColor;

    private void Start()
    {
        cam = Camera.main;
        originalColor = cam.backgroundColor; // Save the original background color
    }

    public void FlashScreen()
    {
        StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()
    {
        cam.backgroundColor = Color.white; // Turn screen white
        yield return new WaitForSeconds(0.1f); // Keep it for 0.1 seconds (adjust as needed)
        cam.backgroundColor = originalColor; // Revert back
    }
}
