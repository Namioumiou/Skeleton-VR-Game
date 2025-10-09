using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class SkeletonAI : MonoBehaviour
{
    [Header("Réaction aux coups")]
    public AudioClip hitSound;
    public AudioClip deathSound;
    public float knockbackForce = 3f;
    [HideInInspector] public AudioSource audioSource;

    [HideInInspector] public bool isDead = false;

    [Header("Cible")]
    public Transform target;

    [Header("Déplacement")]
    public float moveSpeed = 2.5f;
    public float rotateSpeed = 10f;
    public float gravity = -9.81f;

    [Header("Combat")]
    public float attackRange = 1.4f;
    public float attackCooldown = 1.0f;
    public int damage = 5;

    [HideInInspector] public Animator animator;

    private CharacterController cc;
    private float cooldown;
    private Vector3 velocity;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        if (!target && Camera.main) target = Camera.main.transform;

        // Snap au sol
        if (Physics.Raycast(transform.position + Vector3.up * 1.0f, Vector3.down, out var hit, 5f))
        {
            Vector3 p = transform.position;
            p.y = hit.point.y;
            transform.position = p;
        }
    }

    void Update()
    {
        if (!target) return;

        MoveAndAttack();
        ApplyGravity();
        cooldown -= Time.deltaTime;
    }

    void MoveAndAttack()
    {
        Vector3 toTarget = target.position - transform.position;
        Vector3 flatDir = new Vector3(toTarget.x, 0f, toTarget.z);
        float flatDistance = flatDir.magnitude;

        // Rotation fluide
        if (flatDir.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(flatDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, rotateSpeed * Time.deltaTime);
        }

        float speed = 0f;

        if (flatDistance > attackRange)
        {
            Vector3 moveH = flatDir.normalized * moveSpeed;
            cc.Move(moveH * Time.deltaTime);
            speed = moveH.magnitude;
        }
        else
        {
            TryAttack();
        }

        if (animator != null)
            animator.SetFloat("Speed", speed);
    }

    void ApplyGravity()
    {
        if (cc.isGrounded)
            velocity.y = -1f;
        else
            velocity.y += gravity * Time.deltaTime;

        cc.Move(velocity * Time.deltaTime);
    }

    void TryAttack()
    {
        if (cooldown > 0f) return;

        var h = target.GetComponent<Health>();
        if (h != null)
            h.TakeDamage(damage);

        cooldown = attackCooldown;
    }

    // Appel depuis l'épée
    public void TakeHit(Vector3 hitDirection, int damage = 5)
    {
        if (isDead) return; // Ignore si déjà mort

        // Retirer des PV via Health
        Health health = GetComponent<Health>();
        if (health != null)
            health.TakeDamage(damage);

        // Vérifier si le squelette est mort avec ce coup
        if (health != null && health.IsDead)
        {
            DieImmediately(); // Jouer le son de mort et supprimer le squelette
            return;           // Ne pas jouer le son de hit
        }

        // Jouer son de hit seulement si pas mort
        if (hitSound && audioSource)
            audioSource.PlayOneShot(hitSound);

        // Knockback
        StartCoroutine(ApplyKnockback(hitDirection));
    }


    private IEnumerator ApplyKnockback(Vector3 hitDirection)
    {
        float knockDuration = 0.2f;
        float elapsed = 0f;
        Vector3 knockDir = -hitDirection.normalized * knockbackForce;
        knockDir.y = 1f; // léger soulèvement

        while (elapsed < knockDuration)
        {
            cc.Move(knockDir * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void DieImmediately()
    {
        if (isDead) return; // éviter double appel
        isDead = true;

        this.enabled = false; // stop IA

        if (deathSound != null && audioSource != null)
        {
            audioSource.Stop(); // stopper tout autre son
            audioSource.PlayOneShot(deathSound);
        }

        // Délai avant destruction pour laisser le son se jouer
        StartCoroutine(DestroyAfterSound(deathSound != null ? deathSound.length : 0f));
    }

    private IEnumerator DestroyAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay); // attendre la fin du son
        Destroy(gameObject);
    }


}
