using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Should be a struct, ugh
public class PlayerInfo {
    private string username;
    
    private float damageDealt;
    private float damageTaken;

    private int playerKills;
    private int playerDeaths;

    private int enemyKills;
    private int enemyDeaths;

    private bool ready;

    public string Username { get => username; set => username = value; }
    
    public float DamageDealt { get => damageDealt; set => damageDealt = value; }
    public float DamageTaken { get => damageTaken; set => damageTaken = value; }
    
    public int PlayerKills { get => playerKills; set => playerKills = value; }
    public int PlayerDeaths { get => playerDeaths; set => playerDeaths = value; }
    
    public int EnemyKills { get => enemyKills; set => enemyKills = value; }
    public int EnemyDeaths { get => enemyDeaths; set => enemyDeaths = value; }
    
    public bool Ready { get => ready; set => ready = value; }
}

public class PlayerInfoManager : MonoBehaviour {
    #region Variables

    public static PlayerInfoManager instance;
    
    private List<PlayerInfo> playerInfos = new List<PlayerInfo>();

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
        
        Initialize();
    }

    private void OnEnable() {
        USNL.CallbackEvents.OnPlayerInfoPacket += OnPlayerInfoPacket;
        USNL.CallbackEvents.OnPlayerReadyPacket += OnPlayerReadyPacket;
        USNL.CallbackEvents.OnClientDisconnected += OnClientDisconnected;
    }

    private void OnDisable() {
        USNL.CallbackEvents.OnPlayerInfoPacket -= OnPlayerInfoPacket;
        USNL.CallbackEvents.OnPlayerReadyPacket -= OnPlayerReadyPacket;
        USNL.CallbackEvents.OnClientDisconnected -= OnClientDisconnected;
    }

    #endregion
    
    #region Player Info Management

    private void Initialize() {
        playerInfos = new List<PlayerInfo>();
        for (int i = 0; i < USNL.ServerManager.instance.ServerConfig.MaxClients; i++) {
            playerInfos.Add(new PlayerInfo());
        }
    }

    public void SendPlayerInfo(int _id) {
        USNL.PacketSend.PlayerInfo(_id, playerInfos[_id].Username, playerInfos[_id].DamageDealt, playerInfos[_id].DamageTaken, playerInfos[_id].PlayerKills, playerInfos[_id].PlayerDeaths, playerInfos[_id].EnemyKills, playerInfos[_id].EnemyDeaths);
    }

    #endregion

    #region Callbacks

    private void OnPlayerInfoPacket(object _packetObject) {
        USNL.PlayerInfoPacket packet = (USNL.PlayerInfoPacket)_packetObject;
        playerInfos[packet.FromClient].Username = packet.Username;
    }

    private void OnPlayerReadyPacket(object _packetObject) {
        USNL.PlayerReadyPacket packet = (USNL.PlayerReadyPacket)_packetObject;
        playerInfos[packet.FromClient].Ready = packet.Ready;
    }

    private void OnClientDisconnected(object _clientIdObject) {
        int clientId = (int)_clientIdObject;

        playerInfos[clientId] = new PlayerInfo();
    }

    #endregion
}
