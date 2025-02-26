using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    float spawnRate = 5;
    float spawnTime = 0;
    [SerializeField] GameObject Billion;
    



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        spawnBillion();
    }

    void spawnBillion()
    {
        
        float x = gameObject.transform.position.x + 1.1f;
        float y = gameObject.transform.position.y - 0.242257f;
        // Spawn Location for Billions
        Vector2 spawnPos = new Vector2(Random.Range(x,x - 0.2f), Random.Range(y,y - 0.2f));
        //Spawning logic
        if (Time.time >= spawnTime) 
        {
            spawnTime = Time.time + spawnRate;
            Instantiate(Billion, spawnPos, Quaternion.identity);

        }
    }

   
    
       
    
}