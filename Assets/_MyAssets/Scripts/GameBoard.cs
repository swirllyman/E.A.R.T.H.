using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameBoard : MonoBehaviour
{
    public static GameBoard Instance { get; private set; }
    public GameObject overviewCam;
    public GameObject menuScreen;
    public TMP_Text currentResourcesText;
    public TMP_Text spentResourcesText;
    public TMP_Text turnText;
    public Spinner spinner;
    public Player player;
    public BoardSquare[] boardSquares;
    

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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CreateNewBoard();
    }

    [ContextMenu("Start New Game")]
    public void StartNewGameDefault()
    {
        menuScreen.SetActive(false);
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
            //Game Over Here
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
}
