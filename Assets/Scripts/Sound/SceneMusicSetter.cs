using UnityEngine;
public class SceneMusicSetter : MonoBehaviour
{
    public AudioClip sceneMusic;

    void Start()
    {
        if (AudioManager.Instance != null && sceneMusic != null)
        {
            AudioManager.Instance.ChangeMusic(sceneMusic);
        }
    }
}
