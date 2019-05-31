//Put this script on your blue cube.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class AgentDodgeBallPlayer : Agent
{
    public float spawnAreaMarginMultiplier = 3;
    public float agentRunSpeed = 5;
    public float rayDistance = 12f;
    public float[] rayAngles = { 0f, 15, 30, 45f, 60, 75, 90f, 105, 120, 135f, 150, 165, 180f };
    public string[] detectableObjects = new[] { "goal", "wall" };

    /// <summary>
    /// The ground. The bounds are used to spawn the elements.
    /// </summary>
	public GameObject ground;

    /// <summary>
    /// The area bounds.
    /// </summary>
	[HideInInspector]
    public Bounds areaBounds;

    public bool useVectorObs;
    
    Rigidbody agentRB;  //cached on initialization
    RayPerception rayPer;


    public override void InitializeAgent()
    {
        base.InitializeAgent();
        rayPer = GetComponent<RayPerception>();

        // Cache the agent rigidbody
        agentRB = GetComponent<Rigidbody>();

        // Get the ground's bounds
        areaBounds = ground.GetComponent<Collider>().bounds;
    }

    public override void CollectObservations()
    {
        if (useVectorObs)
        {
            AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        }
    }

    /// <summary>
    /// Use the ground's bounds to pick a random spawn position.
    /// </summary>
    public Vector3 GetRandomSpawnPos()
    {
        bool foundNewSpawnLocation = false;
        Vector3 randomSpawnPos = Vector3.zero;
        while (foundNewSpawnLocation == false)
        {
            float randomPosX = Random.Range(-areaBounds.extents.x * spawnAreaMarginMultiplier,
                                areaBounds.extents.x * spawnAreaMarginMultiplier);

            float randomPosZ = Random.Range(-areaBounds.extents.z * spawnAreaMarginMultiplier,
                                            areaBounds.extents.z * spawnAreaMarginMultiplier);
            randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 1f, randomPosZ);
            if (Physics.CheckBox(randomSpawnPos, new Vector3(2.5f, 0.01f, 2.5f)) == false)
            {
                foundNewSpawnLocation = true;
            }
        }
        return randomSpawnPos;
    }

    /// <summary>
    /// Moves the agent according to the selected action.
    /// </summary>
	public void MoveAgent(float[] act)
    {
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        int action = Mathf.FloorToInt(act[0]);

        // Goalies and Strikers have slightly different action spaces.
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
            case 5:
                dirToGo = transform.right * -0.25f;
                break;
            case 6:
                dirToGo = transform.right * 0.25f;
                break;
        }
        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        agentRB.AddForce(dirToGo * agentRunSpeed,
                         ForceMode.VelocityChange);

    }

    /// <summary>
    /// Called every step of the engine. Here the agent takes an action.
    /// </summary>
	public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Move the agent using the action.
        MoveAgent(vectorAction);

        // Penalty given each step to encourage agent to finish task quickly.
        AddReward(-1f / agentParameters.maxStep);
    }

    /// <summary>
    /// In the editor, if "Reset On Done" is checked then AgentReset() will be 
    /// called automatically anytime we mark done = true in an agent script.
    /// </summary>
	public override void AgentReset()
    {
        int rotation = Random.Range(0, 4);
        float rotationAngle = rotation * 90f;

        transform.position = GetRandomSpawnPos();
        agentRB.velocity = Vector3.zero;
        agentRB.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<DodgeBall>())
        {
            GrabBall(collision.gameObject.GetComponent<DodgeBall>());
        }
    }

    private void GrabBall(DodgeBall dodgeBall)
    {
        AddReward(5f);

        dodgeBall.transform.position = GetRandomSpawnPos();

        Done();
    }
}
