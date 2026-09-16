using UnityEngine;

public class FingorMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 20f;
    public Rigidbody2D rb;
    public bool isActiveFingor = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.position += Vector3.left * moveSpeed * Time.fixedDeltaTime;
        if (isActiveFingor) rb.linearVelocity = moveSpeed * Vector3.left;
        else rb.linearVelocity = Vector3.zero;
    }
    
}
