using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RangedWeaponAmmoTypes {
    Handgun,
    SemiAuto
}

public class WeaponRanged : MonoBehaviour {
    [Header("Weapon")]
    [SerializeField] private float rateOfFire;
    [SerializeField] private float shotDamage;
    [SerializeField] private float shotRange;
    [SerializeField] private Transform shootPosition;

    [Header("Ammo")]
    [Tooltip("This is used by the Player Pickup Collector")]
    [SerializeField] private RangedWeaponAmmoTypes ammoType;
    [SerializeField] private int ammoAmount = 100;
    [SerializeField] private int maxAmmo = 100;
    [SerializeField] private bool useAmmo = false;

    [Header("Other")]
    [SerializeField] private Damager damager;
    [Tooltip("Uses 'Attack' trigger in Animation Controller")]
    [SerializeField] private Animator animator;
    [Space]
    [SerializeField] private PlayerController playerController;

    private bool coolingDownShot = false;

    public float ShotRange { get => shotRange; set => shotRange = value; }
    public Transform ShootPosition { get => shootPosition; set => shootPosition = value; }
    public RangedWeaponAmmoTypes AmmoType { get => ammoType; set => ammoType = value; }
    public int AmmoAmount { get => ammoAmount; set => ammoAmount = value; }
    public int MaxAmmo { get => maxAmmo; set => maxAmmo = value; }

    private void Start() {
        if (playerController != null) damager.ClientDamagerId = playerController.ClientId;
    }

    public void Shoot() {
        if (coolingDownShot) return;
        if (useAmmo && ammoAmount <= 0) return;

        RaycastHit hit;
        Debug.DrawRay(shootPosition.position, shootPosition.forward * 15, Color.red, 2f);
        if (Physics.Raycast(shootPosition.position, shootPosition.forward, out hit, shotRange)) {
            damager.DealDamage(new Collider[1] { hit.collider }, -Mathf.Abs(shotDamage));
        }
        
        ammoAmount -= 1;

        if (animator) animator.SetTrigger("Attack");
        
        coolingDownShot = true;
        StartCoroutine(ResetShotCooldown());
    }

    private IEnumerator ResetShotCooldown() {
        yield return new WaitForSeconds(rateOfFire);
        coolingDownShot = false;
    }
}
