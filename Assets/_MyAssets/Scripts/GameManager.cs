using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using LootLocker.Requests;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static int difficulty = 1;
    public static int fossilFuelCost = 5;
    public static int renewableGain = 3;
    public static int turnCost = 1;
    public static float spinMultiplier = 1;

    public GameObject[] uiPanels;
    public SimpleLeaderboard[] leaderboards;
    public TMP_InputField initialsInput;
    public GameObject startPanel;
    public GameObject mainPanel;
    public GameObject submitScoresButton;
    public AudioSource gameMusicAudioSource;
    public AudioClip gameMusicClip;
    public AudioClip gameChoiceClip;

    bool justStart = true;
    private void Awake()
    {
        Instance = this;
        justStart = true;
        mainPanel.SetActive(false);
        startPanel.SetActive(true);
        startPanel.transform.DOScale(1.1f, 1.25f).SetLoops(-1, LoopType.Yoyo);
        difficulty = 1;
        fossilFuelCost = 3;
        renewableGain = 3;
        spinMultiplier = 1;
    }

    private void Start()
    {
        //LootLockerSDKManager.
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Update()
    {
        if (Input.anyKey && justStart)
        {
            justStart = false;
            mainPanel.SetActive(true);
            startPanel.SetActive(false);
        }
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

    public void SubmitScores()
    {
        submitScoresButton.SetActive(false);
        if (difficulty > 0)
        {





            LootLockerSDKManager.SetPlayerName(initialsInput.text, (response) =>
            {
                if (response.success)
                {
                    Debug.Log("Successfully set player name: " + response.name);
                    LootLockerSDKManager.SubmitScore(initialsInput.text, GameBoard.Instance.finalScore, leaderboards[difficulty - 1].leaderboardID, (response) =>
                    {
                        if (response.statusCode == 200)
                        {
                            Debug.Log("Successful");
                        }
                        else
                        {
                            Debug.Log("failed: " + response.Error);
                        }
                        RestartGame();
                    });
                }
                else
                {
                    Debug.Log("Error setting player name");
                    RestartGame();
                }
            });
        }
        else
        {
            RestartGame();
        }
    }

    //Called from UI Buttons
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
