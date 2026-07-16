using UnityEngine;

public class SkeletonSpawner : MonoBehaviour
{
    [Header("Réglages du spawn")]
    public GameObject skeletonPrefab; // Le prefab du squelette
    public Transform spawnPoint;      // L’endroit où il apparaît (l’arbre)
    public float spawnInterval = 10f; // Temps entre chaque spawn (en secondes)
    public int maxSkeletons = 2;      // ✅ Limite locale (si tu veux un contrôle par spawner)

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            TrySpawnSkeleton();
            timer = 0f;
        }
    }

    void TrySpawnSkeleton()
    {
        if (skeletonPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("Spawner : prefab ou spawnPoint manquant !");
            return;
        }

        // ✅ Vérifie la limite globale via SkeletonManager
        if (SkeletonManager.Instance != null && !SkeletonManager.Instance.CanSpawn())
        {
            Debug.Log("❌ Nombre maximum de squelettes atteint !");
            return;
        }

        // ✅ Création du squelette
        Vector3 spawnPos = spawnPoint.position + Vector3.up * 0.2f;
        GameObject instance = Instantiate(skeletonPrefab, spawnPos, spawnPoint.rotation);

        // ✅ Enregistrement du squelette dans le manager
        if (SkeletonManager.Instance != null)
            SkeletonManager.Instance.RegisterSkeleton(instance);

        // ✅ Assignation de la cible
        var ai = instance.GetComponent<SkeletonAI>();
        if (ai && Camera.main != null)
            ai.target = Camera.main.transform;
    }
}
