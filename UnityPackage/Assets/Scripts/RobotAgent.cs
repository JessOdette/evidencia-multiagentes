using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAgent : MonoBehaviour
{
    public GameObject robotModel;
    public Vector3 agentPosition;

    void Start()
    {
        Debug.Log("RobotAgent Start method called.");
        if (robotModel == null)
        {
            robotModel = this.gameObject;
        }

        if (robotModel == null)
        {
            Debug.LogError("Robot model not assigned.");
        }
        else
        {
            Debug.Log("Robot model assigned: " + robotModel.name);
        }
    }

    void Update()
    {
        if (robotModel != null)
        {
            robotModel.transform.position = agentPosition;
            Debug.Log("Robot position updated: " + agentPosition);
        }
    }

    public void UpdateAgentPosition(Vector3 newPosition)
    {
        Debug.Log("UpdateAgentPosition called with: " + newPosition);
        agentPosition = newPosition;
    }
}