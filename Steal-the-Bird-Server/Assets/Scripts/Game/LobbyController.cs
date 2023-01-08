using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviour {
    [SerializeField] private float waitTimeAllReady = 5f;
    [SerializeField] private float waitTimeOneOrMoreReady = 30f;

    private bool noneReady;
    private bool oneOrMoreReady;
    private bool allReady;

    private void Update() {

    }
    
    private void CheckReadiness() {
        int readyCount = 0;

        for (int i = 0; i < PlayerInfoManager.instance.PlayerInfos.Count; i++) {
            if (PlayerInfoManager.instance.PlayerInfos[i].Ready) readyCount++;
        }

        if (readyCount == 0 && !noneReady) {
            noneReady = true;
            oneOrMoreReady = false;
            allReady = false;
            MatchManager.instance.NewCountdown(-1, MatchState.InGame, "Lobby Countdown");
            return;
        }

        if (readyCount > 0 && !oneOrMoreReady) {
            oneOrMoreReady = true;
            noneReady = false;
            allReady = false;
            MatchManager.instance.NewCountdown(waitTimeOneOrMoreReady, MatchState.InGame, "Lobby Countdown");
            return;
        }

        if (readyCount == PlayerInfoManager.instance.PlayerInfos.Count && !allReady) {
            noneReady = false;
            oneOrMoreReady = false;
            allReady = true;
            MatchManager.instance.NewCountdown(waitTimeAllReady, MatchState.InGame, "Lobby Countdown");
            return;
        }
    }

    private void OnEnable() {
        USNL.CallbackEvents.OnPlayerSetupInfoPacket += OnPlayerSetupInfoPacket;
    }

    private void OnDisable() {
        USNL.CallbackEvents.OnPlayerSetupInfoPacket -= OnPlayerSetupInfoPacket;
    }

    private void OnPlayerSetupInfoPacket(object _packetObject) {
        USNL.PlayerSetupInfoPacket packet = (USNL.PlayerSetupInfoPacket)_packetObject;

        USNL.PacketSend.PlayerConfig(packet.FromClient, packet.CharacterId, packet.Username);
    }
}
