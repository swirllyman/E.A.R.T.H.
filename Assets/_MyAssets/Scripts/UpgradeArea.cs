using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UpgradeArea : MonoBehaviour
{
    public GameObject upgradeUI;
    public GameObject vCam;
    public SubTrigger subTrigger;
    public SpriteRenderer myRend;
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
    // Start is called before the first frame update
    void Awake()
    {
        upgradeUI.SetActive(false);
        upgradeReadyUiObject.SetActive(false);
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
                upgradeReadyUiObject.SetActive(true);
                checkPointRend.enabled = false;
                foreach(ParticleSystem p in fadeParticles)
                {
                    p.Play();
                }
                aSource.Play();
            }
        }
    }

    public void SelectUpgrade(int upgradeID)
    {
        selectedUpgrade = options[upgradeID];
        myRend.sprite = selectedUpgrade.upgradeSprite;
        myRend.enabled = false;
        aSource.PlayOneShot(buildClip);
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
        }
    }

    public void UseUpgrade()
    {
        switch (selectedUpgrade.upgradeName)
        {
            case "Solar Power":
                GameBoard.Instance.AddMaxResources(10);
                break;
            case "Wind Power":
                GameManager.renewableGain++;
                break;
            case "Coal Upgrade":
                GameManager.fossilFuelCost--;
                break;
        }
        upgradeUI.SetActive(false);
        finishParticles.Play();
        StartCoroutine(UpgradeRoutine());
    }

    IEnumerator UpgradeRoutine()
    {
        yield return new WaitForSeconds(2.5f);
        myRend.enabled = true;
        aSource.PlayOneShot(buildFinishClip);
        yield return new WaitForSeconds(2.0f);
        vCam.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        Player.Instance.upgradeReady = false;
        GameBoard.Instance.NextTurn();
    }

}

[System.Serializable]
public struct UpgradeOption
{
    public Image optionImage;
    public TMP_Text headerText;
    public TMP_Text descriptionText;
}