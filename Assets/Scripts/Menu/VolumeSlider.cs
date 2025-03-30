using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Scrollbar scrollbar;

    private void Awake()
    {
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        scrollbar.value = savedVolume;
        scrollbar.onValueChanged.AddListener(SetVolume);
    }

    private void SetVolume(float value)
    {
        Debug.Log("Slider changed to: " + value);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateVolume(value);
        }
    }


    private void Update()
    {
        SetVolume(scrollbar.value);
    }


}
