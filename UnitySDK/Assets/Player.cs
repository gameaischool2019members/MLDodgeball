using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 2;
    public float jumpForce = 100;
    public bool grounded = false;
    private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(moveX, 0, moveY).normalized;

        rigidbody.MovePosition(transform.position + move * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rigidbody.AddForce(jumpForce * Vector3.up);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            grounded = true;
        }

        if (collision.gameObject.CompareTag("ball"))
        {
            Application.LoadLevel(0);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            grounded = false;
        }
    }
}
