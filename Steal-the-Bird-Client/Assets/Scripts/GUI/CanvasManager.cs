using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour {
    [SerializeField] private GameObject[] enableInLobby;
    [SerializeField] private GameObject[] enableInGame;
    [Space]
    [SerializeField] private GameObject timedOutCanvas;
    [SerializeField] private GameObject serverClosedCanvas;

    private void Awake() {
        CheckMatchState(MatchManager.instance.MatchState);
    }

    private void Update() {
        if (USNL.ClientManager.instance.TimedOut)
            timedOutCanvas.SetActive(true);
        else if (USNL.ClientManager.instance.ServerClosed)
            serverClosedCanvas.SetActive(true);
    }

    private void OnEnable() {
        USNL.CallbackEvents.OnMatchUpdatePacket += OnMatchUpdatePacket;
    }

    private void OnDisable() {
        USNL.CallbackEvents.OnMatchUpdatePacket -= OnMatchUpdatePacket;
    }

    private void CheckMatchState(MatchState _state) {
        if (_state == MatchState.Lobby) {
            EnableInLobby(true);
            EnableInGame(false);
        } else if (_state == MatchState.InGame | _state == MatchState.Paused) {
            EnableInLobby(false);
            EnableInGame(true);
        }
    }

    private void EnableInLobby(bool _enabled) {
        for (int i = 0; i < enableInLobby.Length; i++)
            enableInLobby[i].SetActive(_enabled);
    }

    private void EnableInGame(bool _enabled) {
        for (int i = 0; i < enableInGame.Length; i++)
            enableInGame[i].SetActive(_enabled);
    }

    private void OnMatchUpdatePacket(object _packetObject) {
        USNL.MatchUpdatePacket packet = (USNL.MatchUpdatePacket)_packetObject;

        CheckMatchState((MatchState)packet.MatchState);
    }
}
