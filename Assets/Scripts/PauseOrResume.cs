using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PauseOrResume : MonoBehaviour {

    public AudioMixer am;
    public GameObject pauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause")) {
            Pause();
        }
    }
    
    public void QuitGame() {
        Application.Quit();
    }
    
    private void Pause() {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume() {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void setMainVolume(float vol) {
        am.SetFloat("MainVol", vol);
    }
    
}
