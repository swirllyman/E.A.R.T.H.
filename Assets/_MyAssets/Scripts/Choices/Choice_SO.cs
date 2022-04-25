using UnityEngine;

[CreateAssetMenu(fileName = "Choice", menuName = "ScriptableObjects/Choice", order = 1)]
public class Choice_SO : ScriptableObject
{
    public string choiceName;
    [TextArea(3, 5)]
    public string choiceText;
    [TextArea(3, 5)]
    public string choiceAnswerText;
    public bool isTrue = false;
}
