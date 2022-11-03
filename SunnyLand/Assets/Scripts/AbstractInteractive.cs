using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractInteractive : MonoBehaviour {
    // 控制是否只能交互一次
    private bool canInteractive;
    // 当前是否在碰撞（触发）
    private bool collision;
    // 按下按键且在触发中，那么可以进行按键后的动作
    private bool trigger;

    protected virtual void Start() {
        canInteractive = true;
    }
    
    protected void Update() {
        if (Trigger() && collision) {
            trigger = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        collision = true;
        if (canInteractive && trigger) {
            if (TriggerOnlyOnce()) {
                canInteractive = false;
            }
            WhenTrigger();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        collision = false;
        if (!TriggerOnlyOnce()) {
            canInteractive = true;
        }
    }

    protected abstract void WhenTrigger();

    protected abstract bool Trigger();

    protected abstract bool TriggerOnlyOnce();
}
