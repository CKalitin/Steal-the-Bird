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
    [Space]
    [SerializeField] private float flightHeight = 6f;

    private Quaternion startRotation; // At begining of turn for lerp
    private float rotationLerp = 0f;
    
    private int rollDirection = 1;

    private float swoopLerp = 0f;

    bool flyingToVector3 = false;

    int flyToTargetIndex = 0;

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

    private void Start() {
        FindNewMarker();
        FlyToMarker(markerTarget.position);
    }

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
                FlyToTarget(playerTarget.position);
            }
            
            CheckMarker();
        }
    }

    private void CheckMarker() {
        if (Vector3.Distance(transform.position, markerTarget.position) < 1f && flyingToVector3) FindNewMarker();
    }

    private void FindNewMarker() {
        markerTarget = BirdController.instance.BirdMarkers[Random.Range(0, BirdController.instance.BirdMarkers.Length)];
        FlyToMarker(markerTarget.position);
    }

    #endregion

    #region Flying

    private void Fly() {
        if (flyingToVector3) FlyTowardsMarker();
        else FlyTowardsTarget();
    }

    private void FlyToMarker(Vector3 _target) {
        flyingToVector3 = true;

        startRotation = transform.rotation;

        rotationLerp = 0f;

        rollDirection = 1;
        Vector3 targetDir = (markerTarget.position - transform.position).normalized;
        if (Vector3.Dot(targetDir, transform.right) < 0) rollDirection *= -1; // If target is to the left
        if (Vector3.Dot(targetDir, transform.forward) < 0) rollDirection *= -1; // If target is to the behind
    }

    private void FlyToTarget(Vector3 _target) {
        flyingToVector3 = false;

        startRotation = transform.rotation;

        rotationLerp = 0f;

        rollDirection = 1;
        Vector3 targetDir = (markerTarget.position - transform.position).normalized;
        if (Vector3.Dot(targetDir, transform.right) < 0) rollDirection *= -1; // If target is to the left
        if (Vector3.Dot(targetDir, transform.forward) < 0) rollDirection *= -1; // If target is to the behind
    }

    #endregion

    #region Flying Movement Functions

    private void FlyTowardsMarker() {
        rb.velocity = transform.forward * speed * Time.fixedDeltaTime;

        RotateTowardsPoint(markerTarget.position, ref rotationLerp);
    }

    private void FlyTowardsTarget() {
        RotateTowardsPoint(playerTarget.position, ref rotationLerp);

        if (flyToTargetIndex == 0) {
            rb.velocity = transform.forward * speed * Time.fixedDeltaTime;

            if (Vector3.Distance(transform.position, playerTarget.position) < playerAttackDistance) flyToTargetIndex = 1;
        } else if (flyToTargetIndex == 1) {

            if (Vector3.Distance(transform.position, playerTarget.position) < 0.5f) flyToTargetIndex = 2;
        } else if (flyToTargetIndex == 2) {

        } else if (flyToTargetIndex == 2) {

        }
    }

    private void RotateTowardsPoint(Vector3 _point, ref float _lerp) {
        float rollAmmout = 0f;

        Vector3 targetDir = (_point - transform.position).normalized;
        Quaternion _lookRotation = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Lerp(startRotation, _lookRotation, _lerp);

        rollAmmout = rollDirection * rollModifier * Mathf.Abs(Mathf.Abs(_lerp - 0.5f) - 0.5f);

        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, rollAmmout));

        _lerp = Mathf.Clamp(_lerp + (turnSpeed * Time.deltaTime), 0, 1);
    }

    private void SwoopDownToPoint() { 
    
    }

    private void SwoopUpFromPoint() {
        
    }

    #endregion
}
