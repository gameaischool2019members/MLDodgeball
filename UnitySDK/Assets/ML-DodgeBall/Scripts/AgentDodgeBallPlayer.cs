using UnityEngine;
using MLAgents;

public class AgentDodgeBallPlayer : Agent, IObservable
{
    public float moveSpeed;
    public GameManager gameManager;
    public AgentDodgeBallPlayer rival;

    [HideInInspector]
    public Rigidbody agentRb;

    public Vector3 StartPosition;
    public Quaternion StartRotation;

    public bool hasBall;
    public Transform ballHolder;

    [Header("Debug")]
    public float angle, distanceToBall;

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        agentRb = GetComponent<Rigidbody>();

        StartPosition = transform.position;
        StartRotation = transform.rotation;
    }

    public override void CollectObservations()
    {
        Vector3 dodgeBallPosition = gameManager.dodgeBall.transform.position;
        Vector3 toDodgeBall = gameManager.dodgeBall.transform.position - transform.position;

        angle = Vector3.SignedAngle(transform.forward, toDodgeBall, Vector3.up);
        prevDistance = distanceToBall;
        distanceToBall = toDodgeBall.sqrMagnitude;

        AddVectorObs(angle);
        AddVectorObs(distanceToBall);

        Debug.DrawRay(transform.position, transform.forward * 2, Color.white, 0.5f);
        Debug.DrawRay(transform.position, toDodgeBall, Color.red, 0.5f);
    }

    #region Actions
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        MoveAgent(vectorAction);
    }
    float prevDistance = float.PositiveInfinity;

    public void MoveAgent(float[] act)
    {
        Vector3 move = Vector3.zero;

        int movementAction = Mathf.FloorToInt(act[0]);

        bool throwBall = Mathf.FloorToInt(act[1]) == 1;

        switch (movementAction)
        {
            case 1:
                move = transform.forward;
                break;
            case 2:
                move = -transform.forward;
                break;
            case 3:
                move = transform.right;
                break;
            case 4:
                move = -transform.right;
                break;
        }

        if (throwBall && gameManager.dodgeBall.isHeld)
        {
            ThrowBall();
        }

        agentRb.MovePosition(transform.position + move * moveSpeed * Time.deltaTime);
    }

    [ContextMenu("Throw Ball")]
    public void ThrowBall()
    {
        gameManager.dodgeBall.rb.isKinematic = false;

        gameManager.dodgeBall.transform.parent = gameManager.transform;
        gameManager.dodgeBall.rb.AddForce(transform.forward * 25, ForceMode.Impulse);
        gameManager.dodgeBall.isHeld = false;
    }
    #endregion

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.GetComponent<DodgeBall>() && !gameManager.dodgeBall.isHeld)
        {
            Debug.Log("Collided with ball");

            AddReward(1);

            Done();
            return;

            gameManager.dodgeBall.transform.SetParent(ballHolder);
            gameManager.dodgeBall.transform.localPosition = Vector3.zero;

            gameManager.dodgeBall.isHeld = true;
            gameManager.dodgeBall.rb.isKinematic = true;
        }
    }

    public float[] GetObservations()
    {
        return new float[]
        {
            transform.position.x,
            transform.position.z,
        };
    }

    public override void AgentReset()
    {
        transform.SetPositionAndRotation(StartPosition, StartRotation);
        hasBall = false;

        gameManager.dodgeBall.transform.localPosition = new Vector3(0, 0.75f, 0);
        gameManager.dodgeBall.rb.velocity = Vector3.zero;
    }
}

