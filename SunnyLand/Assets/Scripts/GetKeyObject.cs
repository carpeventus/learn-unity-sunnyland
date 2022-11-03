using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetKeyObject : MonoBehaviour {

    public PlayerController playerController;
    public GameObject keyObjectHint;
    private AudioSource audioPlayer;
    public AudioClip clip;
    // 1 二段跳 2 蓄力跳
    public int abilityType;
    private bool trigger;
    
    // Start is called before the first frame update
    void Start() {
        audioPlayer = GetComponent<AudioSource>();
    }
    void Update() {
        var pressed = Input.GetButtonDown("Fire");
        if (trigger && pressed) {
            TimeUtil.GamePlay();
            keyObjectHint.SetActive(false);
            trigger = false;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.tag.Equals("Player")) {
            GetComponent<Collider2D>().enabled = false;
            GetAbility(abilityType);
            audioPlayer.PlayOneShot(clip);
            keyObjectHint.SetActive(true);
            trigger = true;
            TimeUtil.GamePause();
        }
    }

    void GetAbility(int abilityType) {
        switch (abilityType) {
            case 1:
                playerController.ownDoubleJumpAbility = true;
                break;
            case 2:
                playerController.ownChargeJumpAbility = true;
                break;
            default: return;
        }
    }
}
