using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundOptions : MonoBehaviour
{
    public GameObject optionsUIObject;
    public GameObject optionsButton;
    [Header("Sound Options")]
    [SerializeField] AudioMixer mainMixer;
    [SerializeField] SliderOption[] sliders;

    private void Start()
    {
        optionsUIObject.SetActive(false);
        optionsButton.SetActive(false);
        for (int i = 0; i < sliders.Length; i++)
        {
            if (!PlayerPrefs.HasKey(sliders[i].paramName))
            {
                PlayerPrefs.SetFloat(sliders[i].paramName, sliders[i].slider.value);
            }
            GetAudioMixerValue(i);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    [ContextMenu("Toggle Menu")]
    public void ToggleMenu()
    {
        optionsUIObject.SetActive(!optionsUIObject.activeInHierarchy);
        optionsButton.SetActive(!optionsUIObject.activeInHierarchy);
        Time.timeScale = optionsUIObject.activeInHierarchy ? 0.0f : 1.0f;
        if (optionsUIObject.activeInHierarchy)
        {
            GameManager.Instance.SelectPanel(0);
        }
    }

    void GetAudioMixerValue(int sliderID)
    {
        float storedValue = PlayerPrefs.GetFloat(sliders[sliderID].paramName);
        sliders[sliderID].slider.value = storedValue;
        mainMixer.SetFloat(sliders[sliderID].paramName, Mathf.Log10(sliders[sliderID].slider.value) * 20.0f);
    }

    public void SetAudioMixerValue(int sliderID)
    {
        mainMixer.SetFloat(sliders[sliderID].paramName, Mathf.Log10(sliders[sliderID].slider.value) * 20.0f);
        PlayerPrefs.SetFloat(sliders[sliderID].paramName, sliders[sliderID].slider.value);
        //Debug.Log("Setting Slider to: " + sliders[sliderID].slider.value);
    }
}

[System.Serializable]
public struct SliderOption
{
    public Slider slider;
    public string paramName;
}