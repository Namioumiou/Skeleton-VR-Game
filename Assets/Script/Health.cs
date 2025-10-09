using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 50;

    [HideInInspector] public int currentHealth;
    [HideInInspector] public bool IsDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"[{name}] HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        IsDead = true;

        SkeletonAI ai = GetComponent<SkeletonAI>();
        if (ai != null)
            ai.DieImmediately();
        else
            Destroy(gameObject);
    }
}
