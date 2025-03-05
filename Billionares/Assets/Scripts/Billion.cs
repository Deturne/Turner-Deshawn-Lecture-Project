using System.Collections.Generic;
using UnityEngine;

public class Billion : MonoBehaviour
{
    public string flagTag; // Set this to "GreenFlag" or "YellowFlag" in the Inspector
    public float speed = 5f;
    public float accel = 2f;

    public float maxSpeed = 4;
    private Rigidbody2D rb;
    private Transform targetFlag;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetFlag = null;
    }

    void FixedUpdate()
    {
        UpdateTargetFlag(); // Find nearest flag each frame

        if (targetFlag != null)
        {
            MoveTowardsFlag();
        }
        else
        {
            SlowDown(); // Stop moving if no flag exists
        }
    }

    void UpdateTargetFlag()
    {
        GameObject[] flags = GameObject.FindGameObjectsWithTag(flagTag);
        if (flags.Length == 0)
        {
            targetFlag = null;
            return;
        }

        float shortestDistance = Mathf.Infinity;
        Transform closestFlag = null;

        foreach (GameObject flag in flags)
        {
            float distance = Vector2.Distance(transform.position, flag.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestFlag = flag.transform;
            }
        }

        targetFlag = closestFlag;
    }

    void MoveTowardsFlag()
    {
        float distance = Vector2.Distance(transform.position, targetFlag.position);

        if (distance < 0.2f) // Stop moving when close
        {
            SlowDown();
            
            return;
        }

        Vector2 direction = (targetFlag.position - transform.position).normalized;
        Vector2 velocity = direction * maxSpeed;

        
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, velocity, accel * Time.fixedDeltaTime);
        
    }

    void SlowDown()
    {
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, 0.3f);
    }
}
