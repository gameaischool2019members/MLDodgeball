using UnityEngine;

public class DodgeBall : MonoBehaviour, IObservable
{
    public Rigidbody rb;
    public bool isHeld;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (transform.position.y < -100)
        {
            transform.localPosition = new Vector3(0, 0.75f, 0);
            rb.velocity = Vector3.zero;
        }
    }

    public float[] GetObservations()
    {
        return new float[] {
            rb.position.x,
            rb.position.y,
            rb.position.z,
            rb.velocity.x,
            rb.velocity.y,
            rb.velocity.z
        };
    }
}

