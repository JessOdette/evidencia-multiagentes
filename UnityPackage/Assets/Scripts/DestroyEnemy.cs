using UnityEngine;

public class DestroyEnemy : MonoBehaviour
{
    // This method is called when another object enters the trigger zone
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detected with: " + other.gameObject.name);

        if (other.gameObject.CompareTag("Dron"))
        {
            Debug.Log("Dron detected!");
            Destroy(gameObject);
        }
    }
}