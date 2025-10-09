using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SkeletonAI : MonoBehaviour
{
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

    CharacterController cc;
    Animator animator;
    float cooldown;
    Vector3 velocity; // <-- gravité ici

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (!target && Camera.main) target = Camera.main.transform;

        // (Optionnel) Snap au sol au spawn pour éviter de "flotter" si instancié trop haut
        if (Physics.Raycast(transform.position + Vector3.up * 1.0f, Vector3.down, out var hit, 5f))
        {
            var p = transform.position;
            p.y = hit.point.y;
            transform.position = p;
        }
    }

    void Update()
    {
        if (!target) return;

        // Direction horizontale vers la cible
        Vector3 toTarget = target.position - transform.position;
        Vector3 flatDir = new Vector3(toTarget.x, 0f, toTarget.z);
        float flatDistance = flatDir.magnitude; // <-- distance horizontale

        // Rotation fluide
        if (flatDir.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(flatDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, rotateSpeed * Time.deltaTime);
        }

        // Déplacement horizontal
        float speed = 0f;
        if (flatDistance > attackRange)
        {
            Vector3 moveH = flatDir.normalized * moveSpeed;
            cc.Move(moveH * Time.deltaTime);
            speed = moveH.magnitude; // pour l'anim
        }
        else
        {
            TryAttack();
        }

        // Gravité (toujours appliquée)
        if (cc.isGrounded)
            velocity.y = -1f;            // colle au sol
        else
            velocity.y += gravity * Time.deltaTime;

        cc.Move(velocity * Time.deltaTime);

        // Paramètre d'anim
        if (animator) animator.SetFloat("Speed", speed);

        // cooldown attaque
        cooldown -= Time.deltaTime;
    }

    void TryAttack()
    {
        if (cooldown > 0f) return;

        var h = target.GetComponent<Health>();
        if (h) h.TakeDamage(damage);

        cooldown = attackCooldown;
    }
}
