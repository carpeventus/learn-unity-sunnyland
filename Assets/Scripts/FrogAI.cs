using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogAI : Enemy {
    
    private Collider2D collider2d;
    
    public LayerMask groundLayer;
    public bool onGround;
    
    public float jumpForce = 3f;
    
    private float moveLeftLimit;
    private float moveRightLimit;
    public float leftOffset;
    public float rightOffset;

    public bool faceLeft;
    
    private static string JUMP_ANIMATO_PARAM = "jumping";
    private static string FALL_ANIMATO_PARAM = "falling";
    // private static string IDLE_ANIMATO_PARAM = "idle";

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        collider2d = GetComponent<Collider2D>();
        faceLeft = true;
        moveLeftLimit = transform.position.x - leftOffset;
        moveRightLimit = transform.position.x + rightOffset;
    }

    // Update is called once per frame
    void Update() {
        onGround = collider2d.IsTouchingLayers(groundLayer);
    }

    protected override void FixedUpdate() {
        SwitchAnimation();
    }

    protected override void Move() {
        if (onGround) {
            rb.velocity = new Vector2(faceLeft ? -moveSpeed : moveSpeed, jumpForce);
        }
        
        if (faceLeft && transform.position.x < moveLeftLimit) {
            // 防止惯性，屁股往后顶
            rb.velocity = Vector2.zero;
            Flip();
        }
        if (!faceLeft && transform.position.x > moveRightLimit) {
            rb.velocity = Vector2.zero;
            Flip();
        }
    }

    void SwitchAnimation() {
        
        if (!onGround) {
            if (rb.velocity.y > 0) {
                PlayAnimation(JUMP_ANIMATO_PARAM);
            } else {
                StopAnimation(JUMP_ANIMATO_PARAM);
                PlayAnimation(FALL_ANIMATO_PARAM);
            }
        } else {
            if (animator.GetBool(JUMP_ANIMATO_PARAM)) {
                StopAnimation(JUMP_ANIMATO_PARAM);
                PlayAnimation(FALL_ANIMATO_PARAM);
            }
            StopAnimation(FALL_ANIMATO_PARAM);
        }
    }
    
    void StopAnimation(String param) {
        animator.SetBool(param, false);
    }

    void PlayAnimation(string param) {
        animator.SetBool(param, true);
    }

    void Flip() {
        faceLeft = !faceLeft;
        transform.rotation = Quaternion.Euler(0, faceLeft ? 0 : 180, 0);
    }


}