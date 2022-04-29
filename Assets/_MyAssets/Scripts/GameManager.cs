using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static int difficulty = 1;
    public static int fossilFuelCost = 3;
    public static int renewableGain = 3;

    public GameObject[] uiPanels;
    public AudioSource gameMusicAudioSource;
    public AudioClip gameMusicClip;
    public AudioClip gameChoiceClip;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public static void ToggleChocie(bool toggle)
    {
        if (toggle)
        {
            Instance.gameMusicAudioSource.clip = Instance.gameChoiceClip;
            Instance.gameMusicAudioSource.Play();
        }
        else
        {
            Instance.gameMusicAudioSource.clip = Instance.gameMusicClip;
            Instance.gameMusicAudioSource.Play();
        }
    }

    public void SelectPanel(int uiPanel)
    {
        for (int i = 0; i < uiPanels.Length; i++)
        {
            uiPanels[i].SetActive(i == uiPanel);
        }
    }

    public void RestartGame()
    {

    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
