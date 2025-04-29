using UnityEngine;

public class BeamDestroyer : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered the trigger is a beam
        if (other.CompareTag("Beam"))
        {
            // Destroy the beam
            Destroy(other.gameObject);
            Debug.Log("Beam destroyed!");
        }
    }
}
