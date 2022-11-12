using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Cherry : AbstractGameCollection {
    protected override void IncreaseCollectionNum(PlayerController playerController) {
        playerController.IncreaseCherryNum();
    }
}