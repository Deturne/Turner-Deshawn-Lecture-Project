using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Billion : MonoBehaviour
{
    public string flagTag;
    public float speed = 5f;
    public float accel = 2f;
    public float maxSpeed = 4;

    private Rigidbody2D rb;
    public Transform targetFlag;
    public Transform targetEnemy; // For turret targeting

    public GameObject turret;
    public GameObject turretInstance;
    public string teamName; // Use this for team assignment and comparison

    public GameObject beam;
    public float fireRate = 2.5f; // Time between shots
    private float lastShotTime = 0f; // Track last shot
    private bool isShooting = false; // Prevent multiple coroutines

    public bool isOnBase = false; // Flag to indicate if the turret is on a base
    public float rotationSpeed = 30f; // Rotation speed in degrees per second

    public GameObject radialHealthBarPrefab; // Assign the prefab in the Unity Editor
    private RadialHealthBar radialHealthBar;

    public int rank = 1; // Rank of the billion
    public int health;
    public int damage;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetFlag = null;
        turretInstance = Instantiate(turret, transform.position, Quaternion.identity);
        turretInstance.transform.parent = transform;

        Debug.Log($"{name} assigned to team: {teamName}");
    }

    public void FixedUpdate()
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
        Shoot();
    }

    void UpdateTargetFlag()
    {
        if (flagTag != "")
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
        else
        {
            targetFlag = null; // No flag tag set, so no target
        }
    }

    public void UpdateTargetEnemy()
    {
        // Find all enemy billions (those with a different teamName)
        Billion[] allBillions = Object.FindObjectsByType<Billion>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        List<Billion> enemyBillions = new List<Billion>();

        foreach (Billion billion in allBillions)
        {
            // Exclude friendly billions and SpecialBillions
            if (billion.teamName != teamName && billion != this)
            {
                enemyBillions.Add(billion);
            }

            SpecialBillion specialBillion = billion as SpecialBillion;
            if (specialBillion != null)
            {
                if (specialBillion.teamName != teamName && specialBillion != this)
                {
                    //Debug.Log($"Excluding friendly SpecialBillion: {specialBillion.name}");
                    enemyBillions.Add(specialBillion);

                }
            }

            if(isOnBase && billion.CompareTag("Base"))
            {
                //Debug.Log($"Excluding friendly Base: {billion.name}");
                enemyBillions.Remove(billion);
            }

        }

        if (enemyBillions.Count == 0)
        {
            targetEnemy = null;
            Debug.LogWarning("No enemy billions found.");
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

        if (closestEnemy != null)
        {
            targetEnemy = closestEnemy;
            Debug.Log($"Target enemy assigned: {targetEnemy.name}");
        }
        else
        {
            targetEnemy = null;
            Debug.LogWarning("No closest enemy found.");
        }
    }

    public void UpdateTurretRotation()
    {
        if (turretInstance == null) return;

        if (targetEnemy != null)
        {
            // Calculate direction to target
            Vector2 direction = targetEnemy.position - turretInstance.transform.position;
            // Calculate angle in degrees
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;

            if (isOnBase)
            {
                // Slowly rotate towards the target angle
                float currentAngle = turretInstance.transform.rotation.eulerAngles.z;
                float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
                turretInstance.transform.rotation = Quaternion.Euler(0, 0, newAngle);
            }
            else
            {
                // Snap to the target angle
                turretInstance.transform.rotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
            }
        }
        else
        {
            // No enemies - point turret forward 
            turretInstance.transform.rotation = Quaternion.identity;
        }
    }

    void MoveTowardsFlag()
    {
        if (rb == null || targetFlag == null) return; // Ensure Rigidbody2D and targetFlag are set
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
        if (rb != null)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, 0.3f);
        }
    }

    void Shoot()
    {
        if (targetEnemy == null || isShooting) return; // Prevent multiple shots at once

        if (Time.time >= lastShotTime + fireRate)
        {
            lastShotTime = Time.time; // Update last shot time
            StartCoroutine(ShootCoroutine());
        }
    }

    IEnumerator ShootCoroutine()
    {
        if (targetEnemy == null) yield break; // Exit if no target

        yield return new WaitForSeconds(1.5f); // Initial delay before shooting

        // Wait until the turret has finished rotating
        while (!IsTurretRotationComplete())
        {
            yield return null; // Wait for the next frame
        }

        if (targetEnemy != null && beam != null)
        {
            GameObject beamInstance = Instantiate(beam, turretInstance.transform.position, turretInstance.transform.rotation);
            beamInstance.transform.parent = transform; // Make the beam a child of the Billion instance
            

            Beam beamScript = beamInstance.GetComponent<Beam>();
            if (beamScript != null)
            {
                Vector2 direction = (targetEnemy.position - beamInstance.transform.position).normalized;
                beamScript.SetDirection(direction);
                beamScript.teamName = teamName; // Set the team name for the beam

            }
        }
    }

    bool IsTurretRotationComplete()
    {
        if (targetEnemy == null) return true;

        Vector2 direction = targetEnemy.position - turretInstance.transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        float currentAngle = turretInstance.transform.rotation.eulerAngles.z;

        return Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) < 1f; // Adjust the threshold as needed
    }
}
