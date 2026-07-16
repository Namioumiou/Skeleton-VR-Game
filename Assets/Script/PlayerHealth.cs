using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable] public class HealthChangedEvent : UnityEvent<int, int> {}

public class PlayerHealth : MonoBehaviour
{
    [Header("Vie")]
    public int maxHealth = 100;
    public int Current { get; private set; }
    public bool IsDead { get; private set; }

    [Header("Événements")]
    public HealthChangedEvent onHealthChanged;
    public UnityEvent onDeath;

    [Header("UI Game Over")]
    public GameObject gameOverCanvas;

    [Header("À désactiver à la mort")]
    [Tooltip("Glisse ici les scripts de locomotion/téléportation à couper (PlayerController, SmoothControls, TeleportControllerInteractor, etc.).")]
    public Behaviour[] disableOnDeath;   // <— IMPORTANT

    void Awake()
    {
        Current = maxHealth;
        IsDead = false;
        onHealthChanged?.Invoke(Current, maxHealth);
        if (gameOverCanvas) gameOverCanvas.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || amount <= 0) return;
        Current = Mathf.Max(0, Current - amount);
        onHealthChanged?.Invoke(Current, maxHealth);
        if (Current == 0) Die();
    }

    public void Heal(int amount)
    {
        if (IsDead || amount <= 0) return;
        Current = Mathf.Min(maxHealth, Current + amount);
        onHealthChanged?.Invoke(Current, maxHealth);
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        // 1) Couper la loco + TP
        if (disableOnDeath != null)
            foreach (var b in disableOnDeath)
                if (b) b.enabled = false;

        // 2) Pause jeu + UI
        onDeath?.Invoke();
        Time.timeScale = 0f;
        if (gameOverCanvas) gameOverCanvas.SetActive(true);
    }

    // appelé par le bouton "Rejouer"
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
