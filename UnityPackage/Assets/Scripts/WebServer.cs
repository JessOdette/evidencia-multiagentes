using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebServer : MonoBehaviour
{
    IEnumerator SendData(string data)
    {
        string url = "http://localhost:8585";
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(data);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + www.error);
            }
            else
            {
                Debug.Log("Response received: " + www.downloadHandler.text); // Verifica la respuesta
                try
                {
                    Vector3 tPos = JsonUtility.FromJson<Vector3>(www.downloadHandler.text);
                    Debug.Log("Parsed Position: " + tPos);

                    GameObject robot = GameObject.Find("Robot");  // Nombre del GameObject del robot
                    if (robot != null)
                    {
                        RobotAgent robotAgent = robot.GetComponent<RobotAgent>();
                        if (robotAgent != null)
                        {
                            robotAgent.UpdateAgentPosition(tPos);
                        }
                        else
                        {
                            Debug.LogError("RobotAgent component not found on Robot.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Robot GameObject not found.");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error parsing JSON: " + e.Message);
                }
            }
        }
    }


    void Start()
    {
        Debug.Log("WebServer Start method called."); // Verifica si Start se ejecuta
        string simulatedJson = "{\"x\": 3.44, \"y\": 0, \"z\": -15.707}";
        StartCoroutine(SendData(simulatedJson));
    }

    void Update()
    {
    }
}
