using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
public class ChoiceManager : MonoBehaviour
{
    public GameObject choiceUI;
    public GameObject finishedChoiceButton;
    public Player player;
    public TMP_Text choiceNameText;
    public TMP_Text choiceDescriptionText;
    public TMP_Text choiceAnswerText;
    public TMP_Text responseText;
    public Image bgImage;
    public Color correctColor;
    public Color incorrectColor;
    public Button trueButton;
    public Button falseButton;
    [Header("Audio")]
    public AudioSource aSource;
    public AudioClip correctClip;
    public AudioClip incorrectClip;

    public Choice_SO[] choices;
    Choice_SO currentChoice;
    List<Choice_SO> availableChoices;
    bool isCorrect = false;

    private void Start()
    {
        availableChoices = new List<Choice_SO>(choices);
        choiceUI.SetActive(false);
        //bgImage.color = correctColor;
        //bgImage.DOBlendableColor(incorrectColor, 1.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void SetupRandomChoice()
    {
        choiceUI.SetActive(true);
        finishedChoiceButton.SetActive(false);
        isCorrect = false;
        trueButton.interactable = true;
        falseButton.interactable = true;
        responseText.text = "";
        choiceAnswerText.enabled = false;
        bgImage.color = correctColor;
        bgImage.DOBlendableColor(incorrectColor, 1.5f).SetLoops(-1, LoopType.Yoyo);

        int choiceIndex = Random.Range(0, availableChoices.Count);
        currentChoice = availableChoices[choiceIndex];
        availableChoices.RemoveAt(choiceIndex);
        if(availableChoices.Count == 0)
        {
            availableChoices = new List<Choice_SO>(choices);
        }

        choiceNameText.text = currentChoice.choiceName;
        choiceDescriptionText.text = currentChoice.choiceText;
        choiceAnswerText.text = currentChoice.choiceAnswerText;
    }

    public void AnswerChoice(bool clickedTrue)
    {
        if(clickedTrue == currentChoice.isTrue)
        {
            Correct();
        }
        else
        {
            Incorrect();
        }
        choiceAnswerText.enabled = true;
        trueButton.interactable = false;
        falseButton.interactable = false;

        StartCoroutine(ChoiceFinishRoutine());
    }

    [ContextMenu("Incorrect")]
    public void Incorrect()
    {
        aSource.PlayOneShot(incorrectClip);
        DOTween.Kill(bgImage);
        bgImage.DOBlendableColor(incorrectColor, .5f);
        responseText.text = "Wrong";
        responseText.color = Color.red;
    }

    [ContextMenu("Correct")]
    public void Correct()
    {
        aSource.PlayOneShot(correctClip);
        isCorrect = true;
        DOTween.Kill(bgImage);
        bgImage.DOBlendableColor(correctColor, .5f);
        responseText.text = "Correct!";
        responseText.color = Color.green;
    }

    IEnumerator ChoiceFinishRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        finishedChoiceButton.SetActive(true);
    }

    public void FinishChoice()
    {
        choiceUI.SetActive(false);
        if (isCorrect)
        {
            player.AddScore(5);
        }
        else
        {
            player.RemoveScore(5);
        }
        player.FinishedSquare();
    }
}
