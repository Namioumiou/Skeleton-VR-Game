using UnityEngine;

public class SwordSound : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip whooshClip;

    [Header("Motion Settings")]
    public float speedThreshold = 1.2f;   // Vitesse minimale pour déclencher le son
    public float cooldownTime = 0.5f;     // Temps minimum entre deux sons
    public float maxSpeed = 5f;           // Pour normaliser pitch/volume

    private Vector3 lastLocalPos;
    private float nextAllowedTime = 0f;

    void Start()
    {
        lastLocalPos = transform.localPosition;
    }

    void Update()
    {
        float speed = (transform.localPosition - lastLocalPos).magnitude / Time.deltaTime;
        lastLocalPos = transform.localPosition;

        // Si on est assez rapide ET qu'on a dépassé le cooldown
        if (speed > speedThreshold && Time.time >= nextAllowedTime)
        {
            float normalizedSpeed = Mathf.Clamp01(speed / maxSpeed);

            // Ajuste le son selon la vitesse
            audioSource.pitch = Mathf.Lerp(1f, 1.5f, normalizedSpeed);
            audioSource.volume = Mathf.Lerp(0.4f, 1f, normalizedSpeed);

            // Joue le son une fois, sans l’interrompre
            audioSource.PlayOneShot(whooshClip);

            // Délai avant le prochain son possible
            nextAllowedTime = Time.time + cooldownTime;
        }
    }
}
