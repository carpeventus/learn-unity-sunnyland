using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class OneWaySwitch : AbstractInteractive
{
    private AudioSource player;
    public AudioClip switched;
    public SwitchTriggerMove block;
    private Animator anim;
    public bool open;

    // Start is called before the first frame update
    
    protected override void Start() {
        base.Start();
        player = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    protected override void WhenTrigger() {
        var stateAfter = FlipSwitch();
        anim.SetBool("switch", stateAfter);
        player.PlayOneShot(switched);
        block.Trigger();
    }

    private bool FlipSwitch() {
        open = !open;
        return open;
    }

    protected override bool Trigger() {
        return Input.GetButtonDown("OpenSwitch");
    }

    protected override bool TriggerOnlyOnce() {
        return true;
    }
}
