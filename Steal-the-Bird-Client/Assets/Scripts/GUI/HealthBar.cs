using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    [SerializeField] private Image healthBarImage;

    private int clientId = -1;

    private void OnEnable() {
        USNL.CallbackEvents.OnHealthBarPacket += OnHealthBarPacket;
    }

    private void OnDisable() {
        USNL.CallbackEvents.OnHealthBarPacket -= OnHealthBarPacket;
    }
    
    private void OnHealthBarPacket(object _packetObject) {
        USNL.HealthBarPacket packet = (USNL.HealthBarPacket)_packetObject;

        clientId = USNL.ClientManager.instance.ClientId;

        if (packet.ClientId == clientId)
            healthBarImage.fillAmount = packet.CurrentHealth / packet.MaxHealth;
    }
}
