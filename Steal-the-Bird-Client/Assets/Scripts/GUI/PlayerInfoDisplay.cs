using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInfoDisplay : MonoBehaviour {
    [SerializeField] private DisplayType displayType;
    [Space]
    [SerializeField] private GameObject playerInfoElementPrefab;
    [SerializeField] private Transform playerInfoElementsParent;
    [SerializeField] private float ySpacing;
    [Space]
    [SerializeField] private int maxElements;
    
    private enum DisplayType {
        SortById,
        SortByScore,
        SortByTotalKdRatio,
        SortByPlayerKdRatio
    }

    private Dictionary<int, Transform> playerInfoElements = new Dictionary<int, Transform>();

    private void Update() {
        DisplayPlayerInfos();
    }

    private void OnEnable() {
        USNL.CallbackEvents.OnDisconnectClientPacket += OnClientDisconnected;
        USNL.CallbackEvents.OnPlayerInfoPacket += OnPlayerInfoPacket;
    }

    private void OnDisable() {
        USNL.CallbackEvents.OnDisconnectClientPacket -= OnClientDisconnected;
        USNL.CallbackEvents.OnPlayerInfoPacket -= OnPlayerInfoPacket;
    }

    private void DisplayPlayerInfos() {
        int[] connectedClientIds = USNL.ClientManager.instance.ServerInfo.ConnectedClientIds;

        //CheckForDisconnectedClients(connectedClientIds);

        List<PlayerInfo> pis = PlayerInfoManager.instance.PlayerInfos;

        int[] playerScores = new int[connectedClientIds.Length];
        for (int i = 0; i < pis.Count; i++)
            if (connectedClientIds.Contains(i)) playerScores[i] = pis[i].Score;
        
        int[] kDRatios = new int[connectedClientIds.Length];
        for (int i = 0; i < pis.Count; i++)
            if (connectedClientIds.Contains(i)) kDRatios[i] = Mathf.Clamp(pis[i].PlayerKills + pis[i].EnemyKills, 1, 999999999) / Mathf.Clamp(pis[i].PlayerDeaths + pis[i].PlayerKills, 1, 999999999);

        int[] playerKDRatios = new int[connectedClientIds.Length];
        for (int i = 0; i < pis.Count; i++)
            if (connectedClientIds.Contains(i)) playerKDRatios[i] = Mathf.Clamp(pis[i].PlayerKills, 1, 999999999) / Mathf.Clamp(pis[i].PlayerDeaths, 1, 999999999);

        if (displayType == DisplayType.SortById) {
            DisplayPlayerInfos(connectedClientIds);
        } else if (displayType == DisplayType.SortByScore) {
            //Debug.Log($"1: ({String.Join(", ", connectedClientIds)}), ({String.Join(", ", playerScores)})");
            int[] ids = connectedClientIds.OrderBy(x => -playerScores[x]).ToArray();
            DisplayPlayerInfos(ids);
        } else if (displayType == DisplayType.SortByTotalKdRatio) {
            int[] ids = connectedClientIds.OrderBy(x => -kDRatios[x]).ToArray();
            DisplayPlayerInfos(ids);
        } else if (displayType == DisplayType.SortByPlayerKdRatio) {
            int[] ids = connectedClientIds.OrderBy(x => -playerKDRatios[x]).ToArray();
            DisplayPlayerInfos(connectedClientIds);
        }
    }

    private void DisplayPlayerInfos(int[] indexes) {
        int count = 0;
        for (int i = 0; i < indexes.Length; i++) {
            if (count >= maxElements) return;

            int x = indexes[i];
            if (playerInfoElements.ContainsKey(x)) {
                playerInfoElements[x].localPosition = new Vector3(0, -count * ySpacing, 0);
                count++;
            } else {
                playerInfoElements.Add(x, Instantiate(playerInfoElementPrefab, playerInfoElementsParent).transform);
                playerInfoElements[x].localPosition = Vector3.zero;
                playerInfoElements[x].localPosition = new Vector3(0, -count * ySpacing, 0);
                count++;
            }
        }
    }

    // TODO DELETE
    private void CheckForDisconnectedClients(int[] connectedClientIds) {
        for (int i = 0; i < playerInfoElements.Count; i++) {
            if (!connectedClientIds.Contains(i)) {
                if (playerInfoElements[i]) Destroy(playerInfoElements[i].gameObject);
                playerInfoElements.Remove(i);
            }
        }
    }

    private void OnClientDisconnected(object _clientIdObject) {
        int clientId = (int)_clientIdObject;

        // Remove player info element if it disconnects
        if (playerInfoElements.ContainsKey(clientId)) {
            Destroy(playerInfoElements[clientId].gameObject);
            playerInfoElements.Remove(clientId);
        }
    }

    private void OnPlayerInfoPacket(object _packetObject) {
        USNL.PlayerInfoPacket packet = (USNL.PlayerInfoPacket)_packetObject;

        DisplayPlayerInfos();

        if (playerInfoElements.ContainsKey(packet.ClientId)) {
            PlayerInfoElement playerInfoElement = playerInfoElements[packet.ClientId].GetComponent<PlayerInfoElement>();
            playerInfoElement.SetInfo(packet);
        }
    }
}
