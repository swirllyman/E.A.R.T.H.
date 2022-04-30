using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameBoard : MonoBehaviour
{
    public static GameBoard Instance { get; private set; }
    public GameObject overviewCam;
    public GameObject menuScreen;
    public GameOverUI gameOverUI;
    public TMP_Text currentResourcesText;
    public TMP_Text spentResourcesText;
    public TMP_Text turnText;
    public Spinner spinner;
    public Player player;
    public BoardSquare[] boardSquares;

    internal bool won = false;

    int currentTurn = 1;
    int maxResources;
    int currentResources;
    int spentResources;

    [ContextMenu("Shuffle Squares")]
    public void CreateNewBoard()
    {
        boardSquares = GetComponentsInChildren<BoardSquare>();
        bool prevWasChoice = false;
        foreach(BoardSquare square in boardSquares)
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

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Awake()
    {
        Instance = this;
        menuScreen.SetActive(true);
        gameOverUI.SetupGameOverUI();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {
        CreateNewBoard();
    }

    [ContextMenu("Start New Game")]
    public void StartNewGameDefault()
    {
        menuScreen.SetActive(false);
        FindObjectOfType<SoundOptions>().optionsButton.SetActive(true);
        GameManager.difficulty = 1;
        SelectCharacter();

        CharacterSelected();
    }

    void SelectCharacter()
    {

    }

    void CharacterSelected()
    {
        overviewCam.SetActive(false);
        currentResources = GameManager.difficulty == 1 ? 20 : (GameManager.difficulty == 2 ? 15 : 10);
        maxResources = currentResources;
        currentResourcesText.text = "Earth Resources: " + currentResources + "/"+maxResources;
        spentResourcesText.text = "Spent Resources: " + spentResources;
        turnText.text = "Turn: " + currentTurn.ToString();
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

        MovePlayer(spinner.currentValue);
    }

    void MovePlayer(int distance)
    {
        player.MoveSpaces(distance);
    }

    public void NextTurn()
    {
        currentTurn++;
        turnText.text = "Turn: "+currentTurn.ToString();
        if (currentResources > 0)
        {
            StartCoroutine(WheelSpin());
        }
        else
        {
            LoseGame();
        }
    }

    public void AddCurrentResources(int amount)
    {
        currentResources = Mathf.Clamp(currentResources + (GameManager.difficulty == 1 ? amount : (GameManager.difficulty == 2 ? amount / 2 : amount / 3)), 0, maxResources);
        currentResourcesText.text = "Earth Resources: " + currentResources + "/" + maxResources;
    }

    public void RemoveCurrentResources(int amount)
    {
        currentResources = Mathf.Clamp(currentResources - amount, 0, maxResources);
        currentResourcesText.text = "Earth Resources: " + currentResources + "/" + maxResources;
        spentResources += amount;
        spentResourcesText.text = "Spent Resources: " + spentResources;
    }

    public void AddMaxResources(int amount)
    {
        maxResources += amount;
        currentResourcesText.text = "Earth Resources: " + currentResources + "/" + maxResources;
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
        yield return new WaitForSeconds(1.0f);
        gameOverUI.spentText.text = spentResources.ToString();
        yield return new WaitForSeconds(1.0f);
        gameOverUI.turnsText.text = currentTurn.ToString();
        yield return new WaitForSeconds(1.0f);

        int totalScore = currentResources - spentResources - currentTurn;
        won = totalScore >= 0;
        string colorScore = won ? "<color=green>" + totalScore + "</color>" : "<color=red>" + totalScore + "</color>";
        gameOverUI.totalScoreText.text = colorScore;

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
            gameOverUI.winText.SetActive(true);
        }
        else
        {
            gameOverUI.lostText.SetActive(true);
        }
        yield return new WaitForSeconds(1.0f);
        gameOverUI.replayButtonObject.SetActive(true);
    }
}

[System.Serializable]
public class GameOverUI
{
    public GameObject endOfGameScreen;
    public GameObject scoreRecap;
    public GameObject gameEndedScreen;
    public GameObject lostText;
    public GameObject winText;
    public GameObject replayButtonObject;
    public GameObject nextButtonObject;

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
        earthResourcesText.text = "";
        spentText.text = "";
        turnsText.text = "";
        totalScoreText.text = "";
    }
}