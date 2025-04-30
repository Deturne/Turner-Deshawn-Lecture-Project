using System;
using System.Collections;
using UnityEngine;

public class RocketBlob : MonoBehaviour
{
    public float speed = 2f;
    public float explosionRadius = 3f;
    public int damage = 10;
    public float explosionDuration = 1f;
    public string teamName; // The team that owns this rocket blob
    private Billion originatingBillion; // Reference to the originating Billion

    public GameObject explosionEffectPrefab; // Prefab for the explosion effect

    private Transform target;
    private Vector3 direction; // Fixed direction for linear movement
    private bool hasExploded = false; // Track if the blob has already exploded

    public void SetTarget(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("Target is null. Cannot set target for RocketBlob.");
            return;
        }
        this.target = target;

        // Calculate the initial direction to the target
        direction = (target.position - transform.position).normalized;
        Debug.Log($"RocketBlob target set to: {target.name}, initial direction: {direction}");
    }

    public void SetOwnerTeam(string team)
    {
        if (team == null)
        {
            Debug.LogWarning("Owner team is null. Cannot set owner team for RocketBlob.");
            return;
        }
        teamName = team;
        Debug.Log($"RocketBlob assigned to team: {teamName}");
    }

    public void SetOriginatingBillion(Billion billion)
    {
        if (billion == null)
        {
            Debug.LogWarning("Originating Billion is null. Cannot set originating Billion for RocketBlob.");
            return;
        }
        originatingBillion = billion;
        Debug.Log($"RocketBlob originating from: {originatingBillion.name}");
    }

    void Update()
    {
        if (hasExploded) return; // Stop movement if the blob has already exploded

        if (target == null)
        {
            Explode(); // Explode if the target is null
            return;
        }

        // Move the RocketBlob along the fixed direction
        transform.position += direction * speed * Time.deltaTime;

        // Check if the RocketBlob has reached the target
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return; // Prevent multiple explosions
        hasExploded = true; // Mark as exploded

        // Instantiate explosion effect
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Expand the RocketBlob to match the explosion radius
        StartCoroutine(ExpandAndDamage());

        // Destroy the RocketBlob after the explosion
        Destroy(gameObject, explosionDuration);
    }

    private IEnumerator ExpandAndDamage()
    {
        float initialScale = transform.localScale.x;
        float targetScale = explosionRadius * 1.2f; // Diameter of the explosion
        float expansionTime = 0.2f; // Time it takes to expand
        float elapsedTime = 0f;

        while (elapsedTime < expansionTime)
        {
            elapsedTime += Time.deltaTime;
            float scale = Mathf.Lerp(initialScale, targetScale, elapsedTime / expansionTime);
            this.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        // Trigger damage for all objects within the explosion radius
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        Debug.Log($"RocketBlob detected {hitObjects.Length} objects within explosion radius.");

        foreach (Collider2D collider in hitObjects)
        {
            Debug.Log($"Detected object: {collider.name}");

            // Check for a Damage component on the object or its parent
            Damage damageComponent = collider.GetComponent<Damage>() ?? collider.GetComponentInParent<Damage>();
            if (damageComponent != null)
            {
                Debug.Log($"Applying damage to object: {collider.name}");
                damageComponent.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning($"Object {collider.name} is missing a Damage component!");
            }

            Billion billion = collider.GetComponent<Billion>();
            if (billion != null)
            {
                
               
                Rigidbody2D rb = billion.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 forceDirection = (billion.transform.position - transform.position).normalized;
                    float forceMagnitude = 0.2f; // Adjust the force magnitude as needed
                    StartCoroutine(ApplyForceOverTime(rb, forceDirection * forceMagnitude, 0.1f));
                }
                
                

            }
        }

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject); // Destroy the RocketBlob after the explosion
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExploded) return; // Skip if already exploded

        //Debug.Log($"RocketBlob collided with: {other.name}");

        // Ignore collisions with other RocketBlob instances
        if (other.CompareTag("Beam"))
        {
            Debug.Log($"Skipping collision with another RocketBlob: {other.name}");
            return;
        }

        // Ignore collisions with colored flags or other beams
        if (other.CompareTag("Yellow Flag") || other.CompareTag("Green Flag") || other.CompareTag("Beam") ||
            other.CompareTag("Blue Flag") || other.CompareTag("Red Flag"))
        {
            return;
        }

        // Check if the collided object is a SpecialBillion
        SpecialBillion specialBillion = other.GetComponent<SpecialBillion>();
        if (specialBillion != null)
        {
            // Ignore friendly SpecialBillions and the originating SpecialBillion
            if (specialBillion.teamName == this.teamName || specialBillion == originatingBillion)
            {
                Debug.Log($"Skipping collision with SpecialBillion: {specialBillion.name}");
                return;
            }

            // Apply damage to enemy SpecialBillions
            Damage damageComponent = specialBillion.GetComponent<Damage>();
            if (damageComponent != null)
            {
                damageComponent.TakeDamage(damage);
            }

            Debug.Log($"RocketBlob exploded on SpecialBillion: {specialBillion.name}. Damage dealt: {damage}");
            Explode(); // Trigger explosion
            return;
        }

        // Check if the collided object is a regular Billion
        Billion billion = other.GetComponent<Billion>();
        if (billion != null)
        {
            // Ignore friendly billions and the originating Billion
            if (billion.teamName == this.teamName || billion == originatingBillion)
            {
                Debug.Log($"Skipping collision with Billion: {billion.name}");
                return;
            }

            // Apply damage to enemy billions
            Damage damageComponent = billion.GetComponent<Damage>();
            if (damageComponent != null)
            {
                damageComponent.TakeDamage(damage);
                Debug.Log($"RocketBlob exploded on Billion: {billion.name}. Damage dealt: {damage}");
            }
            else
            {
                Debug.LogWarning($"Billion {billion.name} is missing a Damage component!");
            }

            //Debug.Log($"RocketBlob exploded on Billion: {billion.name}. Damage dealt: {damage}");
            Explode(); // Trigger explosion
            return;
        }


        // If the object is not friendly and doesn't match any of the above, explode
        Debug.Log($"RocketBlob exploded on non-friendly object: {other.name}");
        Explode();
    }

    void OnDrawGizmosSelected()
    {
        // Draw the explosion radius in the Scene view for debugging
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
    IEnumerator ApplyForceOverTime(Rigidbody2D rb, Vector2 force, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            if(rb == null) yield break; // Exit if the Rigidbody2D is destroyed
            rb.AddForce(force * Time.deltaTime, ForceMode2D.Force);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
