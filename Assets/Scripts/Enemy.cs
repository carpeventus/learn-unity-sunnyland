using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour {
    protected Animator animator;
    protected AudioSource player;
    protected Rigidbody2D rb;
    
    public AudioClip deathClip;
    public float moveSpeed;
    private bool death;
    

    // Start is called before the first frame update
    protected virtual void Start() {
        animator = GetComponent<Animator>();
        player = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Death() {
        Destroy(gameObject);
    }

    private void PlayDeathAnimation() {
        death = true;
        player.PlayOneShot(deathClip);
        animator.SetTrigger("death");
    }
    private void OnCollisionEnter2D(Collision2D col) {
        if (!col.collider.tag.Equals("Player") || death) {
            return;
        }
        PlayerController playerController = col.gameObject.GetComponent<PlayerController>();
        if (playerController.CanHitEnemy(transform.position.y)) {
            PlayDeathAnimation();
            // 踩到怪的跳不算二段跳
            playerController.IncreaseJumpCount();
            if (Input.GetButton("Jump")) {
                playerController.Jump(playerController.jumpForce * 3 / 4, false);
            }
            else {
                playerController.Jump(playerController.jumpForce * 2 / 3, false);
            }
            playerController.IncreaseScore();
        }
        else {
            // 玩家在左边
            if (col.transform.position.x <= transform.position.x) {
                playerController.Hurted(false);
            }else {
                playerController.Hurted(true);
            }
        }
    }
    
        
    private void OnTriggerEnter2D(Collider2D col) {
        if (col.tag.Equals("DeadLine")) {
            Destroy(gameObject);
        }
    }

    protected virtual void FixedUpdate() {
        Move();
    }

    protected virtual void Move() {
        rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
    }
    

}