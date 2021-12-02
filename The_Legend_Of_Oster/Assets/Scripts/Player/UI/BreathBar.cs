using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BreathBar : MonoBehaviour
{
    public Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void SetMaxBreath(float maxBreath)
    {
        slider.maxValue = maxBreath;
        slider.value = maxBreath;
    }

    public void SetCurrentBreath(float currentBreath)
    {
        slider.value = currentBreath;
    }
}