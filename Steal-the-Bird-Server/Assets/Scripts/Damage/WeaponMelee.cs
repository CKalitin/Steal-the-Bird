using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMelee : MonoBehaviour {
    [Header("Weapon")]
    [SerializeField] private float rateOfAttack;
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float dealDamageDelay;
    [Tooltip("Get all health components in sphere of radius attack range from this point.")]
    [SerializeField] private Transform attackPosition;

    [Header("Other")]
    [SerializeField] private Damager damager;
    [Tooltip("Uses 'Attack' trigger")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool useAnimations = false;

    bool coolingDownAttack = false;

    public void Attack() {
        if (!coolingDownAttack) {
            coolingDownAttack = true;

            if (animator && useAnimations) animator.SetTrigger("Attack");
            
            Invoke(nameof(DealDamage), dealDamageDelay);
            Invoke(nameof(ResetCanShoot), rateOfAttack);
        }
    }

    private void DealDamage() {
        damager.DealDamage(Physics.OverlapSphere(attackPosition.position, attackRange), -Mathf.Abs(attackDamage));
    }

    private void ResetCanShoot() {
        coolingDownAttack = false;
    }
}
