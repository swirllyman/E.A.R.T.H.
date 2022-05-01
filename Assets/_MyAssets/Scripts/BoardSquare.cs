using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum SquareType { Choice, FossilFuel, Renewable, Challenge }
public class BoardSquare : MonoBehaviour
{
    internal SquareType mySquareType;
    public TMP_Text myText;
    [SerializeField] SquareIcon[] squareIcons;

    [SerializeField] SpriteRenderer myRend;

    private void Awake()
    {
        mySquareType = (SquareType)GetSquareType();
    }

    int GetSquareType()
    {
        for (int i = 0; i < squareIcons.Length; i++)
        {
            if(myRend.sprite == squareIcons[i].iconSprite)
            {
                return i;
            }
        }
        return -1;
    }

    internal void SetSquareType(int iconID)
    {
        myRend.sprite = squareIcons[iconID].iconSprite;
        myText.text = squareIcons[iconID].iconName;
        myText.color = squareIcons[iconID].textColor;
        mySquareType = (SquareType)iconID;
    }

    [ContextMenu("SetToChoice")]
    public void SetToChoice()
    {
        SetSquareType(0);
    }

    [ContextMenu("SetToFossil")]
    public void SetToFossil()
    {
        SetSquareType(1);
    }
    [ContextMenu("SetToRenewable")]
    public void SetToRenewable()
    {
        SetSquareType(2);
    }
}

[System.Serializable]
public struct SquareIcon
{
    public string iconName;
    public Sprite iconSprite;
    [Tooltip("Sets the Text Color")] public Color textColor;
}