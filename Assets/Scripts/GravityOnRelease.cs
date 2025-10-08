using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityOnRelease : MonoBehaviour
{
    private Rigidbody rb;
    private bool isHeld = true;
    private bool hasBeenReleased = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Pas de gravité au départ
        rb.linearVelocity = Vector3.zero; // S'assure qu'il ne bouge pas au début
        rb.angularVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (!isHeld && !hasBeenReleased)
        {
            rb.useGravity = true;   // Active la gravité à la libération
            hasBeenReleased = true;
        }
    }

    // Appelle cette fonction depuis ton système VR quand l'objet est lâché
    public void Release()
    {
        isHeld = false;
    }

    // Optionnel : si tu veux pouvoir le reprendre
    public void Grab()
    {
        isHeld = true;
        rb.useGravity = false;      // Désactive la gravité quand on reprend
        hasBeenReleased = false;
        rb.linearVelocity = Vector3.zero; // Stoppe tout mouvement résiduel
        rb.angularVelocity = Vector3.zero;
    }
}
