using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeArea : MonoBehaviour
{
    public GameObject upgradeUI;
    public SubTrigger subTrigger;
    public SpriteRenderer myRend;
    public Upgrade_SO[] options;
    public UpgradeOption[] optionUI;

    Upgrade_SO selectedUpgrade;
    // Start is called before the first frame update
    void Awake()
    {
        upgradeUI.SetActive(false);
        subTrigger.onTrigger += OnSubTrigger;
    }

    private void OnSubTrigger(Collider2D other, bool entered)
    {
        if (entered)
        {
            if (other.transform.CompareTag("Player"))
            {
                other.GetComponent<Player>().SetUpgradeReady(this);
            }
        }
    }

    public void SelectUpgrade(int upgradeID)
    {
        selectedUpgrade = options[upgradeID];
        myRend.sprite = selectedUpgrade.upgradeSprite;
        UseUpgrade();
    }

    public void ShowUpgradeScreen()
    {
        upgradeUI.SetActive(true);
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
                GameManager.renewableGain = 4;
                break;
            case "Coal Upgrade":
                GameManager.fossilFuelCost = 2;
                break;
        }
        upgradeUI.SetActive(false);
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