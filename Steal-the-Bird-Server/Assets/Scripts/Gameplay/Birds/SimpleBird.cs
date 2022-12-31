using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleBird : MonoBehaviour {
    #region Variables

    [Header("Flying")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float rollModifier;

    private Quaternion startRotation; // At begining of turn for lerp
    private float rotationLerp = 0f;

    private float rollAmmout = 0f;
    private int rollDirection = 1;

    [Header("Attack")]
    [SerializeField] private float playerAttackDistance = 7f;
    [SerializeField] private LayerMask playerLayerMask;

    [Header("Bird Management")]
    [Tooltip("How many updates per bird update")]
    [SerializeField] private int nthBirdUpdate = 3;

    [Header("Other")]
    [SerializeField] private Rigidbody rb;

    private Transform markerTarget;
    private Transform playerTarget;

    private int updateCount = 0;

    #endregion

    #region Core

    private void Update() {
        if (updateCount % nthBirdUpdate == 0) {
            BirdUpdate();
        }
        updateCount++;
    }

    private void FixedUpdate() {
        Fly();
    }

    #endregion

    #region Bird Update

    private void BirdUpdate() {
        if (playerTarget == null) {
            Collider[] players = Physics.OverlapSphere(transform.position, playerAttackDistance, playerLayerMask);

            if (players.Length > 0) {
                playerTarget = players.OrderBy(n => Vector3.Distance(transform.position, n.transform.position)).ToArray()[0].transform;
            }
            
            CheckMarker();
        }
    }

    private void CheckMarker() {
        if (markerTarget == null) FindNewMarker();
        if (Vector3.Distance(transform.position, markerTarget.position) < 1f) FindNewMarker();
    }

    private void FindNewMarker() {
        markerTarget = BirdController.instance.BirdMarkers[Random.Range(0, BirdController.instance.BirdMarkers.Length)];
        
        startRotation = transform.rotation;
        
        rotationLerp = 0f;
        
        rollDirection = 1;
        Vector3 targetDir = (markerTarget.position - transform.position).normalized;
        if (Vector3.Dot(targetDir, transform.right) < 0) rollDirection *= -1; // If target is to the left
        if (Vector3.Dot(targetDir, transform.forward) < 0) rollDirection *= -1; // If target is to the behind
    }

    #endregion

    #region Flying

    private void Fly() {
        if (playerTarget != null) {
            FlyTowardsMarker();
        } else {
            FlyTowardsMarker();
        }
    }

    private void FlyTowardsMarker() {
        rb.velocity = transform.forward * speed * Time.fixedDeltaTime;

        rollAmmout = 0f;
        if (rotationLerp < 1) {
            if (markerTarget == null) CheckMarker();

            Vector3 targetDir = (markerTarget.position - transform.position).normalized;
            Quaternion _lookRotation = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Lerp(startRotation, _lookRotation, rotationLerp);

            rollAmmout = rollDirection * rollModifier * Mathf.Abs(Mathf.Abs(rotationLerp - 0.5f) - 0.5f);

            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, rollAmmout));

            rotationLerp = Mathf.Clamp(rotationLerp + (turnSpeed * Time.deltaTime), 0, 1);
        }
    }

    #endregion
}
