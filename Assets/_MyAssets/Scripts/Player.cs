using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    internal bool upgradeReady = false;
    [SerializeField] ChoiceManager choiceManager;
    [SerializeField] GameBoard board;
    [SerializeField] TMP_Text currentMovementText;
    [SerializeField] TMP_Text scoreFloaterText;
    [SerializeField] AudioSource aSource;
    [SerializeField] AudioClip moveClip;
    [SerializeField] AudioClip[] floaterClips;
    [SerializeField] float moveSpeed = .5f;
    [SerializeField] float waitSpeed = .25f;

    int currentMoveDistance = 0;
    int currentSpaceID = -1;
    bool finishedWithSquare = false;
    UpgradeArea currentUpgradeArea;

    private void Awake()
    {
        currentMovementText.text = "";
        scoreFloaterText.text = "";
        Instance = this;
    }

    public void PlayMoveSound()
    {
        aSource.PlayOneShot(moveClip);
    }

    public void MoveSpaces(int spacesToMove)
    {
        currentMoveDistance = spacesToMove;
        currentMovementText.text = currentMoveDistance.ToString();
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        while(currentMoveDistance > 0)
        {
            currentMoveDistance--;
            currentSpaceID++;
            Vector3 startPos = transform.position;
            for (float i = 0; i < moveSpeed; i += Time.deltaTime)
            {
                float perc = i / moveSpeed;
                transform.position = Vector3.Lerp(startPos, board.boardSquares[currentSpaceID].transform.position, perc);
                yield return null;
            }
            currentMovementText.text = currentMoveDistance.ToString();
            transform.position = board.boardSquares[currentSpaceID].transform.position;

            aSource.Play();
            if (currentMoveDistance > 0)
            {
                yield return new WaitForSeconds(waitSpeed);
            }
        }

        yield return new WaitForSeconds(.5f);
        currentMovementText.text = "";

        float waitTime = GetSquareWaitTime();
        if(waitTime < 0)
        {
            while (!finishedWithSquare)
            {
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(waitTime);
        }

        EndTurn();
    }

    void EndTurn()
    {

        if (upgradeReady)
        {
            currentUpgradeArea.ShowUpgradeScreen();
        }
        else
        {
            board.NextTurn();
        }
    }

    float GetSquareWaitTime()
    {
        finishedWithSquare = false;
        switch (board.boardSquares[currentSpaceID].mySquareType)
        {
            case SquareType.Renewable:
                AddScore(GameManager.renewableGain);
                return 1.0f;
            case SquareType.FossilFuel:
                RemoveScore(GameManager.fossilFuelCost);
                return 1.0f;
            case SquareType.Choice:
                choiceManager.SetupRandomChoice();
                return -1.0f;
            case SquareType.Challenge:
                finishedWithSquare = true;
                return -1.0f;
            default: 
                return 1.0f;
        }
    }

    public void AddScore(int amount)
    {
        board.AddCurrentResources(amount);
        PlayScoreFloater(amount, true);
    }

    public void RemoveScore(int amount)
    {
        board.RemoveCurrentResources(amount);
        PlayScoreFloater(amount, false);
    }

    public void FinishedSquare()
    {
        finishedWithSquare = true;
    }

    void PlayScoreFloater(int score, bool positive)
    {
        scoreFloaterText.text = (positive ? "+" : "-") + score;

        if (positive)
        {
            aSource.PlayOneShot(floaterClips[0]);
            scoreFloaterText.color = Color.green;
            scoreFloaterText.transform.localPosition = new Vector3(0, -35, 0);
            scoreFloaterText.transform.DOLocalMoveY(-15, 1.0f).SetEase(Ease.OutBounce);
        }
        else
        {
            aSource.PlayOneShot(floaterClips[1]);
            scoreFloaterText.color = Color.red;
            scoreFloaterText.transform.localPosition = new Vector3(0, 0, 0);
            scoreFloaterText.transform.DOLocalMoveY(-35, 1.0f).SetEase(Ease.OutBounce);
        }
        StartCoroutine(HideFloater());
    }

    IEnumerator HideFloater()
    {
        yield return new WaitForSeconds(1.5f);
        scoreFloaterText.text = "";
    }

    public void SetUpgradeReady(UpgradeArea areaToUpgrade)
    {
        upgradeReady = true;
        currentUpgradeArea = areaToUpgrade;
    }
}
