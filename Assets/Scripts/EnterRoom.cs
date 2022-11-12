using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterRoom : AbstractInteractive {


    private AudioSource player;
    public AudioClip doorOpen;


    // Start is called before the first frame update
    
    protected override void Start()
    {
        base.Start();
        player = GetComponent<AudioSource>();
    }



    protected override void WhenTrigger() {
        player.PlayOneShot(doorOpen);
        Invoke("toNextLevel", doorOpen.length);
    }

    protected override bool Trigger() {
        return Input.GetButtonDown("OpenSwitch");
    }

    private void toNextLevel() {
        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex + 1);
    }

    protected override bool TriggerOnlyOnce() {
        return true;
    }
}
