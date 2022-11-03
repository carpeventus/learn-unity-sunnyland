using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTriggerMove : MoveObject {
    private AudioSource player;
    public AudioClip clip;
    private bool triggerBySwitch;

   protected override void Start() {
        base.Start();
        player = GetComponent<AudioSource>();
    }
    
    protected override void FixedUpdate() {
        if (triggerBySwitch) {
            Move();
        }
    }

    public void Trigger() {
        player.PlayOneShot(clip);
        triggerBySwitch = true;
    }

}


