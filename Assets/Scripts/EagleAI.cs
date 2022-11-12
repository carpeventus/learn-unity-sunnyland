using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleAI : Enemy {
    private float topY;
    private float downY;
    public float upOffset = 2f;
    public float downOffset = 2f;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        topY = transform.position.y + upOffset;
        downY = transform.position.y - downOffset;
    }

    protected override void Move() {
        if (rb.velocity.y == 0) {
            rb.velocity = new Vector2(0, moveSpeed);
        }
        if (transform.position.y > topY) {
            rb.velocity = new Vector2(0, -moveSpeed);
        }
        if (transform.position.y < downY) {
            rb.velocity = new Vector2(0, moveSpeed);
        }
    }
}