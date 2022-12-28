using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour {
    [Tooltip("Damage is dealt to these tags.")]
    [SerializeField] private string[] targetTags;
    
    public void DealDamage(Health[] healthComponents, float _healthChange) {
        for (int i = 0; i < healthComponents.Length; i++) {
            if (CheckObjectTag(healthComponents[i].tag))
                healthComponents[i].ChangeHealth(_healthChange);
        }
    }

    public void DealDamage(Collider[] healthComponents, float _healthChange) {
        for (int i = 0; i < healthComponents.Length; i++) {
            Health health = healthComponents[i].GetComponent<Health>();
            if (health == null) continue;
            if (CheckObjectTag(healthComponents[i].tag))
                health.ChangeHealth(_healthChange);
        }
    }

    public bool CheckObjectTag(string _tag) {
        for (int i = 0; i < targetTags.Length; i++) {
            if (_tag == targetTags[i]) return true;
        }
        return false;
    }
}
