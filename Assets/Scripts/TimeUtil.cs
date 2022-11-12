using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUtil
{
    public static void GamePause() {
        Time.timeScale = 0f;
    }

    public static bool IsGamePause() {
        return Time.timeScale == 0f;
    }
    
    public static void GamePlay() {
        Time.timeScale = 1f;
    }
}
