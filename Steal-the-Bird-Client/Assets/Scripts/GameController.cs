using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static bool ApplicationQuitting = false;

    private void Awake() {
        Application.runInBackground = true;
        Debug.LogError("Opened Console.");
    }

    private void Update() {
        if (Input.mouseScrollDelta.y != 0) USNL.PacketSend.MouseScrollDelta(Input.mouseScrollDelta.y);
    }

    private void OnApplicationQuit() {
        ApplicationQuitting = true;
    }
}
