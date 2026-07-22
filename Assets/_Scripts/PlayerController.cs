using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private float moveInputX = 0f;
    private float moveInputY = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInputX = Input.GetAxis("Horizontal");
        moveInputY = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        Vector2 movement = new Vector2(moveInputX, moveInputY) * moveSpeed;
        rb.velocity = new Vector2(movement.x, rb.velocity.y);
    }
}