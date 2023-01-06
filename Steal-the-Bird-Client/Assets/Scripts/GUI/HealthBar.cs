using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    [SerializeField] private Image healthBarImage;

    private int clientId = -1;

    private void OnEnable() {
        USNL.CallbackEvents.OnConnected += OnConnected;
        USNL.CallbackEvents.OnHealthBarPacket += OnHealthBarPacket;
    }

    private void OnDisable() {
        USNL.CallbackEvents.OnConnected -= OnConnected;
        USNL.CallbackEvents.OnHealthBarPacket -= OnHealthBarPacket;
    }

    private void OnConnected(object _object) {
        clientId = USNL.ClientManager.instance.ClientId;
    }

    private void OnHealthBarPacket(object _packetObject) {
        USNL.HealthBarPacket packet = (USNL.HealthBarPacket)_packetObject;

        if (packet.ClientId == clientId)
            healthBarImage.fillAmount = packet.CurrentHealth / packet.MaxHealth;
    }
}
