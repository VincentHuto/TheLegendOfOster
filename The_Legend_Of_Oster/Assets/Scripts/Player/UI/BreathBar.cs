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

    public void SetMaxBreath(int maxBreath)
    {
        slider.maxValue = maxBreath;
        slider.value = maxBreath;
    }

    public void SetCurrentBreath(int currentBreath)
    {
        slider.value = currentBreath;
    }
}