using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    [SerializeField] private Animator animator;

    [Header("Animation Triggers")]
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float fallSpeed = 1f;

    private float posUpdateDeltaTime = 0;
    private float previousTime;

    private Vector3 previousPos;

    private void Start() {
        previousTime = Time.time;
        previousPos = transform.position;
    }

    private void Update() {
        Vector3 diff = transform.position - previousPos;
        if (diff.magnitude > 0) UpdateAnimation();
        previousPos = transform.position;
    }
    
    private void UpdateAnimation() {
        Vector3 diff = transform.position - previousPos;
        posUpdateDeltaTime = Time.time - previousTime;

        if (diff.y < -fallSpeed * posUpdateDeltaTime) {
            animator.SetBool("isWalking", false);
            animator.SetBool("IsFalling", true);
        } else if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(previousPos.x, previousPos.z)) > walkSpeed * posUpdateDeltaTime) {
            animator.SetBool("isWalking", true);
            animator.SetBool("IsFalling", false);
        } else {
            animator.SetBool("isWalking", false);
            animator.SetBool("IsFalling", false);
        }
        
        previousTime = Time.time;
        previousPos = transform.position;
    }
}
