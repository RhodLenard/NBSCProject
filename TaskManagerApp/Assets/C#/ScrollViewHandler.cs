using UnityEngine;
using UnityEngine.UI;

public class ScrollViewHandler : MonoBehaviour
{
    public GameObject scrollViewToShow;  // Reference to the ScrollView you want to show
    public GameObject panel;             // Reference to the Task Manager Panel (which is named "Panel")
    public GameObject backButton;        // Reference to the Back button

    // Start is called before the first frame update
    void Start()
    {
        // Add a listener to the View button click event
        Button viewButton = GetComponent<Button>();
        if (viewButton != null)
        {
            viewButton.onClick.AddListener(OnViewButtonClick);
        }

        // Add a listener to the Back button click event
        if (backButton != null)
        {
            Button backBtn = backButton.GetComponent<Button>();
            backBtn.onClick.AddListener(OnBackButtonClick);
        }

        // Initially hide the ScrollView and make the Task Manager (Panel) visible
        scrollViewToShow.SetActive(false); // Hide the ScrollView at the start
        panel.SetActive(true);             // Ensure the Task Manager Panel is visible at the start
    }

    // Method that is called when the View button is clicked
    void OnViewButtonClick()
    {
        // Hide the Task Manager Panel and show the ScrollView
        panel.SetActive(false);  // Hide the Task Manager Panel (Panel)
        scrollViewToShow.SetActive(true);  // Show the ScrollView
    }

    // Method that is called when the Back button is clicked
    void OnBackButtonClick()
    {
        // Show the Task Manager Panel and hide the ScrollView
        panel.SetActive(true);   // Show the Task Manager Panel (Panel)
        scrollViewToShow.SetActive(false);  // Hide the ScrollView
    }
}
