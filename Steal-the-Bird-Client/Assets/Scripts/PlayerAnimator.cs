using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    #region Variables

    [Header("Animation Triggers")]
    [SerializeField] private float walkSpeed = 1f;

    [Header("Aim")]
    [SerializeField] private Transform weapon;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private USNL.SyncedObject syncedObject;

    private float posUpdateDeltaTime = 0;
    private float previousTime;

    private Vector3 previousPos;

    #endregion

    #region Core

    private void Start() {
        previousTime = Time.time;
        previousPos = transform.position;
        
        animator.SetBool("Idle", true);
    }

    private void Update() {
        Vector3 diff = transform.position - previousPos;
        if (diff.magnitude > 0) UpdateAnimation();
        previousPos = transform.position;
    }

    private void OnEnable() {
        USNL.CallbackEvents.OnPlayerAnimationPacket += OnPlayerAnimationPacket;
        USNL.CallbackEvents.OnPlayerAimPacket += OnPlayerAimPacket;
    }

    private void OnDisable() {
        USNL.CallbackEvents.OnPlayerAnimationPacket -= OnPlayerAnimationPacket;
        USNL.CallbackEvents.OnPlayerAimPacket -= OnPlayerAimPacket;
    }

    private void OnDestroy() {
        if (GameController.ApplicationQuitting) return; // If application is quitting, don't run code

        transform.parent = null;
        animator.SetBool("Dead", true);
    }

    #endregion

    #region Animation

    private void UpdateAnimation() {
        Vector3 diff = transform.position - previousPos;
        posUpdateDeltaTime = Time.time - previousTime;

        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(previousPos.x, previousPos.z)) > walkSpeed * posUpdateDeltaTime) {
            animator.SetBool("Walk", true);
            animator.SetBool("Idle", false);
        } else {
            animator.SetBool("Walk", false);
            animator.SetBool("Idle", true);
        }
        
        previousTime = Time.time;
        previousPos = transform.position;
    }

    private void OnPlayerAnimationPacket(object _packetObject) {
        USNL.PlayerAnimationPacket packet = (USNL.PlayerAnimationPacket)_packetObject;

        if (packet.SyncedObjectUUID != syncedObject.SyncedObjectUuid) return;
        
        if (packet.AnimationIndex == 0) {
            animator.SetTrigger("Attack");
        }
    }

    private void OnPlayerAimPacket(object _packetObject) {
        USNL.PlayerAimPacket packet = (USNL.PlayerAimPacket)_packetObject;
        
        if (packet.SyncedObjectUUID != syncedObject.SyncedObjectUuid) return;

        weapon.LookAt(packet.Target);
    }

    #endregion
}
