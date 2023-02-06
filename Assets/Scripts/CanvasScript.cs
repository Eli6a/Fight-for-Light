using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasScript : MonoBehaviour
{

    public AudioManager audioManager;
    public LevelManager levelManager;
    public bool goodEnding;
    public Animator animator;

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        if (LevelManager.currentLevel == LevelManager.mainMenu)
        {
            Play();
        }
        else if (LevelManager.currentLevel == LevelManager.goodEnding)
        {
            audioManager.GetComponent<AudioSource>().clip = audioManager.victoryClip;
            audioManager.GetComponent<AudioSource>().loop = false;
            Play();
        }

        try
        {
            if (goodEnding)
            {
                animator = GetComponentInChildren<Animator>();
            }
         
        }
        catch {

        }
    }

    public void ChangeAudioAkeboshi()
    {
        levelManager.ChangeAudioAkeboshi(audioManager);
    }

    public void SetVolume(float volume)
    {
        audioManager.SetVolume(volume);
    }
    public void Play()
    {
        audioManager.Play();
    }

    public void Pause()
    {
        audioManager.Pause();
    }
    public void UnPause()
    {
        audioManager.UnPause();
    }
    public void Stop()
    {
        audioManager.Stop();
    }

    public void LoadMainMenu()
    {
        levelManager.LoadMainMenu();
    }
    public void LoadNextLevel()
    {
        levelManager.LoadNextLevel();
    }
    public void TryAgain()
    {
        levelManager.TryAgain();
        audioManager.Play();
    }
}
