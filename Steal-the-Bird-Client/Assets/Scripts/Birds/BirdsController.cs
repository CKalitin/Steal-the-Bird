using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DeadBirdInfo {
    private Vector3 landPosition;
    private bool landOnWater;
    private float fallSpeed;

    public Vector3 LandPosition { get => landPosition; set => landPosition = value; }
    public bool LandOnWater { get => landOnWater; set => landOnWater = value; }
    public float FallSpeed { get => fallSpeed; set => fallSpeed = value; }
}

public class BirdsController : MonoBehaviour {
    public static BirdsController instance;

    private Dictionary<int, DeadBirdInfo> deadBirdsInfo = new Dictionary<int, DeadBirdInfo>();

    public Dictionary<int, DeadBirdInfo> DeadBirdsInfo { get => deadBirdsInfo; set => deadBirdsInfo = value; }

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.Log($"Match Manager instance already exists on ({gameObject}), destroying this.");
            Destroy(this);
        }
    }

    private void OnEnable() {
        USNL.CallbackEvents.OnBirdDeathPacket += OnBirdDeathPacket;
    }

    private void OnDisable() {
        USNL.CallbackEvents.OnBirdDeathPacket -= OnBirdDeathPacket;
    }

    private void OnBirdDeathPacket(object _packetObject) {
        USNL.BirdDeathPacket _packet = (USNL.BirdDeathPacket)_packetObject;

        DeadBirdInfo _deadBirdInfo = new DeadBirdInfo();
        _deadBirdInfo.LandPosition = _packet.LandPosition;
        _deadBirdInfo.LandOnWater = _packet.LandOnWater;
        _deadBirdInfo.FallSpeed = _packet.FallSpeed;

        deadBirdsInfo.Add(_packet.SyncedObjectUUID, _deadBirdInfo);

        StartCoroutine(RemoveDeadBirdInfo(_packet.SyncedObjectUUID));
    }

    private IEnumerator RemoveDeadBirdInfo(int _syncedObjectUUID) {
        yield return new WaitForSeconds(5f);
        deadBirdsInfo.Remove(_syncedObjectUUID);
    }
}
