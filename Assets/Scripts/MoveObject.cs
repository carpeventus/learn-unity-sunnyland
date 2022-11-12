using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour {

    [Header("Direction.1 Left-Right 2 Up-Down")]
    public int moveType;
    public float moveSpeed;

    
    [Header("Horizontal")]
    public float leftOffset;
    public float rightOffset;
    private float leftY;
    private float rightY;
    
    [Header("Vertical")]
    public float upOffset;
    public float downOffset;
    private float topY;
    private float downY;
    
    protected Rigidbody2D rb;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        leftY = transform.position.x - leftOffset;
        rightY = transform.position.x + rightOffset;
        
        topY = transform.position.y + upOffset;
        downY = transform.position.y - downOffset;
    }
    
    
    private void HorizontalMove() {
        if (rb.velocity.x == 0) {
            rb.velocity = new Vector2(-moveSpeed,0);
        }
        if (transform.position.x > rightY) {
            rb.velocity = new Vector2(-moveSpeed,0);
        }
        else if (transform.position.x < leftY) {
            rb.velocity = new Vector2(moveSpeed,0);
        }
        
    }
    
    private void VerticalMove() {
        if (rb.velocity.y == 0) {
            rb.velocity = new Vector2(0, moveSpeed);
        }
        if (transform.position.y > topY) {
            rb.velocity =  new Vector2(0, -moveSpeed);
        }
        else if (transform.position.y < downY) {
            rb.velocity =  new Vector2(0, moveSpeed);
        }
        
    }

    protected void Move() {
        switch (moveType) {
            case 1: HorizontalMove();break;
            case 2: VerticalMove();break;
            default: return;
        }
    }
    protected virtual void FixedUpdate() {
        Move();
    }
}
