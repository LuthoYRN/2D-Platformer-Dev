using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign in inspector

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Pressed esc");
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().Enable();
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().Disable();
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale before leaving
        SceneManager.LoadScene("Menu"); // Change "MainMenu" to your actual main menu scene name
    }
}

