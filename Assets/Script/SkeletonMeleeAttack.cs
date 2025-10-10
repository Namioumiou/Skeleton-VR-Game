using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SkeletonMeleeAttack : MonoBehaviour
{
    [Header("Dégâts infligés")]
    public int damage = 10;

    [Header("Cooldown entre attaques")]
    public float attackCooldown = 1.5f;

    [Header("Références")]
    public Animator animator; // 👈 à lier dans l'inspector (le skeleton)
    public string slashTriggerName = "Slash"; // nom du trigger dans l’Animator

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
            return; // cooldown

        var player = other.GetComponent<PlayerHealth>();
        if (player != null && !player.IsDead)
        {
            player.TakeDamage(damage);
            lastAttackTime = Time.time;

            // 👇 Lancer l’animation
            if (animator)
                animator.SetTrigger(slashTriggerName);

            Debug.Log($"[{name}] Slash hit {other.name} for {damage} dmg!");
        }
    }
}
