using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SimpleLeaderboard : MonoBehaviour
{
    public int leaderboardID = 2725;
    public TMP_Text[] entryTexts;

    // Start is called before the first frame update
    void Start()
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (!response.success)
            {
                Debug.Log("Error starting LootLocker session");
                return;
            }

            Debug.Log("LootLocker Session Started");
            GetScores();
        });
    }

    [ContextMenu("Submit Score")]
    public void AddScore()
    {
        SetNameThenScore(5, "BDE");
    }

    void SetNameThenScore(int score, string newName)
    {
        LootLockerSDKManager.SetPlayerName(newName, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully set player name: "+response.name);
                SubmitScore(score, response.name);
            }
            else
            {
                Debug.Log("Error setting player name");
            }
        });
    }

    public void SubmitScore(int score, string memberID)
    {
        LootLockerSDKManager.SubmitScore(memberID, score, leaderboardID, (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful");
            }
            else
            {
                Debug.Log("failed: " + response.Error);
            }
        });
    }

    [ContextMenu("Get Scores")]
    public void GetScores()
    {
        int count = 10;

        LootLockerSDKManager.GetScoreList(leaderboardID, count, (response) =>
        {
            if (response.statusCode == 200)
            {
                LootLockerLeaderboardMember[] members = response.items;
                for (int i = 0; i < members.Length; i++)
                {
                    //Debug.Log(members[i].player.id + ", " + members[i].player.name + ", " + members[i].player.public_uid);
                    entryTexts[i].text = "Rank " + members[i].rank + " - " + members[i].player.name + " - "+ (members[i].score >= 0 ? "<color=green>" : "<color=red>") + members[i].score;
                }
                for(int i = members.Length; i < entryTexts.Length; i++)
                {
                    entryTexts[i].text = "";
                }
            }
            else
            {
                Debug.Log("failed: " + response.Error);
                for (int i = 0; i < entryTexts.Length; i++)
                {
                    entryTexts[i].text = "";
                }
            }
        });
    }
}
