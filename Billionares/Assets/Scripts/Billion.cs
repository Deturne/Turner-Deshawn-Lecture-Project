using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Billion : MonoBehaviour
{
    public string flagTag;
    public float speed = 5f;
    public float accel = 2f;
    public float maxSpeed = 4;

    private Rigidbody2D rb;
    private Transform targetFlag;
    private Transform targetEnemy; // For turret targeting

    public GameObject turret;
    public GameObject turretInstance;
    public string teamColor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetFlag = null;
        turretInstance = Instantiate(turret, transform.position, Quaternion.identity);
        turretInstance.transform.parent = transform; // Make turret a child of the billion
    }

    void FixedUpdate()
    {
        UpdateTargetFlag(); // Find nearest flag each frame
        UpdateTargetEnemy(); // Find nearest enemy each frame

        if (targetFlag != null)
        {
            MoveTowardsFlag();
        }
        else
        {
            SlowDown(); // Stop moving if no flag exists
        }

        UpdateTurretRotation();
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

    void UpdateTargetEnemy()
    {
        // Find all enemy billions (those with a different teamColor)
        Billion[] allBillions = Object.FindObjectsOfType<Billion>();
        List<Billion> enemyBillions = new List<Billion>();

        foreach (Billion billion in allBillions)
        {
            if (billion.teamColor != teamColor && billion != this)
            {
                enemyBillions.Add(billion);
            }
        }

        if (enemyBillions.Count == 0)
        {
            targetEnemy = null;
            return;
        }

        // Find the nearest enemy
        float shortestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Billion enemy in enemyBillions)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        targetEnemy = closestEnemy;
    }

    void UpdateTurretRotation()
    {
        if (turretInstance == null) return;

        if (targetEnemy != null)
        {
            // Calculate direction to target
            Vector2 direction = targetEnemy.position - turretInstance.transform.position;
            // Calculate angle in degrees
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
            // Apply rotation to turret
            turretInstance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            // No enemies - point turret forward 
            turretInstance.transform.rotation = Quaternion.identity;
        }
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