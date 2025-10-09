using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class SkeletonAI : MonoBehaviour
{
    [Header("Réaction aux coups")]
    public AudioClip hitSound;
    public AudioClip deathSound;
    public float knockbackForce = 5f;
    [HideInInspector] public AudioSource audioSource;

    private bool isKnockingBack = false;
    private float speed = 0f;

    [HideInInspector] public bool isDead = false;

    [Header("Mort")] public float timeBeforeDestroy = 1.5f; // temps avant destruction, en secondes

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

        // Si le squelette est mort, bloquer tout Update de mouvement
        if (isDead)
        {
            speed = 0f;
            return;
        }

        MoveAndAttack();
        ApplyGravity();
        cooldown -= Time.deltaTime;

        // Met à jour Speed seulement si pas knockback
        if (animator && !isKnockingBack)
        {
            animator.SetFloat("Speed", speed);
        }
    }

    void MoveAndAttack()
    {
        if (isKnockingBack || isDead) return;

        Vector3 toTarget = target.position - transform.position;
        Vector3 flatDir = new Vector3(toTarget.x, 0f, toTarget.z);
        float flatDistance = flatDir.magnitude;

        // Rotation fluide
        if (flatDir.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(flatDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, rotateSpeed * Time.deltaTime);
        }

        speed = 0f;

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

    public void TakeHit(Vector3 hitDirection, int damage = 5)
    {
        if (isDead) return;

        Health health = GetComponent<Health>();
        if (health != null)
            health.TakeDamage(damage);

        if (health != null && health.IsDead)
        {
            DieImmediately();
            return;
        }

        if (hitSound && audioSource)
            audioSource.PlayOneShot(hitSound);

        StartCoroutine(ApplyKnockback(hitDirection));
    }

    private IEnumerator ApplyKnockback(Vector3 hitDirection)
    {
        isKnockingBack = true;

        if (animator) animator.SetTrigger("Knockback"); // restera sur Idle

        float knockDuration = 0.2f;
        float elapsed = 0f;
        Vector3 knockDir = hitDirection.normalized * knockbackForce;
        knockDir.y = 1f;

        while (elapsed < knockDuration)
        {
            cc.Move(knockDir * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isKnockingBack = false;
    }

    public void DieImmediately()
    {
        if (isDead) return;
        isDead = true;

        // Bloquer l'IA et le mouvement
        moveSpeed = 0f;
        attackRange = 0f;
        cooldown = Mathf.Infinity;

        // Désactiver le CharacterController pour éviter tout déplacement
        if (cc != null) cc.enabled = false;

        // Déclencher l'animation de mort
        if (animator != null)
            animator.SetTrigger("Die");

        // Jouer le son de mort
        if (deathSound && audioSource)
            audioSource.PlayOneShot(deathSound);

        // Détruire le squelette après timeBeforeDestroy secondes
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(timeBeforeDestroy);
        Destroy(gameObject);
    }


}
