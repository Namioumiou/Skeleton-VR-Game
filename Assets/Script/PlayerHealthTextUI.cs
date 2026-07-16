using TMPro;
using UnityEngine;

public class PlayerHealthTextUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public TMP_Text text;

    void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.onHealthChanged.AddListener(OnHealthChanged);

        RefreshNow();
    }

    void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.onHealthChanged.RemoveListener(OnHealthChanged);
    }

    void OnHealthChanged(int current, int max)
    {
        if (text) text.text = $"{current}/{max}";
        Debug.Log($"[UI] HP -> {current}/{max}");
    }

    public void RefreshNow()
    {
        if (playerHealth != null && text != null)
            text.text = $"{playerHealth.Current}/{playerHealth.maxHealth}";
    }
}
