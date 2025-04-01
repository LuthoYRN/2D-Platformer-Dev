using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{

    public GameObject mainMenuPanel;
    public GameObject mapPanel;
    public GameObject optionsPanel;
    public VolumeSlider volumeSlider;
    public AudioManager audioManager;
    public GameObject[] mapPieces;
    public void playGame ()
    {
        int lastCompletedLevel = PlayerPrefs.GetInt("LastCompletedLevel", 0);
        Debug.Log("Play button pressed");
        SceneManager.LoadScene(lastCompletedLevel + 1); //Make sure to update this to say +1 whenever a level is finished!!!!!!
    }

    public void restartGame()
    {
        Debug.Log("Resarting game");
        PlayerPrefs.SetInt("LastCompletedLevel", 0);
        PlayerPrefs.Save();
    }
    public void exitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void OpenMap()
    {
        mainMenuPanel.SetActive(false);
        mapPanel.SetActive(true);
        UpdateMap();
    }

    public void CloseMap()
    {
        int lastCompletedLevel = PlayerPrefs.GetInt("LastCompletedLevel", 0);
        mapPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        mapPieces[lastCompletedLevel].SetActive(false);
    }

    void UpdateMap()
    {
        int lastCompletedLevel = PlayerPrefs.GetInt("LastCompletedLevel", 0);
        Debug.Log("Levels completed: " + lastCompletedLevel);
        mapPieces[lastCompletedLevel].SetActive(true);
    }

    public void OpenOptionsMenu()
    {
        optionsPanel.SetActive(true); 
        mainMenuPanel.SetActive(false);
        audioManager.PauseMusic();
        volumeSlider.OnOptionsMenuOpened(); 
    }

    public void CloseOptionsMenu()
    {
        optionsPanel.SetActive(false); 
        mainMenuPanel.SetActive(true);
        audioManager.ResumeMusic();
        volumeSlider.OnOptionsMenuClosed(); 
    }


}
