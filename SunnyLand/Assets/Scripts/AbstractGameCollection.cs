using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractGameCollection : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip playerGot;
    protected AudioSource player;
    protected Animator anim;
    private bool got;
    
    protected virtual void Start() {
        player = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();

    }

    protected abstract void IncreaseCollectionNum(PlayerController playerController);

    protected void PlayerGot() {
        got = true;
        anim.SetTrigger("playerGot");
        player.PlayOneShot(playerGot);
    }
    
    private void Disappear() {
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D col) {
        if (col.tag.Equals("Player") && !got) {
            PlayerController playerController = col.gameObject.GetComponent<PlayerController>();
            IncreaseCollectionNum(playerController);
            PlayerGot();
        }
    }
}
