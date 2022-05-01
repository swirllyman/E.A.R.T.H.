using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameBoard : MonoBehaviour
{
    public static GameBoard Instance { get; private set; }
    public static int CurrentResources { get => Instance.currentResources;}
    public GameObject overviewCam;
    public GameObject menuScreen;
    public GameObject gameFinishedEffect;
    public GameOverUI gameOverUI;
    public TMP_Text currentResourcesText;
    public TMP_Text spentResourcesText;
    public TMP_Text turnText;
    public Spinner spinner;
    public Player player;
    public BoardSquaresSet[] boardSquares;
    public bool randomizeBoard = false;

    internal bool won = false;
    internal int finalScore;

    int turnCost = 1;
    int currentTurn = 1;
    int maxResources = 30;
    int currentResources;
    int spentResources;

    [ContextMenu("Shuffle Squares")]
    public void CreateNewBoard()
    {
        bool prevWasChoice = false;
        SetSquareSetActive(0);
        foreach (BoardSquare square in boardSquares[0].boardSquares)
        {
            int randomRoll = Random.Range(0, 3);
            if(prevWasChoice)
                randomRoll = Random.Range(1, 3);
            square.SetSquareType(randomRoll);
            if (randomRoll == 0)
                prevWasChoice = true;
            else
                prevWasChoice = false;
        }
    }

    public void SetSquareSetActive(int squareSet)
    {
        for (int i = 0; i < boardSquares.Length; i++)
        {
            boardSquares[i].setObject.SetActive(i == squareSet);
            boardSquares[i].upgradeAreas.SetActive(false);
        }
    }

    public void ShowUpgradeAreas()
    {
        boardSquares[GameManager.difficulty].upgradeAreas.SetActive(true);
    }

    private void Awake()
    {
        Instance = this;
        finalScore = 0;
        menuScreen.SetActive(true);
        gameOverUI.SetupGameOverUI();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {
        if(randomizeBoard)
            CreateNewBoard();

        SetSquareSetActive(Random.Range(1, 4));
    }

    public void PlayGameFinished()
    {
        gameFinishedEffect.SetActive(true);
        gameOverUI.gameOverAudioSource.PlayOneShot(gameOverUI.gameOverTriggeredAudio);
    }

    [ContextMenu("Start New Game")]
    public void StartNewGameDefault()
    {
        menuScreen.SetActive(false);
        GameManager.difficulty = 0;
        SetSquareSetActive(GameManager.difficulty);
        SelectCharacter();

        CharacterSelected();
    }

    public void StartNewGame(int diff)
    {
        menuScreen.SetActive(false);
        GameManager.difficulty = diff;
        SetSquareSetActive(GameManager.difficulty);
        SelectCharacter();

        CharacterSelected();
    }


    void SelectCharacter()
    {

    }

    void CharacterSelected()
    {
        overviewCam.SetActive(false);
        ShowUpgradeAreas();
        currentResources = GameManager.difficulty == (0 | 1) ? 20 : (GameManager.difficulty == 2 ? 15 : 10);
        currentResourcesText.text = "Earth Resources: <color=green>" + currentResources + "/"+maxResources;
        spentResourcesText.text = "Spent Resources: <color=red>" + spentResources;
        turnText.text = "Turn Cost Total: <color=red>" + turnCost.ToString() + "</color> -- Current Turn: " + currentTurn.ToString();
        StartCoroutine(WheelSpin(2.0f));
    }

    IEnumerator WheelSpin(float waitTime = .5f)
    {
        yield return new WaitForSeconds(waitTime);
        spinner.ShowSpinner();
        while (spinner.inUse)
        {
            yield return null;
        }
        player.PlayMoveSound();
        yield return new WaitForSeconds(.5f);
        spinner.HideSpinner();
        yield return new WaitForSeconds(.35f);
        spinner.visuals.SetActive(false);

        MovePlayer(Mathf.RoundToInt(spinner.currentValue * GameManager.spinMultiplier));
    }

    public void MovePlayer(int distance)
    {
        player.MoveSpaces(distance);
    }

    public void NextTurn()
    {
        currentTurn++;
        turnCost += GameManager.turnCost;
        turnText.text = "Turn Cost Total: <color=red>" + turnCost.ToString() + "</color> -- Current Turn: " + currentTurn.ToString();
        //if (currentResources > 0)
        //{
        //    StartCoroutine(WheelSpin());
        //}
        //else
        //{
        //    LoseGame();
        //}
        StartCoroutine(WheelSpin());
    }

    public int GetCurrentResources()
    {
        return currentResources;
    }

    public void AddCurrentResources(int amount)
    {
        currentResources = Mathf.Clamp(currentResources + amount, 0, maxResources);
        UpdateResourceText();
    }

    void UpdateResourceText()
    {
        currentResourcesText.text = "Earth Resources <color=orange>(E.R.)</color>: " + (currentResources >= 0 ? "<color=green>" : "<color=red>") + currentResources + "/" + maxResources;
    }

    public void RemoveCurrentResources(int amount)
    {
        currentResources = Mathf.Clamp(currentResources - amount, currentResources - amount, maxResources);
        UpdateResourceText();
        spentResources += amount;
        spentResourcesText.text = "Spent Resources: <color=red>" + spentResources;
    }

    public void AddMaxResources(int amount)
    {
        maxResources += amount;
        UpdateResourceText();
    }

    public void WinGame()
    {
        StartCoroutine(ScoreRecapRoutine());
    }

    public void LoseGame()
    {
        StartCoroutine(ScoreRecapRoutine());
    }

    IEnumerator ScoreRecapRoutine()
    {
        gameOverUI.endOfGameScreen.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        gameOverUI.scoreRecap.SetActive(true);
        gameOverUI.earthResourcesText.text = currentResources.ToString();
        gameOverUI.gameOverAudioSource.PlayOneShot(gameOverUI.textAppearClip);
        yield return new WaitForSeconds(1.0f);
        gameOverUI.gameOverAudioSource.PlayOneShot(gameOverUI.textAppearClip);
        gameOverUI.spentText.text = spentResources.ToString();
        yield return new WaitForSeconds(1.0f);
        gameOverUI.gameOverAudioSource.PlayOneShot(gameOverUI.textAppearClip);
        gameOverUI.turnsText.text = currentTurn.ToString();
        yield return new WaitForSeconds(1.0f);

        int totalScore = currentResources - spentResources - turnCost;
        finalScore = totalScore;
        won = totalScore >= 0;
        string colorScore = won ? "<color=green>" + totalScore + "</color>" : "<color=red>" + totalScore + "</color>";
        gameOverUI.totalScoreText.text = colorScore;
        gameOverUI.gameOverAudioSource.PlayOneShot(gameOverUI.scoreAppearClip);
        yield return new WaitForSeconds(.5f);
        gameOverUI.nextButtonObject.SetActive(true);
    }

    public void ShowGameOverText()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        gameOverUI.scoreRecap.SetActive(false);
        gameOverUI.gameEndedScreen.SetActive(true);

        if (won)
        {
            gameOverUI.gameOverAudioSource.PlayOneShot(gameOverUI.winClip);
            gameOverUI.winText.SetActive(true);
        }
        else
        {
            gameOverUI.gameOverAudioSource.PlayOneShot(gameOverUI.loseClip);
            gameOverUI.lostText.SetActive(true);
        }
        yield return new WaitForSeconds(1.0f);
        gameOverUI.replayButtonObject.SetActive(true);
    }

    public void SetupInitials()
    {
        gameOverUI.gameEndedScreen.SetActive(false);
        gameOverUI.nameEntryObject.SetActive(true);
    }
}

[System.Serializable]
public class GameOverUI
{
    public GameObject nameEntryObject;

    public GameObject endOfGameScreen;
    public GameObject scoreRecap;
    public GameObject gameEndedScreen;
    public GameObject lostText;
    public GameObject winText;
    public GameObject replayButtonObject;
    public GameObject nextButtonObject;
    public AudioSource gameOverAudioSource;
    public AudioClip gameOverTriggeredAudio;
    public AudioClip textAppearClip;
    public AudioClip scoreAppearClip;
    public AudioClip winClip;
    public AudioClip loseClip;

    public TMP_Text earthResourcesText;
    public TMP_Text spentText;
    public TMP_Text turnsText;
    public TMP_Text totalScoreText;

    public void SetupGameOverUI()
    {
        endOfGameScreen.SetActive(false);
        lostText.SetActive(false);
        winText.SetActive(false);
        replayButtonObject.SetActive(false);
        gameEndedScreen.SetActive(false);
        nextButtonObject.SetActive(false);
        nameEntryObject.SetActive(false);
        earthResourcesText.text = "";
        spentText.text = "";
        turnsText.text = "";
        totalScoreText.text = "";
    }
}

[System.Serializable]
public struct BoardSquaresSet
{
    public BoardSquare[] boardSquares;
    public GameObject setObject;
    public GameObject upgradeAreas;
}