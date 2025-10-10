using Oculus.Haptics;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input; // For HandController.Handedness
using Oculus.Platform;
using System; // Optional for Oculus APIs
using Unity.VisualScripting;
using UnityEngine;


public class GrabHapticFeedback : MonoBehaviour
{
    private GrabInteractable interactable;
    public HapticClip clip;

    private void Awake()
    {
        interactable = GetComponent<GrabInteractable>();
        if (interactable != null )
        {
            interactable.WhenSelectingInteractorViewAdded += OnSelect;
        }
    }


    private void OnSelect(IInteractorView interactorView)
    {
        if (interactorView is GrabInteractor grabInteractor)
        {
            Debug.Log($"Object selected by GrabInteractor: {grabInteractor.gameObject.name}");
            var hapticPlayer = grabInteractor.gameObject.GetComponentInChildren<HapticSource>();
            if (hapticPlayer != null)
            {
                hapticPlayer.clip = clip;
                hapticPlayer.Play();
            }
        }
        else if (interactorView is HandGrabInteractor handGrabInteractor)
        {
            Debug.Log($"Object selected by HandGrabInteractor: {handGrabInteractor.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("Unknown interactor type.");
        }
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.WhenSelectingInteractorViewAdded -= OnSelect;
        }
    }
}
