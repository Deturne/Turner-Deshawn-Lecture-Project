using UnityEngine;

public class Damage : MonoBehaviour
{
    private int maxHealth = 100;
    private int health;

    public GameObject damageRing;
    private Vector3 initialScale; // Store the initial scale of the object
    private int clickCount = 0; // Track the number of clicks
    private bool isTakingDamage = false; // Prevent multiple damage calls per click

    
    void Start()
    {
        health = maxHealth;
        if (damageRing != null)
        {
            damageRing = Instantiate(damageRing, transform.position, Quaternion.identity);
        }
        initialScale = transform.localScale; // Save the initial scale
    }

  
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && !isTakingDamage)
            {
                Damage damageable = hit.collider.gameObject.GetComponent<Damage>();

                if (damageable != null && damageable == this) // Ensure the clicked object is this one
                {
                    //isTakingDamage = true; // Prevent multiple damage calls
                    clickCount++; // Increment click count
                    damageable.TakeDamage(30);
                    Debug.Log("Clicked on " + hit.collider.gameObject.name + ", Click number: " + clickCount);
                    Debug.Log("Health: " + damageable.health + ", Scale: " + damageable.transform.localScale);
                }
                else
                {
                    Debug.Log("No object or wrong object");
                }
            }
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isTakingDamage = false; // Reset the flag when the mouse button is released
        }

        if (damageRing != null)
        {
            damageRing.transform.position = gameObject.transform.position;
        }
    }

    public void TakeDamage(int damage)
    {
        health = health - damage;
        float dmgProgress = 1 - ((float)health / maxHealth);

        // Ensure the object only shrinks and doesn't grow
        float scale = Mathf.Lerp(initialScale.x, 0, dmgProgress);
        if (scale < transform.localScale.x) // Only apply scaling if it's smaller than the current scale
        {
            transform.localScale = new Vector3(scale, scale, initialScale.z);
        }

        Debug.Log("Health: " + health + ", Scale: " + transform.localScale);

        if (health <= 0)
        {
            if (damageRing != null)
            {
                Destroy(damageRing); // Destroy the damage ring
            }
            Destroy(gameObject); // Destroy the main object
        }
    }
}