using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class Spinner : MonoBehaviour
{
    public GameObject visuals;
    public bool inUse = false;
    
    internal int currentValue;
    [SerializeField] Transform spinnerTitleTransform;
    [SerializeField] Transform pullArrowTransform;
    [SerializeField] Rigidbody2D spinningBody;
    [SerializeField] SpriteRenderer arrowTipImage;
    [SerializeField] TMP_Text currentScoreText;
    [SerializeField] Slider pullLeverSlider;
    [SerializeField] AudioSource aSource;
    [SerializeField] AudioClip releaseClip;
    [SerializeField] AudioClip[] visibilitySounds;
    [SerializeField] float minVelocity = .1f;

    [SerializeField] Color[] valueColors;

    Player myPlayer;
    Vector3 startScale;
    bool spinning = false;
    int prevValue;
    // Start is called before the first frame update
    void Start()
    {
        myPlayer = GetComponentInParent<Player>();
        startScale = currentScoreText.transform.localScale;
        pullArrowTransform.DOLocalMoveY(55, 1.0f).SetEase(Ease.InQuint).SetLoops(-1, LoopType.Yoyo);
        spinnerTitleTransform.DOScale(Vector3.one * .45f, 2.0f).SetLoops(-1, LoopType.Yoyo);

        visuals.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    ShowSpinner();
        //}

        //if (Input.GetKeyDown(KeyCode.LeftControl))
        //{
        //    HideSpinner();
        //}

        if (spinning)
        {
            UpdateCurrentValue();
            if (Mathf.Abs(spinningBody.angularVelocity) < minVelocity)
            {
                SpinFinished();
            }
        }
    }

    void UpdateCurrentValue()
    {
        currentValue = (int)Mathf.Clamp(Mathf.Lerp(7, 1, (spinningBody.transform.eulerAngles.z % 360) / 360.0f), 1, 6);

        if(prevValue != currentValue)   //Value Changed Here
        {
            aSource.Play();
            //Tween Here
            if (!DOTween.IsTweening(currentScoreText.transform))
            {
                currentScoreText.transform.localScale = startScale;
                currentScoreText.transform.DOPunchScale(Vector3.one * .15f, .05f);
            }
            arrowTipImage.color = valueColors[currentValue - 1];
            currentScoreText.color = valueColors[currentValue - 1];
            currentScoreText.text = currentValue.ToString();
        }

        prevValue = currentValue;
    }

    public void ShowSpinner()
    {
        inUse = true;
        visuals.SetActive(true);
        aSource.PlayOneShot(visibilitySounds[0]);
        visuals.transform.localScale = Vector3.zero;
        visuals.transform.DOScale(Vector3.one, .35f).SetEase(Ease.OutQuint);
        AllowSpin();
    }

    public void HideSpinner()
    {
        inUse = false;
        aSource.PlayOneShot(visibilitySounds[1]);
        visuals.transform.localScale = Vector3.one;
        visuals.transform.DOScale(Vector3.zero, .35f).SetEase(Ease.InQuint);
    }

    public void Spin()
    {
        if (!pullLeverSlider.interactable) return;
        aSource.PlayOneShot(releaseClip);
        pullLeverSlider.interactable = false;
        spinning = true;
        DOVirtual.Float(pullLeverSlider.value, 0.0f, .25f, t => { pullLeverSlider.value = t; }).SetEase(Ease.OutQuart);
        //pullLeverSlider.value.Do
        
        float pullForce = Mathf.Lerp(5, 50, pullLeverSlider.value) * Random.Range(.8f, 1.1f);
        spinningBody.constraints = RigidbodyConstraints2D.FreezePosition;
        spinningBody.AddTorque(-pullForce, ForceMode2D.Impulse); 
    }

    void SpinFinished()
    {
        inUse = false;
        spinning = false;
        spinningBody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void AllowSpin()
    {
        pullLeverSlider.interactable = true;
        spinningBody.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    [Header("Debug")]
    public int spacesToMove = 5;
    [ContextMenu("OverrideSpin")]
    public void OverrideSpin()
    {
        currentValue = spacesToMove;
        SpinFinished();
    }

}