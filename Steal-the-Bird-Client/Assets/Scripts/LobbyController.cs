using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviour {
    public static LobbyController instance;

    [Header("Characters")]
    [SerializeField] private Transform[] characterLocations;
    [SerializeField] private CharacterSelection[] characterSelections;
    [Space]
    [SerializeField] private string[] characterIdsToSyncedObjectTags;

    [Header("Other")]
    [SerializeField] private USNL.Package.SyncedObjectPrefabs syncedObjectPrefabs;

    private Dictionary<int, GameObject> characters = new Dictionary<int, GameObject>();

    public int TotalCharacters { get => characterIdsToSyncedObjectTags.Length; }

    private void Awake() {
        if (instance == null) instance = this;
        else {
            Debug.Log($"Lobby Controller instance already exists on ({gameObject}), destroying this.");
            Destroy(this);
        }
    }

    private void Update() {
        CheckForDisconnectedClients();
    }

    private void OnEnable() {
        USNL.CallbackEvents.OnPlayerConfigPacket += OnPlayerConfigPacket;
        USNL.CallbackEvents.OnPlayerReadyPacket += OnPlayerReadyPacket;
        USNL.CallbackEvents.OnConnected += OnConnected;
    }

    private void OnDisable() {
        USNL.CallbackEvents.OnPlayerConfigPacket -= OnPlayerConfigPacket;
        USNL.CallbackEvents.OnPlayerReadyPacket -= OnPlayerReadyPacket;
        USNL.CallbackEvents.OnConnected -= OnConnected;
    }

    private void OnPlayerConfigPacket(object _packetObject) {
        USNL.PlayerConfigPacket packet = (USNL.PlayerConfigPacket)_packetObject;

        int id = GetIndex(packet.ClientId);
        
        // If client disconnected
        if (packet.CharacterId <= 0) {
            characterSelections[id].Toggle(false);
        }

        characterSelections[id].Toggle(true);
        characterSelections[id].UsernameText.text = packet.Username;

        if (characters.ContainsKey(id)) {
            Destroy(characters[id]);
            characters.Remove(id);
        }

        if (packet.CharacterId < 0) return;
        
        GameObject newCharacter = Instantiate(syncedObjectPrefabs.SyncedObjects[characterIdsToSyncedObjectTags[packet.CharacterId]], characterLocations[id]);
        newCharacter.transform.localPosition = Vector3.zero;
        characters.Add(id, newCharacter);
    }

    private void CheckForDisconnectedClients() {
        for (int i = 0; i < characterSelections.Length; i++) {
            if (USNL.ClientManager.instance.CheckClientConnected(i)) continue;
            characterSelections[GetIndex(i)].Toggle(false);
        }
    }

    private void OnPlayerReadyPacket(object _packetObject) {
        USNL.PlayerReadyPacket packet = (USNL.PlayerReadyPacket)_packetObject;

        int id = GetIndex(packet.ClientId);

        characterSelections[id].Toggle(true);
        characterSelections[id].TogglableIsReady.SetActive(packet.Ready);
    }

    private void OnConnected(object _object) {
        USNL.PacketSend.PlayerSetupInfo(PlayerPrefs.GetString("Username", "Username"), 0);
    }

    private int GetIndex(int _id) {
        if (_id == USNL.ClientManager.instance.ClientId) return 0;
        else if (_id == 0) return USNL.ClientManager.instance.ClientId;
        return _id;
    }
}
