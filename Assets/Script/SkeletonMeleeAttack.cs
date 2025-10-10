using UnityEngine;

public class SkeletonMeleeAttack : MonoBehaviour
{
    [Header("Dégâts infligés")]
    public int damage = 10;

    [Header("Cooldown entre 2 coups")]
    public float attackCooldown = 1.0f;

    private float lastAttackTime = -999f;

    private void OnTriggerEnter(Collider other)
    {
        TryDealDamage(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDealDamage(other);
    }

    private void TryDealDamage(Collider other)
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return; // encore en cooldown

        // Vérifie si c’est le joueur
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null && !player.IsDead)
        {
            player.TakeDamage(damage);
            lastAttackTime = Time.time;
            Debug.Log($"[MeleeAttack] {name} a frappé {other.name} pour {damage} dégâts !");
        }
    }
}
