using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour {
    [SerializeField] private float moveSpeed;
    
    [Header("Other")]
    [SerializeField] private Rigidbody rb;

    private Vector3 peacefulTarget;

    public Vector3 PeacefulTarget { get => peacefulTarget; set => peacefulTarget = value; }

    private void Awake() {
        if (peacefulTarget == Vector3.zero) peacefulTarget = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
    }

    private void FixedUpdate() {
        FlyPeacefully();
    }

    private void FlyPeacefully() {
        transform.LookAt(transform.position - peacefulTarget);
        
        rb.velocity = peacefulTarget * moveSpeed;

        //rb.MovePosition(transform.position + ((transform.position + peacefulTarget) * moveSpeed * Time.fixedDeltaTime));
    }
}
