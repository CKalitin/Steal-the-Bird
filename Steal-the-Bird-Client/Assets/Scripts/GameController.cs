using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static bool ApplicationQuitting = false;

    private void Awake() {
        Application.runInBackground = true;
        Debug.LogError("Opened Console.");
    }

    private void OnApplicationQuit() {
        ApplicationQuitting = true;
    }
}
