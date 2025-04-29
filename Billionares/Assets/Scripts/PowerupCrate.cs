using System;
using UnityEngine;

public class PowerupCrate : MonoBehaviour
{
    public float collectionRange = 2f; // Range for collection
    public LayerMask billionLayer; // Layer for billions
    private PowerupSpawner spawner;

    void Start()
    {
        spawner = FindObjectOfType<PowerupSpawner>();
    }

    void Update()
    {
        Collider2D[] nearbyBillions = Physics2D.OverlapCircleAll(transform.position, collectionRange, billionLayer);
        if (nearbyBillions.Length > 1)
        {
            string firstColor = nearbyBillions[0].GetComponent<Billion>().teamName;
            bool allSameColor = true;

            foreach (Collider2D collider in nearbyBillions)
            {
                Billion billion = collider.GetComponent<Billion>();
                if (billion.teamName != firstColor)
                {
                    allSameColor = false;
                    break;
                }
            }

            if (allSameColor)
            {
                CollectCrate(firstColor);
            }
        }
    }

    void CollectCrate(string teamColor)
    {
        Debug.Log($"Powerup crate collected by team: {teamColor}");
        spawner.CrateCollected();

        // Find the spawner for the team that collected the crate
        Spawner[] allSpawners = FindObjectsByType<Spawner>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Debug.Log($"Found {allSpawners.Length} spawners in the scene.");
        foreach (Spawner spawner in allSpawners)
        {
            if (spawner.teamReference == teamColor)
            {
                if (spawner.remainingSpecialSpawns > 0)
                {
                    spawner.ActivateSpecialBillionSpawning(teamColor);
                    break;
                }
            }
        }

        Destroy(gameObject);
    }
}
