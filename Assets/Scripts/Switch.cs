using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Switch : OneWaySwitch
{
    protected override bool TriggerOnlyOnce() {
        return false;
    }
}
