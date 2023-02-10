using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour {
    [Header("Attacking")]
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float rotationSpeed = 120f;

    [Header("References")]
    [SerializeField] private EnemyPathfinding enemyPathfinding;
    [SerializeField] private WeaponMelee meleeWeapon;
    [SerializeField] private USNL.SyncedObject syncedObject;

    Vector3 previousPos;
    bool attacking = false;

    private void Update() {
        RotateTowardsPlayer();

        bool previousAttacking = attacking;

        attacking = false;
        if (Vector3.Distance(transform.position, enemyPathfinding.Target) <= attackDistance) {
            if (enemyPathfinding.Target != null & enemyPathfinding.Target != Vector3.zero & enemyPathfinding.Target != transform.position) {
                attacking = true;
                meleeWeapon.Attack();
            }
        }

        if (previousAttacking != attacking) {
            if (attacking) USNL.PacketSend.EnemyAnimation(syncedObject.SyncedObjectUUID, 1);
            else USNL.PacketSend.EnemyAnimation(syncedObject.SyncedObjectUUID, 0);
        }
    }

    private void RotateTowardsPlayer() {
        if (enemyPathfinding.Target == transform.position) return;
        Vector3 targetDirection = (enemyPathfinding.Target - transform.position).normalized;
        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        targetRotation.x = 0;
        targetRotation.z = 0;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
