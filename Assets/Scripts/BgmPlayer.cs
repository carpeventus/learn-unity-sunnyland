using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource player;
    public AudioClip bgm;
    void Start() {
        player = GetComponent<AudioSource>();
        player.clip = bgm;
        player.loop = true;
        player.volume = 0.3f;
        player.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
