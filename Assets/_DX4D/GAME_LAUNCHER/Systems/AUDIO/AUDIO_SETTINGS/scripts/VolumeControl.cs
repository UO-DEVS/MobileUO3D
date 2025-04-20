//BY DX4D
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] Jukebox _jukebox;
    Jukebox jukebox => _jukebox;
    [SerializeField] Slider _slider;
    Slider slider => _slider;
    [SerializeField] AudioChannel _channel = AudioChannel.Music;
    [SerializeField] string _parameterName = "Volume";
    string parameterName => (_channel.ToString() + _parameterName);

    //[SerializeField] bool _reset = false;
    //private void ResetPlayerPrefs()
    //{
    //    if (_reset)
    //    {
    //        RecordSliderValue(jukebox.GetVolume(_channel));
    //        _reset = false;
    //    }
    //}
    //ON VALIDATE
    private void OnValidate()
    {
        if (!_jukebox) _jukebox = FindObjectOfType<Jukebox>(); //FIND JUKEBOX
        if (!_slider) _slider = gameObject.GetComponentInChildren<Slider>();

        //ResetPlayerPrefs();
        RefreshSliderValues();
    }
    //AWAKE
    private void Awake()
    {
        if (!_jukebox) _jukebox = FindObjectOfType<Jukebox>(); //FIND JUKEBOX
        if (slider) slider.onValueChanged.AddListener(OnSliderValueChanged);

        LoadSliderValues();
        RefreshSliderValues();
    }
    //START
    void Start()
    {
    }
    //ON DISABLE
    private void OnDisable()
    {
        RecordSliderValue(slider.value); //RECORD VOLUME TO PLAYER PREFS
    }
    //ON SLIDER VALUE CHANGED
    internal void OnSliderValueChanged(float _value)
    {
        Debug.Log("SETTING " + _channel.ToString().ToUpper() + " VOLUME TO " + _value);
        //_slider.value = _value;
        jukebox.SetVolume(_channel, (int)_value);
        RecordSliderValue(_value); //RECORD VOLUME TO PLAYER PREFS
    }
    //REFRESH FROM JUKEBOX
    void RefreshSliderValues()
    {
        if (jukebox && slider)
        {
            slider.value = jukebox.GetVolume(_channel);
            //_slider.value = PlayerPrefs.GetFloat(parameterName, _slider.value);
        }
    }
    //RECORD TO PLAYER PREFS
    void RecordSliderValue(float _newValue)
    {
        PlayerPrefs.SetFloat(parameterName, _newValue);
    }
    //LOAD FROM PLAYER PREFS
    void LoadSliderValues()
    {
        if (jukebox)
        {
            jukebox.SetVolume(_channel, (int)PlayerPrefs.GetFloat(parameterName, jukebox.GetVolume(_channel)));
            //jukebox.RefreshAudio();
        }
    }
}
