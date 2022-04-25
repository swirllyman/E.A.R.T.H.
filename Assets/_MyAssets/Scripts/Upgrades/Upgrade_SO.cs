using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade", order = 1)]
public class Upgrade_SO : ScriptableObject
{
    public string upgradeName;
    [TextArea(3, 5)]
    public string upgradeDescription;
    public Sprite upgradeSprite;
}
