using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int maxHealth = 50;
    public UnityEvent onDeath;

    int current;

    void Awake() => current = maxHealth;

    public void TakeDamage(int amount)
    {
        current = Mathf.Max(0, current - amount);
        Debug.Log($"[{name}] HP: {current}/{maxHealth}");

        if (current <= 0) Die();
    }

    void Die()
    {
        onDeath?.Invoke();
        Destroy(gameObject); // ou Destroy(transform.root.gameObject);
    }
}
