using UnityEngine;

public class SeedTrigger : MonoBehaviour
{
    // Reference to the parent DirtInteractable
    public Dirt parentDirt;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Seed") && parentDirt.state == "Hole")
        {
            // Destroy the seed
            Destroy(other.gameObject);

            // Call ChangeState on parent
            parentDirt.ChangeState("Seeded");
        }
    }
}