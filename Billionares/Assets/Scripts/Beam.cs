using UnityEngine;

public class Beam : MonoBehaviour
{
    public float speed = 10f;
    private Vector2 direction;
    public int damage;
    public Color teamColor;
    public void SetDirection(Vector2 direction)
    {
        this.direction = direction.normalized;
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore collisions with colored flags or other beams
        if (other.CompareTag("Yellow Flag") || other.CompareTag("Green Flag") || other.CompareTag("Beam"))
        {
            return;
        }

        // Check if the collided object is a friendly Billion
        Billion billion = other.GetComponent<Billion>();
        if (billion != null && billion.teamColor == this.teamColor)
        {
            return; // Ignore friendly billions
        }

        // Apply damage if the collided object has a Damage component
        Damage damageComponent = other.GetComponent<Damage>();
        if (damageComponent != null)
        {
            damageComponent.TakeDamage(damage);
        }

        // Destroy the beam on collision with any other object
        Destroy(gameObject);
    }
}
