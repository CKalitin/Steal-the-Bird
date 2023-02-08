using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour {
    [Header("Animation Triggers")]
    [SerializeField] private float walkSpeed = 0.5f;
    [SerializeField] private float runSpeed = 2f;
    [Space]
    [SerializeField] private float attackTime;

    bool isAttacking;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private USNL.SyncedObject syncedObject;

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
    
    private void OnEnable() { USNL.CallbackEvents.OnEnemyAnimationPacket += OnEnemyAnimationPacket; }
    private void OnDisable() { USNL.CallbackEvents.OnEnemyAnimationPacket -= OnEnemyAnimationPacket; }

    private void UpdateAnimation() {
        Vector3 diff = transform.position - previousPos;
        posUpdateDeltaTime = Time.time - previousTime;

        if (isAttacking) {
            previousTime = Time.time;
            previousPos = transform.position;
            return;
        }
        
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(previousPos.x, previousPos.z)) > runSpeed * posUpdateDeltaTime) {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", true);
            animator.SetBool("isIdle", false);
        } else if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(previousPos.x, previousPos.z)) > walkSpeed * posUpdateDeltaTime) {
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", false);
        } else {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", true);
        }

        previousTime = Time.time;
        previousPos = transform.position;
    }

    private void OnEnemyAnimationPacket(object _packetObject) {
        USNL.EnemyAnimationPacket packet = (USNL.EnemyAnimationPacket)_packetObject;
        if (packet.SyncedObjectUUID != syncedObject.SyncedObjectUuid) return;

        if (packet.AnimationIndex == 0) {
            isAttacking = false;
        } else {
            isAttacking = true;

            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", false);

            StartCoroutine(AttackCoroutine());
        }
        Debug.Log(isAttacking);
    }

    private IEnumerator AttackCoroutine() {
        while (isAttacking) {
            animator.SetTrigger("isAttacking");
            yield return new WaitForSeconds(attackTime);
        }
    }
}
