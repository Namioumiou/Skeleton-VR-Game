using Oculus.Interaction;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class GravityOnRelease : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasBeenGrabbedOnce = false; // pour savoir si l’objet a déjà été attrapé une fois

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Gravité désactivée au démarrage
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    public void OnGrab()
    {
        rb.isKinematic = false;
    }

    public void OnRelease()
    {
        
        Debug.Log("objet release");
        // Si c’est la première fois qu’on relâche l’objet → on active la gravité
        if (!hasBeenGrabbedOnce)
        {
            rb.useGravity = true;
            hasBeenGrabbedOnce = true;
            
        }
    }
}
