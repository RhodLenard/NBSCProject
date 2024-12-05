using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // This method is called when the "Start" button is clicked
    public void OnStartButtonClick()
    {
        // Load the task management scene
        SceneManager.LoadScene("TaskManagerScene");
    }

    // This method is called when the "Quit" button is clicked
    public void OnQuitButtonClick()
    {
        Debug.Log("Quit button clicked. Exiting the application...");


        // Quit the application
        Application.Quit();
    }

    public void OnBackToMainMenuButtonClick()
    {
        // Load the Main Menu scene
        SceneManager.LoadScene("MainMenu");
    }
}