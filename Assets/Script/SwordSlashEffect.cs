using UnityEngine;

public class SwordSlashEffect : MonoBehaviour
{
    [Header("Références")]
    public Transform slashTip;           // SlashOrigin (pointe)

    [Header("Activation du slash")]
    public float activationSpeed = 100f;   // vitesse minimale pour activer le trail
    public float maxSpeed = 10f;          // vitesse maximale pour largeur et longueur
    public float fadeDuration = 100f;    // durée pendant laquelle le trail reste visible après ralentissement

    [Header("Taille du Trail")]
    public float minWidth = 0.05f;
    public float maxWidth = 10f;
    public float minTrailTime = 0.1f;    // durée minimale du trail
    public float maxTrailTime = 0.5f;    // durée maximale du trail

    private TrailRenderer trail;
    private Vector3 lastTipPos;
    private float fadeTimer = 0f;

    void Start()
    {
        if (slashTip == null)
        {
            Debug.LogError("Slash Tip non assigné !");
            enabled = false;
            return;
        }

        trail = slashTip.GetComponent<TrailRenderer>();
        if (trail == null)
        {
            Debug.LogError("TrailRenderer manquant sur le Slash Tip !");
            enabled = false;
            return;
        }

        lastTipPos = slashTip.position;
        trail.emitting = false; // désactivé au départ
    }

    void Update()
    {
        // Calcul de la vitesse du tip
        float speed = (slashTip.position - lastTipPos).magnitude / Time.deltaTime;
        lastTipPos = slashTip.position;

        // Normalisation de la vitesse
        float normalized = Mathf.Clamp01(speed / maxSpeed);

        // Ajuste la largeur du trail selon la vitesse
        trail.startWidth = Mathf.Lerp(minWidth, maxWidth, normalized);

        // Ajuste la durée du trail pour allonger les slashs rapides
        trail.time = Mathf.Lerp(minTrailTime, maxTrailTime, normalized);

        // Activation du trail avec fade
        if (speed > activationSpeed)
        {
            trail.emitting = true;
            fadeTimer = fadeDuration; // reset du timer
        }
        else
        {
            if (fadeTimer > 0f)
            {
                fadeTimer -= Time.deltaTime;
                trail.emitting = true; // garde actif pendant le fade
            }
            else
            {
                trail.emitting = false; // désactive quand le fade est terminé
            }
        }
    }
}
