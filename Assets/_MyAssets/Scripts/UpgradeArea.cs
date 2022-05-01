using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UpgradeArea : MonoBehaviour
{
    public GameObject upgradeUI;
    public GameObject loopingParticles;
    public GameObject explosionObject;
    public GameObject vCam;
    public SubTrigger subTrigger;
    public SpriteRenderer myRend;
    public SpriteRenderer outlineRend;
    public SpriteRenderer buildAreaOutline;
    public Upgrade_SO[] options;
    public UpgradeOption[] optionUI;
    public Color upgradeColor;
    public ParticleSystem[] fadeParticles;
    public ParticleSystem finishParticles;
    public SpriteRenderer checkPointRend;
    public GameObject upgradeReadyUiObject;
    public AudioSource aSource;
    public AudioClip buildClip;
    public AudioClip buildFinishClip;

    Upgrade_SO selectedUpgrade;
    Color outlineStartColor;
    // Start is called before the first frame update
    void Awake()
    {
        upgradeUI.SetActive(false);
        outlineStartColor = outlineRend.color;
        upgradeReadyUiObject.SetActive(false);
        explosionObject.SetActive(false);
        subTrigger.onTrigger += OnSubTrigger;
        upgradeReadyUiObject.transform.DOScale(upgradeReadyUiObject.transform.localScale * 1.05f, 1.5f).SetLoops(-1);
    }

    private void OnSubTrigger(Collider2D other, bool entered)
    {
        if (entered)
        {
            if (other.transform.CompareTag("Player"))
            {
                other.GetComponent<Player>().SetUpgradeReady(this);
                outlineRend.DOColor(upgradeColor, 1.5f).SetLoops(-1);
                upgradeReadyUiObject.SetActive(true);
                explosionObject.SetActive(true);
                checkPointRend.enabled = false;
                foreach(ParticleSystem p in fadeParticles)
                {
                    p.Play();
                }
                aSource.Play();
                loopingParticles.SetActive(false);
                subTrigger.GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    public void SelectUpgrade(int upgradeID)
    {
        selectedUpgrade = options[upgradeID];
        myRend.sprite = selectedUpgrade.upgradeSprite;
        myRend.enabled = false;
        aSource.PlayOneShot(buildClip);
        DOTween.Kill(outlineRend);
        outlineRend.color = outlineStartColor;
        UseUpgrade();
    }

    public void ShowUpgradeScreen()
    {
        upgradeReadyUiObject.SetActive(false);
        upgradeUI.SetActive(true);
        vCam.SetActive(true);
        for (int i = 0; i < options.Length; i++)
        {
            optionUI[i].headerText.text = options[i].upgradeName;
            optionUI[i].descriptionText.text = options[i].upgradeDescription;
            optionUI[i].optionImage.sprite = options[i].upgradeSprite;
            optionUI[i].upgradeButton.interactable = options[i].upgradeCost <= GameBoard.CurrentResources || options[i].upgradeCost == 0;
        }
    }

    public void UseUpgrade()
    {
        bool endTurn = true;
        int skipAmount = 0;
        switch (selectedUpgrade.upgradeName)
        {
            case "Solar Power":
                GameBoard.Instance.AddMaxResources(15);
                break;

            case "Wind Power":
                GameManager.renewableGain += 2;
                break;

            case "Power Plant":
                GameBoard.Instance.AddMaxResources(30);
                Player.Instance.RemoveScore(10);
                GameManager.turnCost += 1;
                break;

            case "Coal Upgrade":
                GameManager.fossilFuelCost--;
                Player.Instance.RemoveScore(5);
                break;

            case "Electric Cars":
                GameManager.spinMultiplier = 1.5f;
                break;

            case "Gas Cars":
                GameManager.spinMultiplier = 2.0f;
                GameManager.fossilFuelCost += 2;
                break;

            case "Geothermal Power":
                GameBoard.Instance.AddMaxResources(10);
                Player.Instance.AddScore(10);
                break;

            case "Oil Upgrade":
                GameManager.fossilFuelCost = 0;
                GameManager.turnCost += 1;
                break;

            case "Harvest Forest":
                endTurn = false;
                skipAmount = 10;
                Player.Instance.RemoveScore(15);
                break;
        }
        upgradeUI.SetActive(false);
        finishParticles.Play();
        StartCoroutine(UpgradeRoutine(endTurn, skipAmount));
    }

    IEnumerator UpgradeRoutine(bool endTurn, int skipAmount)
    {
        yield return new WaitForSeconds(2.5f);
        myRend.enabled = true;
        aSource.PlayOneShot(buildFinishClip);
        buildAreaOutline.DOColor(upgradeColor, 1.5f);
        yield return new WaitForSeconds(2.0f);
        vCam.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        Player.Instance.upgradeReady = false;
        if (endTurn)
        {
            GameBoard.Instance.NextTurn();
        }
        else
        {
            GameBoard.Instance.MovePlayer(skipAmount);
        }
    }

}

[System.Serializable]
public struct UpgradeOption
{
    public Image optionImage;
    public Button upgradeButton;
    public TMP_Text headerText;
    public TMP_Text descriptionText;
}