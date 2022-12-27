using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [HideInInspector] public Transform CameraTransform;

    private void OnDestroy() {
        if (CameraTransform) CameraTransform.parent = null;
    }
}
