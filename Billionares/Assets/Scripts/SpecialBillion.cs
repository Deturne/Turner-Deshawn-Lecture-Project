using UnityEngine;

public class SpecialBillion : Billion
{
    public GameObject visualIndicatorPrefab; // Translucent item prefab
    public GameObject rocketBlobPrefab; // Rocket blob prefab
    public float rocketFireInterval = 7f;

    private float lastRocketFireTime;

    new void Start()
    {
        base.Start();
        teamName = base.teamName;

        // Add a visual indicator above the special billion
        if (visualIndicatorPrefab != null)
        {
            GameObject indicator = Instantiate(visualIndicatorPrefab, transform.position + Vector3.up * 1.1f, Quaternion.identity, transform);

            // Make the indicator translucent
            SpriteRenderer spriteRenderer = indicator.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 0.5f; // Set alpha to 50% transparency
                spriteRenderer.color = color;
            }
            else
            {
                Debug.LogWarning("No SpriteRenderer found on visualIndicatorPrefab.");
            }
        }
    }

    new void FixedUpdate()
    {
        // Call the base class FixedUpdate
        base.FixedUpdate();

        if (base.targetEnemy != null)
        {
            Debug.Log($"SpecialBillion FixedUpdate: Target enemy is {base.targetEnemy.name}");
        }
        else
        {
            Debug.LogWarning("SpecialBillion FixedUpdate: No target enemy assigned.");
        }

        // Fire rocket blobs at intervals
        if (Time.time >= lastRocketFireTime + rocketFireInterval)
        {
            FireRocketBlob();
            lastRocketFireTime = Time.time;
        }
    }

    void FireRocketBlob()
    {
        if (base.targetEnemy == null)
        {
            Debug.LogWarning("Cannot fire rocket blob: No target enemy assigned.");
            return;
        }

        GameObject rocketBlob = Instantiate(rocketBlobPrefab, transform.position, Quaternion.identity);
        RocketBlob blobScript = rocketBlob.GetComponent<RocketBlob>();
        if (blobScript != null)
        {
            blobScript.SetTarget(base.targetEnemy);
            blobScript.SetOwnerTeam(teamName); // Pass the team of the SpecialBillion to the RocketBlob
            blobScript.SetOriginatingBillion(this); // Pass the SpecialBillion as the originating Billion
        }
    }

    
}

