using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour {
    [Tooltip("Damage is dealt to these tags.")]
    [SerializeField] private string[] targetTags;
    
    private int clientDamagerId = -1;

    public int ClientDamagerId { get => clientDamagerId; set => clientDamagerId = value; }

    public void DealDamage(Health[] healthComponents, float _healthChange) {
        for (int i = 0; i < healthComponents.Length; i++) {
            if (CheckObjectTag(healthComponents[i].tag))
                healthComponents[i].ChangeHealth(_healthChange, clientDamagerId);
        }
    }

    public void DealDamage(Collider[] healthComponents, float _healthChange) {
        for (int i = 0; i < healthComponents.Length; i++) {
            Health health = healthComponents[i].GetComponent<Health>();
            if (health == null) continue;
            if (CheckObjectTag(healthComponents[i].tag))
                health.ChangeHealth(_healthChange, clientDamagerId);
        }
    }

    public bool CheckObjectTag(string _tag) {
        for (int i = 0; i < targetTags.Length; i++) {
            if (_tag == targetTags[i]) return true;
        }
        return false;
    }
}
