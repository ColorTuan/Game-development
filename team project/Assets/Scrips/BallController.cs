using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private bool isGround = true;

    [SerializeField] private float speed = 0.05f;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(speed * h, 0f,0f);
        transform.position += movement; 
        
    }

    void OnCollisionEnter2D(Collider2D other)
    {
        if (other.GetComponent<Collider>().tag == "Ground")
        {
            isGround = true;
            Debug.Log(isGround);
        }
    
        if (Input.GetKey(KeyCode.Space) & isGround)
        {
            rb.AddForce(transform.up * speed, ForceMode2D.Impulse);
        }
    }

    
}
