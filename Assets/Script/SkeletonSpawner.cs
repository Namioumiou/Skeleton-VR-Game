using UnityEngine;

public class SkeletonSpawner : MonoBehaviour
{
    [Header("Réglages du spawn")]
    public GameObject skeletonPrefab; // Le prefab du squelette
    public Transform spawnPoint;      // L’endroit où il apparaît (l’arbre)
    public float spawnInterval = 10f; // Temps entre chaque spawn (en secondes)

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnSkeleton();
            timer = 0f;
        }
    }

    void SpawnSkeleton()
    {
        if (skeletonPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("Spawner : prefab ou spawnPoint manquant !");
            return;
        }

        // ✅ Création du squelette avec léger offset pour éviter qu’il flotte ou s’enfonce
        Vector3 spawnPos = spawnPoint.position + Vector3.up * 0.2f;
        var instance = Instantiate(skeletonPrefab, spawnPos, spawnPoint.rotation);

        // ✅ Assignation de la cible
        var ai = instance.GetComponent<SkeletonAI>();
        if (ai && Camera.main != null)
            ai.target = Camera.main.transform;
    }
}
