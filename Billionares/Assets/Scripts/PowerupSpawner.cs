using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    public GameObject powerupCratePrefab; // Assign the crate prefab in the Unity Editor
    public float spawnInterval = 10f; // Time between spawns
    public LayerMask unsuitableLayers; // Layers for walls and bases
    public Vector2 arenaBounds; // Define the arena size (e.g., width and height)

    private GameObject currentCrate;

    void Start()
    {
        InvokeRepeating(nameof(SpawnCrate), spawnInterval, spawnInterval);
    }

    void SpawnCrate()
    {
        if (currentCrate != null) return; // Only one crate at a time

        Vector2 spawnPosition;
        int maxAttempts = 10; // Prevent infinite loops
        int attempts = 0;

        do
        {
            spawnPosition = new Vector2(
                Random.Range(-arenaBounds.x / 2, arenaBounds.x / 2),
                Random.Range(-arenaBounds.y / 2, arenaBounds.y / 2)
            );
            attempts++;
        }
        while (Physics2D.OverlapCircle(spawnPosition, 1f, unsuitableLayers) != null && attempts < maxAttempts);

        if (attempts < maxAttempts)
        {
            currentCrate = Instantiate(powerupCratePrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void CrateCollected()
    {
        if (currentCrate != null)
        {
            Destroy(currentCrate);
            currentCrate = null;
        }
    }
}
