using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Block : MonoBehaviour {
    private Rigidbody2D rb;

    public GameObject hitDialog;
    private Vector2 originalPoint;
    public float jumpForce = 4f;
    public float fallGravity = 8f;
    
    public AudioClip hintClip;
    private AudioSource player;
    private bool trigger;
    
    // Start is called before the first frame update
    void Start() {
        hitDialog.SetActive(false);
        originalPoint = gameObject.transform.position;
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        var pressed = Input.GetButtonDown("Fire");
        if (trigger && pressed) {
            TimeUtil.GamePlay();
            HintDisappear();
            trigger = false;
        }
    }

    private void FixedUpdate() {
        ResetPosition();
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.collider.tag.Equals("Player") && col.transform.position.y < transform.position.y) {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            PlayHintSound();
            PopHint();
            trigger = true;
            TimeUtil.GamePause();
        }
    }

    private void ResetPosition() {
        // 上升
        if (rb.velocity.y > 0) {
            rb.gravityScale = fallGravity / 2;
            // 下降
        } else if (rb.velocity.y < 0) {
            rb.gravityScale = fallGravity;
            if (transform.position.y < originalPoint.y) {
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0f;
                transform.position = originalPoint;
            }
        }
  
    }

    private void PlayHintSound() {
        player.PlayOneShot(hintClip);
    }
    

    private void PopHint() {
        hitDialog.SetActive(true);
    }
    
    private void HintDisappear() {
        hitDialog.SetActive(false);
    }
}
