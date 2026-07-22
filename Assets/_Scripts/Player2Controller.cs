using UnityEngine;

public class Player2Controller : MonoBehaviour
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
        // Dùng phím WASD cho Player 2
        if (Input.GetKey(KeyCode.A)) moveInputX = -1;
        else if (Input.GetKey(KeyCode.D)) moveInputX = 1;
        else moveInputX = 0;

        if (Input.GetKey(KeyCode.W)) moveInputY = 1;
        else if (Input.GetKey(KeyCode.S)) moveInputY = -1;
        else moveInputY = 0;
    }

    void FixedUpdate()
    {
        Vector2 movement = new Vector2(moveInputX, moveInputY) * moveSpeed;
        rb.velocity = new Vector2(movement.x, rb.velocity.y);
    }
}