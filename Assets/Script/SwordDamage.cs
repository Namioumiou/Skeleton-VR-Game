// SwordDamage.cs  (détection via trigger sur la lame)
using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public int damage = 25;
    public bool useTrigger = true;
    public string targetTag = ""; // "Enemy" si tu veux filtrer

    void OnTriggerEnter(Collider other)
    {
        if (!useTrigger) return;
        TryHit(other);
    }

    void TryHit(Collider c)
    {
        if (!string.IsNullOrEmpty(targetTag) && !c.CompareTag(targetTag)) return;
        var h = c.GetComponentInParent<Health>();
        if (h) h.TakeDamage(damage);
    }
}
