using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [Space]
    [SerializeField] private bool destroyOnZeroHealth;

    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }

    private void Awake() {
        //currentHealth = maxHealth;
    }
    
    public void ChangeHealth(float _damage) {
        currentHealth += _damage;
        if (currentHealth <= 0)
            if (destroyOnZeroHealth) Destroy(gameObject);
    }

    public void ChangeHealth(float _damage, int _damagerClientId) {
        currentHealth += _damage;
        
        if (currentHealth <= 0 & destroyOnZeroHealth) Destroy(gameObject);

        HandlePlayerInfo(_damage, _damagerClientId);
    }

    private void HandlePlayerInfo(float _damage, int _damagerClientId) {
        if (_damagerClientId >= 0) {
            PlayerInfoManager.instance.PlayerInfos[_damagerClientId].DamageDealt += _damage;
        }

        if (GetComponent<PlayerController>() != null) {
            PlayerController pc = GetComponent<PlayerController>();

            PlayerInfoManager.instance.PlayerInfos[pc.ClientId].DamageTaken += _damage;

            if (currentHealth <= 0 & _damagerClientId >= 0) {
                PlayerInfoManager.instance.PlayerInfos[pc.ClientId].PlayerDeaths += 1;
                PlayerInfoManager.instance.PlayerInfos[_damagerClientId].PlayerKills += 1;
            } else if (currentHealth <= 0) {
                PlayerInfoManager.instance.PlayerInfos[pc.ClientId].EnemyDeaths += 1;
            }
        } else {
            if (currentHealth <= 0 & destroyOnZeroHealth & _damagerClientId >= 0) PlayerInfoManager.instance.PlayerInfos[_damagerClientId].EnemyKills += 1;
        }
    }
}
