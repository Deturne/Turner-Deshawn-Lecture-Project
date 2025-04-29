using UnityEngine;

public class Beam : MonoBehaviour
{
    public float speed = 10f;
    private Vector2 direction;
    public int damage;
    [SerializeField] public string teamName;
    public string teamReference;
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
        if (other.CompareTag("Yellow Flag") || other.CompareTag("Green Flag") || other.CompareTag("Beam") || other.CompareTag("Blue Flag") || other.CompareTag("Red Flag"))
        {
            return;
        }

        // Check if the collided object is a friendly Billion
        Billion billion = other.GetComponent<Billion>();
        if (billion != null && billion.teamName == this.teamName)
        {
            return; // Ignore friendly billions
        }

        // Apply damage if the collided object has a Damage component
        Damage damageComponent = other.GetComponent<Damage>();
        if (damageComponent != null)
        {
            damageComponent.TakeDamage(damage, teamReference); 
        }

        if (billion != null && billion.teamName != this.teamName)
        {
            //billion.TakeDamage(damage, teamReference); // Pass the team reference to the billion
            //Debug.Log($"Beam hit billion. Awarding XP to team: {teamReference}");
        }
            Destroy(gameObject);


            // Destroy the beam on collision with any other object
           
    }
}
