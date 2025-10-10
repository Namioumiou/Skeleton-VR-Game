using UnityEngine;

public class SwordSound : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip whooshClip;

    [Header("Motion Settings")]
    public float speedThreshold = 3f;   // vitesse minimale pour déclencher le son
    public float cooldownTime = 0.5f;     // temps minimum entre deux sons
    public float maxSpeed = 5f;

    [Header("Tip de l'épée")]
    public Transform slashTip;
    public Transform swordRoot;

    private Vector3 lastTipPos;
    private float nextAllowedTime = 0f;

    void Start()
    {
        if (slashTip == null || swordRoot == null)
        {
            Debug.LogError("SlashTip ou SwordRoot non assigné !");
            enabled = false;
            return;
        }

        lastTipPos = slashTip.position - swordRoot.position;
    }

    void Update()
    {
        // vitesse relative de la pointe par rapport à l'épée
        Vector3 relativePos = slashTip.position - swordRoot.position;
        float speed = (relativePos - lastTipPos).magnitude / Time.deltaTime;
        lastTipPos = relativePos;

        // déclenchement du son si vitesse suffisante et cooldown écoulé
        if (speed > speedThreshold && Time.time >= nextAllowedTime)
        {
            float normalizedSpeed = Mathf.Clamp01(speed / maxSpeed);

            audioSource.pitch = Mathf.Lerp(1f, 1.5f, normalizedSpeed);
            audioSource.volume = Mathf.Lerp(0.4f, 1f, normalizedSpeed);

            audioSource.PlayOneShot(whooshClip);

            // fixe le temps minimum avant le prochain son
            nextAllowedTime = Time.time + cooldownTime;
        }
    }
}
