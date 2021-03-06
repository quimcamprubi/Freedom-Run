using UnityEngine;
using UnityEngine.UI;

public class VolumeCode : MonoBehaviour
{
    public Slider slider;
    public float sliderValue;
    public Image imagenMute;

    // Start is called before the first frame update
    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat("volumeAudio", 1.0f);
        AudioListener.volume = slider.value;
        CheckIfMuted();
    }

    public void ChangeSlider(float value)
    {
        sliderValue = value;
        PlayerPrefs.SetFloat("volumeAudio", sliderValue);
        AudioListener.volume = slider.value;
        CheckIfMuted();
    }

    public void CheckIfMuted()
    {
        if (sliderValue == 0)
            imagenMute.enabled = true;
        else
            imagenMute.enabled = false;
    }
}