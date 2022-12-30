using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour {
    public static BirdController instance;

    [Tooltip("Birds fly to these")]
    [SerializeField] private Transform[] birdMarkers;

    public Transform[] BirdMarkers { get => birdMarkers; set => birdMarkers = value; }

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.Log($"Match Manager instance already exists on ({gameObject}), destroying this.");
            Destroy(this);
        }
    }
}
