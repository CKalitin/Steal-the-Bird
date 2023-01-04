using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBird : MonoBehaviour {
    [SerializeField] private float sinkSpeed = 0.09f;
    [SerializeField] private float fallSpeedDivider = 30f;
    [Space]
    [SerializeField] private float despawnTime = 60f;
    [Space]
    [SerializeField] private BirdAnimator birdAnimator;

    private int syncedObjectUUID;
    private Vector3 startPosition;

    private Vector3 landPosition;
    private bool landInWater;
    private float fallSpeed;

    private Quaternion startRotation;

    private float fallDistance = 0f;

    private bool foundDeadBirdInfo = false;

    private float lerp;

    private bool dead = false;
    private bool deathComplete = false;
    private bool hitWorld = false;

    private float totalDeltaTime;

    public int SyncedObjectUUID { get => syncedObjectUUID; set => syncedObjectUUID = value; }
    public Vector3 StartPosition { get => startPosition; set => startPosition = value; }
    public Quaternion StartRotation { get => startRotation; set => startRotation = value; }
    public bool Dead { get => dead; set => dead = value; }

    public void SetDead() {
        dead = true;
        birdAnimator.SetDead();
    }

    private void Update() {
        if (!dead) return;

        if (deathComplete) return;

        if (totalDeltaTime >= 1f && !foundDeadBirdInfo) Destroy(gameObject);
        if (totalDeltaTime >= despawnTime) Destroy(gameObject);
        
        LookForDeadBirdInfo();

        if (!hitWorld) FallFromSky();
        else if (landInWater) SinkIntoWater();

        totalDeltaTime += Time.deltaTime;
    }

    private void LookForDeadBirdInfo() {
        if (foundDeadBirdInfo) return;

        if (BirdsController.instance.DeadBirdsInfo.ContainsKey(syncedObjectUUID)) {
            DeadBirdInfo _deadBirdInfo = BirdsController.instance.DeadBirdsInfo[syncedObjectUUID];
            landPosition = _deadBirdInfo.LandPosition;
            landInWater = _deadBirdInfo.LandOnWater;
            fallSpeed = _deadBirdInfo.FallSpeed;
            fallDistance = Vector3.Distance(startPosition, landPosition);

            foundDeadBirdInfo = true;
        }
    }

    private void FallFromSky() {
        if (!foundDeadBirdInfo) return;
        
        lerp += (fallSpeed * Time.deltaTime) / fallDistance / fallSpeedDivider;

        transform.position = Vector3.Lerp(startPosition, landPosition, lerp);

        if (Vector3.Distance(transform.position, landPosition) <= 2f) {
            birdAnimator.SetFly(false);
            birdAnimator.SetLanded();
        }
        if (Vector3.Distance(transform.position, landPosition) <= 0.1f) {
            hitWorld = true;
            birdAnimator.SetExit();

            birdAnimator.Animator.enabled = false;
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        if (hitWorld & !landInWater) {
            deathComplete = true;
        }
    }

    private void SinkIntoWater() {
        if (!foundDeadBirdInfo) return;
        
        if (transform.position.y - landPosition.y < -2f) deathComplete = true;
        
        transform.position += Vector3.down * sinkSpeed * Time.deltaTime;
    }
}
