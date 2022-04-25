using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGame : MonoBehaviour
{
    public delegate void OnGameCompleted(MiniGame game);
    public event OnGameCompleted onGameCompleted;

    void Start()
    {
        StartCoroutine(FreeWin());
    }

    IEnumerator FreeWin()
    {
        yield return new WaitForSeconds(2.0f);
        GameCompleted();
    }

    void Update()
    {
        
    }

    public virtual void GameCompleted()
    {
        onGameCompleted?.Invoke(this);
    }
}
