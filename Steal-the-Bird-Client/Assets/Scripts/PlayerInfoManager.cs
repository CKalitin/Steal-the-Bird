using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Should be a struct, ugh
[Serializable]
public class PlayerInfo {
    [SerializeField] private string username;

    [SerializeField] private float damageDealt;
    [SerializeField] private float damageTaken;

    [SerializeField] private int playerKills;
    [SerializeField] private int playerDeaths;

    [SerializeField] private int enemyKills;
    [SerializeField] private int enemyDeaths;

    [SerializeField] private bool ready;

    [SerializeField] private int score;

    public PlayerInfo() {
        username = "";
        damageDealt = 0f;
        damageTaken = 0f;
        playerKills = 0;
        playerDeaths = 0;
        enemyKills = 0;
        enemyDeaths = 0;
        ready = false;
        score = 0;
    }

    public string Username { get => username; set => username = value; }

    public float DamageDealt { get => damageDealt; set => damageDealt = value; }
    public float DamageTaken { get => damageTaken; set => damageTaken = value; }

    public int PlayerKills { get => playerKills; set => playerKills = value; }
    public int PlayerDeaths { get => playerDeaths; set => playerDeaths = value; }

    public int EnemyKills { get => enemyKills; set => enemyKills = value; }
    public int EnemyDeaths { get => enemyDeaths; set => enemyDeaths = value; }

    public bool Ready { get => ready; set => ready = value; }

    public int Score { get => score; set => score = value; }
}

public class PlayerInfoManager : MonoBehaviour {
    #region Variables
    
    public static PlayerInfoManager instance;

    [SerializeField] private List<PlayerInfo> playerInfos = null;

    public List<PlayerInfo> PlayerInfos { get => playerInfos; set => playerInfos = value; }

    #endregion

    #region Core

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.Log($"Match Manager instance already exists on ({gameObject}), destroying this.");
            Destroy(this);
        }
    }

    private void OnEnable() {
        USNL.CallbackEvents.OnPlayerInfoPacket += OnPlayerInfoPacket;
        USNL.CallbackEvents.OnPlayerReadyPacket += OnPlayerReadyPacket;
        USNL.CallbackEvents.OnServerInfoPacket += OnServerInfoPacket;
    }

    private void OnDisable() {
        USNL.CallbackEvents.OnPlayerInfoPacket -= OnPlayerInfoPacket;
        USNL.CallbackEvents.OnPlayerReadyPacket -= OnPlayerReadyPacket;
        USNL.CallbackEvents.OnServerInfoPacket -= OnServerInfoPacket;
    }

    #endregion

    #region Callbacks

    private void OnPlayerInfoPacket(object _packetObject) {
        USNL.PlayerInfoPacket packet = (USNL.PlayerInfoPacket)_packetObject;

        if (playerInfos.Count <= 0) return;
        
        playerInfos[packet.ClientId].Username = packet.Username;
        playerInfos[packet.ClientId].DamageDealt = packet.DamageDealt;
        playerInfos[packet.ClientId].DamageTaken = packet.DamageTaken;
        playerInfos[packet.ClientId].PlayerKills = packet.PlayerKills;
        playerInfos[packet.ClientId].PlayerDeaths = packet.PlayerDeaths;
        playerInfos[packet.ClientId].EnemyKills = packet.EnemyKills;
        playerInfos[packet.ClientId].EnemyDeaths = packet.EnemyDeaths;
        playerInfos[packet.ClientId].Score = packet.Score;
    }

    private void OnPlayerReadyPacket(object _packetObject) {
        USNL.PlayerReadyPacket packet = (USNL.PlayerReadyPacket)_packetObject;
        playerInfos[packet.ClientId].Ready = packet.Ready;
    }

    private void OnServerInfoPacket(object _clientIdObject) {
        USNL.Package.ServerInfoPacket packet = (USNL.Package.ServerInfoPacket)_clientIdObject;
        
        if (playerInfos.Count > 0) return;
        
        playerInfos = new List<PlayerInfo>();
        for (int i = 0; i < packet.MaxClients; i++)
            playerInfos.Add(new PlayerInfo());
    }

    #endregion
}
