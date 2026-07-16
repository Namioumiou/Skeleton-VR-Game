using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class SkeletonAI : MonoBehaviour
{
    [Header("Cible")]
    public Transform target; // assigne PlayerController (Transform) si possible

    [Header("Déplacement")]
    public float moveSpeed = 2.5f;
    public float rotateSpeed = 10f;
    public float gravity = -9.81f;

    [Header("Sol")]
    public LayerMask groundLayers = ~0;
    public float snapToGroundDistance = 5f;

    [Header("Combat")]
    public float attackRange = 1.4f;            // portée sphérique
    public float verticalTolerance = 1.0f;      // diff de hauteur max pour frapper
    public bool requireLineOfSight = true;      // évite de frapper à travers murs/sol
    public LayerMask losObstacles = ~0;         // couches bloquant le ray
    public float attackCooldown = 1.0f;
    public int damage = 5;

    [Header("Feedback")]
    public AudioClip hitSound;
    public AudioClip deathSound;
    public float knockbackForce = 5f;

    [Header("Debug")]
    public bool verbose = false;

    // --- état interne ---
    CharacterController cc;
    Animator animator;
    AudioSource audioSource;

    bool isKnockingBack = false;
    bool isDead = false;
    float cooldown = 0f;
    Vector3 velocity;
    float animSpeed = 0f;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        if (!target && Camera.main) target = Camera.main.transform;

        SnapToGroundOnce();
    }

    void Update()
    {
        if (!target || isDead)
        {
            SetAnimSpeed(0f);
            return;
        }

        // petites corrections au sol si on flotte/glisse
        KeepStuckToGround();

        if (!isKnockingBack)
        {
            MoveTowardsTargetOrAttack();
        }

        if (verbose && InAttackRangeNow())
            Debug.Log($"[{name}] In range! cooldown={cooldown:0.00}");


        ApplyGravity();

        if (cooldown > 0f) cooldown -= Time.deltaTime;
    }

    void OnDestroy()
    {
        if (SkeletonManager.Instance != null)
            SkeletonManager.Instance.UnregisterSkeleton(gameObject);
    }

    // ---------- Déplacement + attaque ----------
    void MoveTowardsTargetOrAttack()
    {
        Vector3 to = target.position - transform.position;

        // rotation horizontale fluide
        Vector3 flat = new Vector3(to.x, 0f, to.z);
        if (flat.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(flat);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, rotateSpeed * Time.deltaTime);
        }

        float dist = to.magnitude;

        if (dist > attackRange || !InAttackRangeNow())
        {
            // avancer
            Vector3 moveH = flat.normalized * moveSpeed;
            cc.Move(moveH * Time.deltaTime);
            SetAnimSpeed(moveH.magnitude);
        }
        else
        {
            // frapper
            TryAttack();
            SetAnimSpeed(0f);
        }
    }

    // ---------- Conditions de frappe robustes ----------
    bool InAttackRangeNow()
    {
        if (!target) return false;

        // tolérance verticale
        float dy = Mathf.Abs(transform.position.y - target.position.y);
        if (dy > verticalTolerance) return false;

        // distance horizontale
        Vector3 a = transform.position;
        Vector3 b = target.position;
        a.y = 0f; b.y = 0f;
        float flatDist = Vector3.Distance(a, b);
        if (flatDist > attackRange) return false;

        // (Optionnel) ligne de vue
        if (requireLineOfSight)
        {
            Vector3 o = transform.position + Vector3.up * 0.9f;
            Vector3 t = target.position    + Vector3.up * 0.9f;

            // losObstacles = layers "sol/décors" (PAS Enemy/Player)
            if (Physics.Linecast(o, t, out var hit, losObstacles, QueryTriggerInteraction.Ignore))
            {
                // si on tape autre chose que la cible → bloqué
                if (hit.transform != target && hit.transform.root != target)
                    return false;
            }
        }

        return true;
    }



    void TryAttack()
    {
        if (cooldown > 0f) return;
        if (!InAttackRangeNow()) return;

        bool didHit = false;

        var ph = target.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            Debug.Log($"[{name}] ATTACK PlayerHealth for {damage}");
            ph.TakeDamage(damage);
            didHit = true;
        }
        else
        {
            var h = target.GetComponent<Health>();
            if (h != null)
            {
                Debug.Log($"[{name}] ATTACK Health for {damage}");
                h.TakeDamage(damage);
                didHit = true;
            }
        }

        if (didHit)
            cooldown = attackCooldown;
    }


    // ---------- Gravité / Sol ----------
    void ApplyGravity()
    {
        if (cc.isGrounded) velocity.y = -1f;
        else               velocity.y += gravity * Time.deltaTime;

        cc.Move(velocity * Time.deltaTime);
    }

    void SnapToGroundOnce()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out var hit, snapToGroundDistance, groundLayers, QueryTriggerInteraction.Ignore))
        {
            var p = transform.position;
            p.y = hit.point.y;
            transform.position = p;
        }
    }

    void KeepStuckToGround()
    {
        // si on n’est pas grounded, essaye de “recoller” doucement par raycast
        if (!cc.isGrounded)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, out var hit, 0.5f, groundLayers, QueryTriggerInteraction.Ignore))
            {
                var p = transform.position;
                p.y = hit.point.y;
                transform.position = p;
                velocity.y = -1f;
            }
        }
    }

    // ---------- Réception d’un coup ----------
    public void TakeHit(Vector3 hitDirection, int dmg = 5)
    {
        if (isDead) return;

        var health = GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(dmg);
            if (health.IsDead) { DieImmediately(); return; }
        }

        StartCoroutine(Knockback(hitDirection));
    }

    IEnumerator Knockback(Vector3 dir)
    {
        isKnockingBack = true;
        if (animator) animator.SetTrigger("Knockback");

        float t = 0f, dur = 0.2f;
        Vector3 k = dir.normalized * knockbackForce; k.y = 1f;

        while (t < dur)
        {
            cc.Move(k * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }

        isKnockingBack = false;
    }

    // ---------- Mort ----------
    public void DieImmediately()
    {
        if (isDead) return;
        isDead = true;

        if (SkeletonManager.Instance != null)
            SkeletonManager.Instance.UnregisterSkeleton(gameObject);

        moveSpeed = 0f;
        cooldown = Mathf.Infinity;
        if (cc) cc.enabled = false;

        if (animator) animator.SetTrigger("Die");
        if (deathSound) audioSource.PlayOneShot(deathSound);

        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    // ---------- util ----------
    void SetAnimSpeed(float s)
    {
        animSpeed = s;
        if (animator) animator.SetFloat("Speed", animSpeed);
    }
}
