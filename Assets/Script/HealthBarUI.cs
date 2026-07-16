using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    public PlayerHealth health;
    public Slider slider;
    public TMP_Text valueText;

    void Awake()
    {
        if (!slider) slider = GetComponentInChildren<Slider>();
    }

    void OnEnable()
    {
        if (health)
        {
            slider.minValue = 0;
            slider.maxValue = health.maxHealth;
            slider.value = health.Current;
            health.onHealthChanged.AddListener(OnHealthChanged);
        }
    }

    void OnDisable()
    {
        if (health) health.onHealthChanged.RemoveListener(OnHealthChanged);
    }

    void OnHealthChanged(int current, int max)
    {
        slider.maxValue = max;
        slider.value = current;
        if (valueText) valueText.text = $"{current}/{max}";
    }
}
