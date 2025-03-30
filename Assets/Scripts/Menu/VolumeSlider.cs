using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Scrollbar scrollbar;
    public AudioManager audioManager;

    private void Awake()
    {
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        scrollbar.value = savedVolume;
        scrollbar.onValueChanged.AddListener(SetVolume);
    }

    private void SetVolume(float value)
    {
        Debug.Log("Slider changed to: " + value);
        if (audioManager != null)
        {
            audioManager.UpdateVolume(value);
        }
    }


    private void Update()
    {
        SetVolume(scrollbar.value);
    }

    public void OnOptionsMenuOpened()
    {
        if (audioManager != null)
        {
            audioManager.PauseMusic();
        }
    }

    // Call this method when the options menu is closed
    public void OnOptionsMenuClosed()
    {
        if (audioManager != null)
        {
            audioManager.ResumeMusic();
        }
    }
}
