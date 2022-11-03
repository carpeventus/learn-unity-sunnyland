using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Gem : AbstractGameCollection {
    protected override void IncreaseCollectionNum(PlayerController playerController) {
        playerController.IncreaseGemNum();
    }
}       