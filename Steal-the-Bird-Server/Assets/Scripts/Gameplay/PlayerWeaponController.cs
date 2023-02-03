using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour {
    #region Variables
    
    [Header("Weapon Switching")]
    [SerializeField] private int currentWeaponIndex = 0;
    [SerializeField] private WeaponStruct[] weapons;

    float currentMouseScrollDelta;
    
    private int clientId;

    USNL.ClientInput clientInput;

    public int ClientId { get => clientId; set => clientId = value; }
    public int CurrentWeaponIndex { get => currentWeaponIndex; }
    public WeaponStruct[] Weapons { get => weapons; }

    [Serializable]
    public struct WeaponStruct {
        public GameObject weapon;
        public string weaponTag;
        public bool available;
    }

    #endregion
    
    #region Core

    private void Start() {
        clientInput = USNL.InputManager.instance.GetClientInput(clientId);

        PlayerRaycastManager.instance.Pwcs.Add(clientId, this);
        
        SetWeapon(0);
    }

    private void Update() {
        ControlWeaponChanging();
        Attack();
        
        currentMouseScrollDelta = 0;
    }

    private void OnEnable() { USNL.CallbackEvents.OnMouseScrollDeltaPacket += OnMouseScrollDeltaPacket; }
    private void OnDisable() { USNL.CallbackEvents.OnMouseScrollDeltaPacket -= OnMouseScrollDeltaPacket; }

    #endregion

    #region Using Weapons

    private void Attack() {
        if (clientInput.GetKey(KeyCode.Mouse0) && weapons[currentWeaponIndex].available) {
            if (weapons[currentWeaponIndex].weapon.GetComponent<WeaponMelee>())
                weapons[currentWeaponIndex].weapon.GetComponent<WeaponMelee>().Attack();
            else if (weapons[currentWeaponIndex].weapon.GetComponent<WeaponRanged>())
                weapons[currentWeaponIndex].weapon.GetComponent<WeaponRanged>().Shoot();
        }
    }

    public void AimWeapon(Vector3 _lookAt) {
        for (int i = 0; i < weapons.Length; i++) {
            if (weapons[i].weapon.GetComponent<WeaponRanged>())
                weapons[i].weapon.GetComponent<WeaponRanged>().transform.LookAt(_lookAt);
        }
    }

    #endregion

    #region Switching Weapons

    private void ControlWeaponChanging() {
        if (Mathf.Abs(currentMouseScrollDelta) >= 1) {
            if (currentMouseScrollDelta > 0) ChangeWeapon(1);
            if (currentMouseScrollDelta < 0) ChangeWeapon(-1);
            currentMouseScrollDelta = 0;
        }

        for (int i = 0; i < weapons.Length; i++) {
            if (Input.GetKeyDown((KeyCode)(i + 49))) SetWeapon(i);
        }
    }

    // Holy shit Github Copilot wrote most of this
    private void ChangeWeapon(int _input) {
        weapons[currentWeaponIndex].weapon.SetActive(false);

        bool newWeaponSelected = false;
        int iters = 0;
        while (!newWeaponSelected) {
            currentWeaponIndex += _input;

            if (currentWeaponIndex > weapons.Length - 1) currentWeaponIndex = 0;
            if (currentWeaponIndex < 0) currentWeaponIndex = weapons.Length - 1;

            if (weapons[currentWeaponIndex].available) {
                weapons[currentWeaponIndex].weapon.SetActive(true);
                newWeaponSelected = true;
                break;
            }

            iters++;
            if (iters > weapons.Length) {
                break;
            }
        }
    }

    public void SetWeapon(int _weaponIndex) {
        if (weapons[_weaponIndex].available) {
            weapons[currentWeaponIndex].weapon.SetActive(false);
            currentWeaponIndex = _weaponIndex;
            weapons[currentWeaponIndex].weapon.SetActive(true);
        }
    }

    #endregion

    #region Other

    private void OnDestroy() {
        PlayerRaycastManager.instance.Pwcs.Remove(clientId);
    }

    #endregion

    #region Callbacks

    private void OnMouseScrollDeltaPacket(object _packetObject) {
        USNL.MouseScrollDeltaPacket packet = (USNL.MouseScrollDeltaPacket)_packetObject;
        if (packet.FromClient == clientId) {
            currentMouseScrollDelta += packet.Y;
        }
    }

    #endregion
}
