using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSound : MonoBehaviour
{
    private Sprite soundOnImage;
    public Sprite soundOffImage;
    public Button button;
    private bool isOn = true;

    public AudioSource audioSource;

    private void Awake()
    {
        // Ensure the audioSource is assigned, either from inspector or found in the scene
        if (audioSource == null)
        {
            audioSource = FindObjectOfType<AudioSource>(); // Try to find the AudioSource if not already assigned
        }

        // Ensure audioSource is found or else log an error
        if (audioSource == null)
        {
            Debug.LogError("AudioSource not found! Please make sure an AudioSource exists in the scene.");
        }

        // Initialize the button image state if not set already
        if (soundOnImage == null)
        {
            soundOnImage = button.image.sprite;
        }

        // Handle the mute state based on PlayerPrefs (or static variable)
        isOn = PlayerPrefs.GetInt("SoundState", 1) == 1; // 1 for sound on, 0 for sound off
        if (audioSource != null)
        {
            audioSource.mute = !isOn; // Mute or unmute based on saved state
        }
        button.image.sprite = isOn ? soundOnImage : soundOffImage;

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Continuously check if audioSource is missing after scene changes
        if (audioSource == null)
        {
            audioSource = FindObjectOfType<AudioSource>(); // Try to find it again
            if (audioSource == null)
            {
                Debug.LogError("AudioSource not found in the scene!");
            }
        }
    }

    public void ButtonClicked()
    {
        // Toggle the mute state
        isOn = !isOn;

        // Update the button image based on the sound state
        button.image.sprite = isOn ? soundOnImage : soundOffImage;

        // Mute or unmute the audioSource
        if (audioSource != null)
        {
            audioSource.mute = !isOn;
        }

        // Save the sound state to PlayerPrefs
        PlayerPrefs.SetInt("SoundState", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}
