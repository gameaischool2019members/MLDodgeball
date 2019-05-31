using UnityEngine;

public class DodgeBall : MonoBehaviour
{
    public Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
}

