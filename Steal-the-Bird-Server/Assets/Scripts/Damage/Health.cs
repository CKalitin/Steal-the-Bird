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
    
    public void ChangeHealth(float _damage, int _damagerClientId) {
        currentHealth += _damage;
        
        if (currentHealth <= 0 & destroyOnZeroHealth) Destroy(gameObject);

        HandlePlayerInfo(_damage, _damagerClientId);
    }

    private void HandlePlayerInfo(float _damage, int _damagerClientId) {
        bool updateDamagerInfo = false;

        if (_damagerClientId >= 0) {
            PlayerInfoManager.instance.PlayerInfos[_damagerClientId].DamageDealt += Mathf.Abs(_damage);
            updateDamagerInfo = true;
        }
        
        if (GetComponent<PlayerController>() != null) {
            PlayerController pc = GetComponent<PlayerController>();

            PlayerInfoManager.instance.PlayerInfos[pc.ClientId].DamageTaken += Mathf.Abs(_damage);

            if (currentHealth <= 0 & _damagerClientId >= 0) {
                PlayerInfoManager.instance.PlayerInfos[pc.ClientId].PlayerDeaths += 1;
                PlayerInfoManager.instance.PlayerInfos[_damagerClientId].PlayerKills += 1;
                updateDamagerInfo = true;
            } else if (currentHealth <= 0) {
                PlayerInfoManager.instance.PlayerInfos[pc.ClientId].EnemyDeaths += 1;
            }

            PlayerInfoManager.instance.SendPlayerInfo(pc.ClientId);
        } else {
            if (currentHealth <= 0 & destroyOnZeroHealth & _damagerClientId >= 0) {
                PlayerInfoManager.instance.PlayerInfos[_damagerClientId].EnemyKills += 1;
                updateDamagerInfo = true;
            }
        }

        if (updateDamagerInfo) PlayerInfoManager.instance.SendPlayerInfo(_damagerClientId);
    }
}
