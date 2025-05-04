
using System.Collections.Generic;
using UnityEngine;

public class ProcGenBases : MonoBehaviour
{
    public List<GameObject> basePrefabs; // List of color-coded base prefabs
    public Vector2 generationAreaSize = new Vector3(100f, 1f);
    public Transform parentContainer;

    public GameObject healthBarPrefab; // Prefab for the health bar
    public GameObject xpBarPrefab; // Prefab for the XP bar

    void Start()
    {
        // If no parentContainer provided, instances will be generated as children of the generator.
        if (parentContainer == null)
        {
            parentContainer = transform.root;
        }

        

        GenerateBases();
    }

    void GenerateBases()
    {
        List<Vector3> existingPositions = new List<Vector3>(); // Track positions of spawned bases
        float minDistance = 2f; // Minimum distance between bases
        const int maxAttempts = 10; // Limit the number of attempts to find a valid position

        foreach (GameObject basePrefab in basePrefabs)
        {
            Vector3 randomPosition;
            int attempts = 0;

            // Find a valid position for the base
            do
            {
                randomPosition = GetRandomPositionInGenerationArea();
                attempts++;
            } while (!IsPositionValid(randomPosition, existingPositions, minDistance) && attempts < maxAttempts);

            if (attempts >= maxAttempts)
            {
                Debug.LogWarning($"Failed to find a valid position for {basePrefab.name} after {maxAttempts} attempts.");
                continue; // Skip spawning this base if no valid position is found
            }

            existingPositions.Add(randomPosition); // Add the valid position to the list
            

            // Instantiate the base prefab at the valid position
            GameObject newBase = Instantiate(basePrefab, randomPosition, Quaternion.identity, parentContainer.transform);

            // Attach health and XP bars
            AttachHealthBar(newBase);
            AttachXPBar(newBase);
        }
    }

    void AttachHealthBar(GameObject baseObject)
    {
        if (healthBarPrefab != null)
        {
            
            Vector3 offset = new Vector3(0, 0, 0); 
            Vector3 healthBarPosition = baseObject.transform.position + offset;

            // Instantiate the health bar as a child of the base
            GameObject healthBarInstance = Instantiate(healthBarPrefab, healthBarPosition, Quaternion.identity);
            healthBarInstance.transform.SetParent(baseObject.transform);

            // Link the health bar to the base's Damage script
            Damage damageScript = baseObject.GetComponent<Damage>();
            if (damageScript != null)
            {
                RadialHealthBar healthBar = healthBarInstance.GetComponent<RadialHealthBar>();
                if (healthBar != null)
                {
                    damageScript.radialHealthBar = healthBar;

                    // Optionally, initialize the health bar's max health
                    healthBar.SetMaxHealth(damageScript.health);
                    healthBar.SetHealth(damageScript.health);
                }
                else
                {
                    Debug.LogWarning($"Health bar prefab is missing the RadialHealthBar component.");
                }
            }
            else
            {
                Debug.LogWarning($"Base {baseObject.name} is missing a Damage component.");
            }
        }
        else
        {
            Debug.LogWarning("Health bar prefab is not assigned in the ProceduralGenerator script.");
        }
    }

    void AttachXPBar(GameObject baseObject)
    {
        if (xpBarPrefab != null)
        {
            // Offset the XP bar above the health bar
            Vector3 offset = new Vector3(0, 0, 0); // Adjust the Y offset as needed
            Vector3 xpBarPosition = baseObject.transform.position + offset;

            // Instantiate the XP bar as a child of the base
            GameObject xpBarInstance = Instantiate(xpBarPrefab, xpBarPosition, Quaternion.identity);
            xpBarInstance.transform.SetParent(baseObject.transform);

            // Link the XP bar to the base's Team script
            Team teamScript = baseObject.GetComponent<Team>();
            if (teamScript != null)
            {
                RadialHealthBar xpBar = xpBarInstance.GetComponent<RadialHealthBar>();
                if (xpBar != null)
                {
                    teamScript.xpBar = xpBar;

                    // Optionally, initialize the XP bar
                    xpBar.xpSlider.value = 0;
                }
                else
                {
                    Debug.LogWarning($"XP bar prefab is missing the RadialHealthBar component.");
                }
            }
            else
            {
                Debug.LogWarning($"Base {baseObject.name} is missing a Team component.");
            }
        }
        else
        {
            Debug.LogWarning("XP bar prefab is not assigned in the ProceduralGenerator script.");
        }
    }

    Vector2 GetRandomPositionInGenerationArea()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-generationAreaSize.x / 2, generationAreaSize.x / 2),
            Random.Range(-generationAreaSize.y / 2, generationAreaSize.y / 2));

        

        return transform.position + randomPosition;
    }

    // Draw Gizmo to visualize the generation area
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, generationAreaSize);
    }
    bool IsPositionValid(Vector3 position, List<Vector3> existingPositions, float minDistance)
    {
        foreach (Vector3 existingPosition in existingPositions)
        {
            if (Vector3.Distance(position, existingPosition) < minDistance)
            {
                return false; // Position is too close to an existing base
            }
        }
        return true;
    }
}
