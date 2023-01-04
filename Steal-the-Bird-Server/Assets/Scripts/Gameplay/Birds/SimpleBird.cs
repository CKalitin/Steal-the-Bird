using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleBird : MonoBehaviour {
    #region Variables

    [Header("Flying")]
    [SerializeField] private float flySpeed = 250f;
    [SerializeField] private float turnSpeed = 0.8f;
    [SerializeField] private float rollModifier = 50f;
    [Space]
    [SerializeField] private float flightHeight = 7f;
    [Space]
    [SerializeField] private float swoopSpeed = 0.5f;
    [SerializeField] private float swoopAngle = 50f;

    private Quaternion startRotation; // At begining of turn for lerp
    private float rotationLerp = 0f;
    private int rollDirection = 1;

    private float swoopLerp = 0f;

    bool flyingToVector3 = false;

    int flyToTargetIndex = 0;

    [Header("Attack")]
    [SerializeField] private float damage;
    [Space]
    [SerializeField] private float playerAttackDistance = 7f;
    [SerializeField] private LayerMask playerLayerMask;

    [Header("Bird Management")]
    [Tooltip("How many updates per bird update")]
    [SerializeField] private int nthBirdUpdate = 3;
    [Space]
    [SerializeField] private LayerMask worldLayerMask;
    [SerializeField] private float birdDeathForwardSpeed;

    [Header("Other")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Health health;
    [SerializeField] private USNL.SyncedObject syncedObject;

    private float previousCurrentHealth;

    private Transform markerTarget;
    private Transform playerTarget;

    private int updateCount = 0;

    #endregion

    #region Core

    private void Start() {
        FindNewMarker();
        FlyToMarker(markerTarget.position);

        previousCurrentHealth = health.CurrentHealth;
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
        if (flyingToVector3) {
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
        markerTarget = BirdsController.instance.BirdMarkers[Random.Range(0, BirdsController.instance.BirdMarkers.Length)];
        FlyToMarker(markerTarget.position);
    }

    private void CheckHealthChanged() {
        if (previousCurrentHealth != health.CurrentHealth) {
            previousCurrentHealth = health.CurrentHealth;
            if (!flyingToVector3) flyToTargetIndex = 2; // If attacking player, stop
        }
    }

    #endregion

    #region Flying

    private void Fly() {
        if (flyingToVector3) FlyTowardsMarker();
        else FlyTowardsTarget();

        CheckHealthChanged();
    }

    private void FlyToMarker(Vector3 _target) {
        flyingToVector3 = true;

        startRotation = transform.rotation;

        rotationLerp = 0f;

        rollDirection = 1;
        Vector3 targetDir = (_target - transform.position).normalized;
        if (Vector3.Dot(targetDir, transform.right) < 0) rollDirection *= -1; // If target is to the left
        if (Vector3.Dot(targetDir, transform.forward) < 0) rollDirection *= -1; // If target is to the behind
    }

    private void FlyToTarget(Vector3 _target) {
        flyingToVector3 = false;

        startRotation = transform.rotation;

        rotationLerp = 0f;

        rollDirection = 1;
        Vector3 targetDir = (_target - transform.position).normalized;
        if (Vector3.Dot(targetDir, transform.right) < 0) rollDirection *= -1; // If target is to the left
        if (Vector3.Dot(targetDir, transform.forward) < 0) rollDirection *= -1; // If target is to the behind
    }

    #endregion

    #region Flying Movement Functions

    private void FlyTowardsMarker() {
        rb.velocity = transform.forward * flySpeed * Time.fixedDeltaTime;

        RotateTowardsPoint(markerTarget.position, ref rotationLerp);
    }

    private void FlyTowardsTarget() {
        if (playerTarget == null) flyToTargetIndex = 2;

        if (flyToTargetIndex == 0) {
            RotateTowardsPoint(playerTarget.position, ref rotationLerp);

            rb.velocity = transform.forward * flySpeed * Time.fixedDeltaTime;

            if (Vector3.Distance(transform.position, playerTarget.position) < playerAttackDistance) {
                flyToTargetIndex = 1;
                swoopLerp = 0f;
            }
        } else if (flyToTargetIndex == 1) {
            RotateTowardsPoint(playerTarget.position, ref rotationLerp);

            rb.velocity = transform.forward * flySpeed * Time.fixedDeltaTime;
            
            SwoopDown(swoopLerp);
            swoopLerp = Mathf.Clamp(swoopLerp + (swoopSpeed * Time.deltaTime), 0, 1);

            if (Vector3.Distance(transform.position, playerTarget.position) < 0.5f) {
                playerTarget.GetComponent<Health>().ChangeHealth(-Mathf.Abs(damage));
                swoopLerp = 0f;
                flyToTargetIndex = 2;
            }
        } else if (flyToTargetIndex == 2) {
            rb.velocity = transform.forward * flySpeed * Time.fixedDeltaTime;

            SwoopUp(swoopLerp);

            if (transform.position.y > flightHeight - 1 || swoopLerp <= 0.5f)
                swoopLerp = Mathf.Clamp(swoopLerp + (swoopSpeed * Time.deltaTime), 0, 1);

            if (swoopLerp >= 1) {
                flyToTargetIndex = 0;
                FindNewMarker();
            }
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

    private void SwoopDown(float _lerp) {
        float swoopAmount = (swoopAngle * 2) * Mathf.Abs(Mathf.Abs(_lerp - 0.5f) - 0.5f);
        
        transform.rotation = Quaternion.Euler(new Vector3(swoopAmount, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
    }

    private void SwoopUp(float _lerp) {
        float swoopAmount = -((swoopAngle * 2) * Mathf.Abs(Mathf.Abs(_lerp - 0.5f) - 0.5f));

        transform.rotation = Quaternion.Euler(new Vector3(swoopAmount, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
    }

    #endregion

    #region Other

    private void OnDestroy() {
        RaycastHit hit;

        float height = transform.position.y;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f, worldLayerMask)) {
            height = Vector3.Distance(hit.point, transform.position);
        }

        Vector3 raycastPoint = transform.position + (transform.forward * birdDeathForwardSpeed * height);
        raycastPoint.y += 100f;

        if (Physics.Raycast(raycastPoint, Vector3.down, out hit, 500f, worldLayerMask)) {
            if (Physics.Raycast(transform.position, hit.point - transform.position, out hit, 500f, worldLayerMask)) {
                USNL.PacketSend.BirdDeath(syncedObject.SyncedObjectUUID, hit.point, hit.transform.gameObject.tag == "Water", flySpeed);
            }
        }
    }

    #endregion
}
