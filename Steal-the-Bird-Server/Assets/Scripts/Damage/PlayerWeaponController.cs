using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour {
    [Header("Weapons")]
    [SerializeField] private GameObject weaponGameObject;
    [SerializeField] private Transform weaponParent;
    
    [Header("Other")]
    [SerializeField] private PlayerController playerController;
    
    private int clientId;

    USNL.ClientInput clientInput;

    public int ClientId { get => clientId; set => clientId = value; }

    private void Start() {
        clientInput = USNL.InputManager.instance.GetClientInput(clientId);

        PlayerRaycastManager.instance.Pwcs.Add(clientId, this);
    }

    private void Update() {
        if (clientInput.GetKey(KeyCode.Mouse0)) ShootActiveWeapon();
    }

    private void ShootActiveWeapon() {
        weaponGameObject.GetComponent<WeaponRanged>().Shoot();
    }

    public void AimWeapon(Vector3 _lookAt) {
        weaponParent.LookAt(_lookAt);
    }

    private void OnDestroy() {
        PlayerRaycastManager.instance.Pwcs.Remove(clientId);
    }
}
