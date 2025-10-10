using UnityEngine;
using System.Collections.Generic;

public class SkeletonManager : MonoBehaviour
{
    public static SkeletonManager Instance; // Singleton
    public int maxSkeletons = 5; // Nombre max
    private List<GameObject> skeletons = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Optionnel: ne pas détruire entre scènes
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Supprime les entrées nulles (objet détruit)
    private void CleanList()
    {
        skeletons.RemoveAll(s => s == null);
    }

    public void RegisterSkeleton(GameObject skeleton)
    {
        if (skeleton == null) return;
        CleanList();
        if (!skeletons.Contains(skeleton))
        {
            skeletons.Add(skeleton);
            Debug.Log($"[SkeletonManager] Registered. Count = {skeletons.Count}/{maxSkeletons}");
        }
    }

    public void UnregisterSkeleton(GameObject skeleton)
    {
        // Retire la référence (si null on nettoie la liste)
        if (skeleton == null)
        {
            CleanList();
            Debug.Log($"[SkeletonManager] Unregistered null. Count = {skeletons.Count}/{maxSkeletons}");
            return;
        }

        if (skeletons.Remove(skeleton))
        {
            Debug.Log($"[SkeletonManager] Unregistered. Count = {skeletons.Count}/{maxSkeletons}");
        }
    }

    public bool CanSpawn()
    {
        CleanList();
        return skeletons.Count < maxSkeletons;
    }

    public int GetSkeletonCount()
    {
        CleanList();
        return skeletons.Count;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
