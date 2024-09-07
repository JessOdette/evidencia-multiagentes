using UnityEngine;

public class EnemyDisappear : MonoBehaviour
{
    // Nombre del objeto que queremos detectar (dron)
    public string droneTag = "dron(render)";

    // Este método se llama cuando otro objeto entra en el trigger
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que tocó el trigger es el dron
        if (other.CompareTag(droneTag))
        {
            // Hacer desaparecer el objeto 
            Destroy(gameObject);
        }
    }
}