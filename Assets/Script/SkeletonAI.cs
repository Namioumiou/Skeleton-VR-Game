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

    private CharacterController cc;
    private Animator animator;
    private float verticalVelocity;
    private float cooldown;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>(); // ✅ Récupère l’Animator
    }

    void Start()
    {
        if (target == null && Camera.main != null)
            target = Camera.main.transform;
    }

    void Update()
    {
        if (!target) return;

        // Direction horizontale vers la cible
        Vector3 direction = target.position - transform.position;
        Vector3 flatDir = new Vector3(direction.x, 0, direction.z);
        float distance = direction.magnitude;

        // Rotation fluide vers la cible
        if (flatDir.sqrMagnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(flatDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        }

        // Mouvement
        Vector3 move = Vector3.zero;
        float speed = 0f;
        if (distance > attackRange)
        {
            move = flatDir.normalized * moveSpeed;
            speed = move.magnitude; // ✅ Pour l’animation
        }

        // Gravité
        if (cc.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -1f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        move.y = verticalVelocity;

        // Déplacement
        cc.Move(move * Time.deltaTime);

        // ✅ Met à jour la vitesse dans l’Animator
        if (animator)
            animator.SetFloat("Speed", speed);

        // Attaque
        if (distance <= attackRange)
        {
            if (cooldown <= 0f)
            {
                var h = target.GetComponent<Health>();
                if (h != null)
                    h.TakeDamage(damage);
                cooldown = attackCooldown;
            }
        }

        cooldown -= Time.deltaTime;
    }
}
