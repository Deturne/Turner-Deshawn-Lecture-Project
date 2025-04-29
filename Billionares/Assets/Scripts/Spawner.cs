using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Spawner : MonoBehaviour
{
    public float spawnRate = 5;
    public float spawnTime = 0;
    [SerializeField] GameObject Billion;
    [SerializeField] GameObject specialBillion;
    public int specialBillionCount = 5; // Number of special billions to spawn
    private int remainingSpecialSpawns = 0;
    public string teamReference;
    public int rank = 1;
    public Vector2 spawnPoint; // Spawn point for the base
    private bool specialSpawningActive = false; // Track if special spawning is active

    void Start()
    {
        
    }

    void Update()
    {
        //Debug.Log($"Spawner for team {teamReference}: Remaining special spawns: {remainingSpecialSpawns}, Special spawning active: {specialSpawningActive}");
        spawnBillion();
    }

    void spawnBillion()
    {
        float x = gameObject.transform.position.x + 1.1f;
        float y = gameObject.transform.position.y - 0.242257f;
        Vector2 spawnPos = new Vector2(UnityEngine.Random.Range(x, x - 0.2f), UnityEngine.Random.Range(y, y - 0.2f));

        if (Time.time >= spawnTime)
        {
            spawnTime = Time.time + spawnRate;

            if (specialSpawningActive && remainingSpecialSpawns > 0)
            {
                Debug.Log($"Spawning special billion. Remaining special spawns: {remainingSpecialSpawns}");
                GameObject billionInstance = Instantiate(specialBillion, spawnPos, Quaternion.identity);

                Billion billionScript = billionInstance.GetComponent<Billion>();
                if (billionScript != null)
                {
                    Debug.Log("Spawned a special billion.");
                }

                remainingSpecialSpawns--;

                // If no more special billions are left to spawn, deactivate special spawning
                if (remainingSpecialSpawns <= 1)
                {
                    specialSpawningActive = false;
                    Debug.Log("Special billion spawning completed.");
                }
            }
            else
            {
                Debug.Log("Spawning regular billion.");
                GameObject billionInstance = Instantiate(Billion, spawnPos, Quaternion.identity);

                Billion billionScript = billionInstance.GetComponent<Billion>();
                if (billionScript != null)
                {
                    Debug.Log("Spawned a regular billion.");
                }
            }
        }
    }

    public void ActivateSpecialBillionSpawning(string teamName)
    {
        if (teamReference == teamName && !specialSpawningActive)
        {
            Debug.Log($"Activating special billion spawning for team: {teamName}");
            remainingSpecialSpawns = specialBillionCount;
            specialSpawningActive = true; // Mark special spawning as active
        }
        else if (specialSpawningActive)
        {
            Debug.Log($"Special spawning already active for team: {teamName}");
        }
    }
}
    



    

   
    
       
    
