using UnityEngine;

public class SirenLight : MonoBehaviour
{
    private Light pointLight;
    public Color color1 = Color.red;
    public Color color2 = Color.blue;
    public float speed = 1.0f;

void Start()
{
    pointLight = GetComponent<Light>();
    if (pointLight == null)
    {
        Debug.LogError("No Light component found on " + gameObject.name);
    }
}



void Update()
{
    float t = Mathf.PingPong(Time.time * speed, 1.0f);
    pointLight.color = Color.Lerp(Color.red, Color.blue, t);
}

}
