using System.Collections.Generic;
using UnityEngine;

namespace USNL {
    #region Packet Enums

    public enum ClientPackets {
        WelcomeReceived,
        Ping,
        ClientInput,
        RaycastFromCamera,
        PlayerInfo,
        PlayerReady,
    }

    public enum ServerPackets {
        Welcome,
        Ping,
        ServerInfo,
        DisconnectClient,
        SyncedObjectInstantiate,
        SyncedObjectDestroy,
        SyncedObjectInterpolationMode,
        SyncedObjectVec2PosUpdate,
        SyncedObjectVec3PosUpdate,
        SyncedObjectRotZUpdate,
        SyncedObjectRotUpdate,
        SyncedObjectVec2ScaleUpdate,
        SyncedObjectVec3ScaleUpdate,
        SyncedObjectVec2PosInterpolation,
        SyncedObjectVec3PosInterpolation,
        SyncedObjectRotZInterpolation,
        SyncedObjectRotInterpolation,
        SyncedObjectVec2ScaleInterpolation,
        SyncedObjectVec3ScaleInterpolation,
        PlayerSpawned,
        MatchUpdate,
        Countdown,
        BirdDeath,
        PlayerInfo,
        PlayerReady,
    }

    #endregion

    #region Packet Structs

    public struct PlayerSpawnedPacket {
        private int clientId;
        private int playerSyncedObjectUUID;

        public PlayerSpawnedPacket(int _clientId, int _playerSyncedObjectUUID) {
            clientId = _clientId;
            playerSyncedObjectUUID = _playerSyncedObjectUUID;
        }

        public int ClientId { get => clientId; set => clientId = value; }
        public int PlayerSyncedObjectUUID { get => playerSyncedObjectUUID; set => playerSyncedObjectUUID = value; }
    }

    public struct MatchUpdatePacket {
        private int matchState;

        public MatchUpdatePacket(int _matchState) {
            matchState = _matchState;
        }

        public int MatchState { get => matchState; set => matchState = value; }
    }

    public struct CountdownPacket {
        private int[] startTimeArray;
        private float duration;
        private string countdownTag;

        public CountdownPacket(int[] _startTimeArray, float _duration, string _countdownTag) {
            startTimeArray = _startTimeArray;
            duration = _duration;
            countdownTag = _countdownTag;
        }

        public int[] StartTimeArray { get => startTimeArray; set => startTimeArray = value; }
        public float Duration { get => duration; set => duration = value; }
        public string CountdownTag { get => countdownTag; set => countdownTag = value; }
    }

    public struct BirdDeathPacket {
        private int syncedObjectUUID;
        private Vector3 landPosition;
        private bool landOnWater;
        private float fallSpeed;

        public BirdDeathPacket(int _syncedObjectUUID, Vector3 _landPosition, bool _landOnWater, float _fallSpeed) {
            syncedObjectUUID = _syncedObjectUUID;
            landPosition = _landPosition;
            landOnWater = _landOnWater;
            fallSpeed = _fallSpeed;
        }

        public int SyncedObjectUUID { get => syncedObjectUUID; set => syncedObjectUUID = value; }
        public Vector3 LandPosition { get => landPosition; set => landPosition = value; }
        public bool LandOnWater { get => landOnWater; set => landOnWater = value; }
        public float FallSpeed { get => fallSpeed; set => fallSpeed = value; }
    }

    public struct PlayerInfoPacket {
        private int clientId;
        private string username;
        private float damageDealt;
        private float damageTaken;
        private int playerKills;
        private int playerDeaths;
        private int enemyKills;
        private int enemyDeaths;

        public PlayerInfoPacket(int _clientId, string _username, float _damageDealt, float _damageTaken, int _playerKills, int _playerDeaths, int _enemyKills, int _enemyDeaths) {
            clientId = _clientId;
            username = _username;
            damageDealt = _damageDealt;
            damageTaken = _damageTaken;
            playerKills = _playerKills;
            playerDeaths = _playerDeaths;
            enemyKills = _enemyKills;
            enemyDeaths = _enemyDeaths;
        }

        public int ClientId { get => clientId; set => clientId = value; }
        public string Username { get => username; set => username = value; }
        public float DamageDealt { get => damageDealt; set => damageDealt = value; }
        public float DamageTaken { get => damageTaken; set => damageTaken = value; }
        public int PlayerKills { get => playerKills; set => playerKills = value; }
        public int PlayerDeaths { get => playerDeaths; set => playerDeaths = value; }
        public int EnemyKills { get => enemyKills; set => enemyKills = value; }
        public int EnemyDeaths { get => enemyDeaths; set => enemyDeaths = value; }
    }

    public struct PlayerReadyPacket {
        private bool ready;

        public PlayerReadyPacket(bool _ready) {
            ready = _ready;
        }

        public bool Ready { get => ready; set => ready = value; }
    }


    #endregion

    #region Packet Handlers

    public static class PacketHandlers {
       public delegate void PacketHandler(USNL.Package.Packet _packet);
        public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {
            { Package.PacketHandlers.Welcome },
            { Package.PacketHandlers.Ping },
            { Package.PacketHandlers.ServerInfo },
            { Package.PacketHandlers.DisconnectClient },
            { Package.PacketHandlers.SyncedObjectInstantiate },
            { Package.PacketHandlers.SyncedObjectDestroy },
            { Package.PacketHandlers.SyncedObjectInterpolationMode },
            { Package.PacketHandlers.SyncedObjectVec2PosUpdate },
            { Package.PacketHandlers.SyncedObjectVec3PosUpdate },
            { Package.PacketHandlers.SyncedObjectRotZUpdate },
            { Package.PacketHandlers.SyncedObjectRotUpdate },
            { Package.PacketHandlers.SyncedObjectVec2ScaleUpdate },
            { Package.PacketHandlers.SyncedObjectVec3ScaleUpdate },
            { Package.PacketHandlers.SyncedObjectVec2PosInterpolation },
            { Package.PacketHandlers.SyncedObjectVec3PosInterpolation },
            { Package.PacketHandlers.SyncedObjectRotZInterpolation },
            { Package.PacketHandlers.SyncedObjectRotInterpolation },
            { Package.PacketHandlers.SyncedObjectVec2ScaleInterpolation },
            { Package.PacketHandlers.SyncedObjectVec3ScaleInterpolation },
            { PlayerSpawned },
            { MatchUpdate },
            { Countdown },
            { BirdDeath },
            { PlayerInfo },
            { PlayerReady },
        };

        public static void PlayerSpawned(Package.Packet _packet) {
            int clientId = _packet.ReadInt();
            int playerSyncedObjectUUID = _packet.ReadInt();

            USNL.PlayerSpawnedPacket playerSpawnedPacket = new USNL.PlayerSpawnedPacket(clientId, playerSyncedObjectUUID);
            Package.PacketManager.instance.PacketReceived(_packet, playerSpawnedPacket);
        }

        public static void MatchUpdate(Package.Packet _packet) {
            int matchState = _packet.ReadInt();

            USNL.MatchUpdatePacket matchUpdatePacket = new USNL.MatchUpdatePacket(matchState);
            Package.PacketManager.instance.PacketReceived(_packet, matchUpdatePacket);
        }

        public static void Countdown(Package.Packet _packet) {
            int[] startTimeArray = _packet.ReadInts();
            float duration = _packet.ReadFloat();
            string countdownTag = _packet.ReadString();

            USNL.CountdownPacket countdownPacket = new USNL.CountdownPacket(startTimeArray, duration, countdownTag);
            Package.PacketManager.instance.PacketReceived(_packet, countdownPacket);
        }

        public static void BirdDeath(Package.Packet _packet) {
            int syncedObjectUUID = _packet.ReadInt();
            Vector3 landPosition = _packet.ReadVector3();
            bool landOnWater = _packet.ReadBool();
            float fallSpeed = _packet.ReadFloat();

            USNL.BirdDeathPacket birdDeathPacket = new USNL.BirdDeathPacket(syncedObjectUUID, landPosition, landOnWater, fallSpeed);
            Package.PacketManager.instance.PacketReceived(_packet, birdDeathPacket);
        }

        public static void PlayerInfo(Package.Packet _packet) {
            int clientId = _packet.ReadInt();
            string username = _packet.ReadString();
            float damageDealt = _packet.ReadFloat();
            float damageTaken = _packet.ReadFloat();
            int playerKills = _packet.ReadInt();
            int playerDeaths = _packet.ReadInt();
            int enemyKills = _packet.ReadInt();
            int enemyDeaths = _packet.ReadInt();

            USNL.PlayerInfoPacket playerInfoPacket = new USNL.PlayerInfoPacket(clientId, username, damageDealt, damageTaken, playerKills, playerDeaths, enemyKills, enemyDeaths);
            Package.PacketManager.instance.PacketReceived(_packet, playerInfoPacket);
        }

        public static void PlayerReady(Package.Packet _packet) {
            bool ready = _packet.ReadBool();

            USNL.PlayerReadyPacket playerReadyPacket = new USNL.PlayerReadyPacket(ready);
            Package.PacketManager.instance.PacketReceived(_packet, playerReadyPacket);
        }
    }

    #endregion

    #region Packet Send

    public static class PacketSend {
        private static void SendTCPData(USNL.Package.Packet _packet) {
            _packet.WriteLength();
            if (USNL.Package.Client.instance.IsConnected) {
                USNL.Package.Client.instance.Tcp.SendData(_packet);
                NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length());
            }
        }
    
        private static void SendUDPData(USNL.Package.Packet _packet) {
            _packet.WriteLength();
            if (USNL.Package.Client.instance.IsConnected) {
                USNL.Package.Client.instance.Udp.SendData(_packet);
                NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length());
            }
        }
    
        public static void RaycastFromCamera(Vector3 _cameraPosition, Quaternion _cameraRotation, Vector2 _resolution, float _fieldOfView, Vector2 _mousePosition) {
            using (Package.Packet _packet = new Package.Packet((int)USNL.ClientPackets.RaycastFromCamera)) {
                _packet.Write(_cameraPosition);
                _packet.Write(_cameraRotation);
                _packet.Write(_resolution);
                _packet.Write(_fieldOfView);
                _packet.Write(_mousePosition);

                SendTCPData(_packet);
            }
        }

        public static void PlayerInfo(string _username) {
            using (Package.Packet _packet = new Package.Packet((int)USNL.ClientPackets.PlayerInfo)) {
                _packet.Write(_username);

                SendTCPData(_packet);
            }
        }

        public static void PlayerReady(bool _ready) {
            using (Package.Packet _packet = new Package.Packet((int)USNL.ClientPackets.PlayerReady)) {
                _packet.Write(_ready);

                SendTCPData(_packet);
            }
        }
    }

#endregion
}

namespace USNL.Package {
    #region Packet Enums
    public enum ClientPackets {
        WelcomeReceived,
        Ping,
        ClientInput,
        RaycastFromCamera,
        PlayerInfo,
        PlayerReady,
    }

    public enum ServerPackets {
        Welcome,
        Ping,
        ServerInfo,
        DisconnectClient,
        SyncedObjectInstantiate,
        SyncedObjectDestroy,
        SyncedObjectInterpolationMode,
        SyncedObjectVec2PosUpdate,
        SyncedObjectVec3PosUpdate,
        SyncedObjectRotZUpdate,
        SyncedObjectRotUpdate,
        SyncedObjectVec2ScaleUpdate,
        SyncedObjectVec3ScaleUpdate,
        SyncedObjectVec2PosInterpolation,
        SyncedObjectVec3PosInterpolation,
        SyncedObjectRotZInterpolation,
        SyncedObjectRotInterpolation,
        SyncedObjectVec2ScaleInterpolation,
        SyncedObjectVec3ScaleInterpolation,
        PlayerSpawned,
        MatchUpdate,
        Countdown,
        BirdDeath,
        PlayerInfo,
        PlayerReady,
    }
    #endregion

    #region Packet Structs

    public struct WelcomePacket {
        private string welcomeMessage;
        private string serverName;
        private int clientId;

        public WelcomePacket(string _welcomeMessage, string _serverName, int _clientId) {
            welcomeMessage = _welcomeMessage;
            serverName = _serverName;
            clientId = _clientId;
        }

        public string WelcomeMessage { get => welcomeMessage; set => welcomeMessage = value; }
        public string ServerName { get => serverName; set => serverName = value; }
        public int ClientId { get => clientId; set => clientId = value; }
    }

    public struct PingPacket {
        private bool placeholder;

        public PingPacket(bool _placeholder) {
            placeholder = _placeholder;
        }

        public bool Placeholder { get => placeholder; set => placeholder = value; }
    }

    public struct ServerInfoPacket {
        private string serverName;
        private int[] connectedClientsIds;
        private int maxClients;
        private bool serverFull;

        public ServerInfoPacket(string _serverName, int[] _connectedClientsIds, int _maxClients, bool _serverFull) {
            serverName = _serverName;
            connectedClientsIds = _connectedClientsIds;
            maxClients = _maxClients;
            serverFull = _serverFull;
        }

        public string ServerName { get => serverName; set => serverName = value; }
        public int[] ConnectedClientsIds { get => connectedClientsIds; set => connectedClientsIds = value; }
        public int MaxClients { get => maxClients; set => maxClients = value; }
        public bool ServerFull { get => serverFull; set => serverFull = value; }
    }

    public struct DisconnectClientPacket {
        private string disconnectMessage;

        public DisconnectClientPacket(string _disconnectMessage) {
            disconnectMessage = _disconnectMessage;
        }

        public string DisconnectMessage { get => disconnectMessage; set => disconnectMessage = value; }
    }

    public struct SyncedObjectInstantiatePacket {
        private string syncedObjectTag;
        private int syncedObjectUUID;
        private Vector3 position;
        private Quaternion rotation;
        private Vector3 scale;

        public SyncedObjectInstantiatePacket(string _syncedObjectTag, int _syncedObjectUUID, Vector3 _position, Quaternion _rotation, Vector3 _scale) {
            syncedObjectTag = _syncedObjectTag;
            syncedObjectUUID = _syncedObjectUUID;
            position = _position;
            rotation = _rotation;
            scale = _scale;
        }

        public string SyncedObjectTag { get => syncedObjectTag; set => syncedObjectTag = value; }
        public int SyncedObjectUUID { get => syncedObjectUUID; set => syncedObjectUUID = value; }
        public Vector3 Position { get => position; set => position = value; }
        public Quaternion Rotation { get => rotation; set => rotation = value; }
        public Vector3 Scale { get => scale; set => scale = value; }
    }

    public struct SyncedObjectDestroyPacket {
        private int syncedObjectUUID;

        public SyncedObjectDestroyPacket(int _syncedObjectUUID) {
            syncedObjectUUID = _syncedObjectUUID;
        }

        public int SyncedObjectUUID { get => syncedObjectUUID; set => syncedObjectUUID = value; }
    }

    public struct SyncedObjectInterpolationModePacket {
        private bool serverInterpolation;

        public SyncedObjectInterpolationModePacket(bool _serverInterpolation) {
            serverInterpolation = _serverInterpolation;
        }

        public bool ServerInterpolation { get => serverInterpolation; set => serverInterpolation = value; }
    }

    public struct SyncedObjectVec2PosUpdatePacket {
        private int[] syncedObjectUUIDs;
        private Vector2[] positions;

        public SyncedObjectVec2PosUpdatePacket(int[] _syncedObjectUUIDs, Vector2[] _positions) {
            syncedObjectUUIDs = _syncedObjectUUIDs;
            positions = _positions;
        }

        public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
        public Vector2[] Positions { get => positions; set => positions = value; }
    }

    public struct SyncedObjectVec3PosUpdatePacket {
        private int[] syncedObjectUUIDs;
        private Vector3[] positions;

        public SyncedObjectVec3PosUpdatePacket(int[] _syncedObjectUUIDs, Vector3[] _positions) {
            syncedObjectUUIDs = _syncedObjectUUIDs;
            positions = _positions;
        }

        public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
        public Vector3[] Positions { get => positions; set => positions = value; }
    }

    public struct SyncedObjectRotZUpdatePacket {
        private int[] syncedObjectUUIDs;
        private float[] rotations;

        public SyncedObjectRotZUpdatePacket(int[] _syncedObjectUUIDs, float[] _rotations) {
            syncedObjectUUIDs = _syncedObjectUUIDs;
            rotations = _rotations;
        }

        public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
        public float[] Rotations { get => rotations; set => rotations = value; }
    }

    public struct SyncedObjectRotUpdatePacket {
        private int[] syncedObjectUUIDs;
        private Vector3[] rotations;

        public SyncedObjectRotUpdatePacket(int[] _syncedObjectUUIDs, Vector3[] _rotations) {
            syncedObjectUUIDs = _syncedObjectUUIDs;
            rotations = _rotations;
        }

        public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
        public Vector3[] Rotations { get => rotations; set => rotations = value; }
    }

    public struct SyncedObjectVec2ScaleUpdatePacket {
        private int[] syncedObjectUUIDs;
        private Vector2[] scales;

        public SyncedObjectVec2ScaleUpdatePacket(int[] _syncedObjectUUIDs, Vector2[] _scales) {
            syncedObjectUUIDs = _syncedObjectUUIDs;
            scales = _scales;
        }

        public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
        public Vector2[] Scales { get => scales; set => scales = value; }
    }

    public struct SyncedObjectVec3ScaleUpdatePacket {
        private int[] syncedObjectUUIDs;
        private Vector3[] scales;

        public SyncedObjectVec3ScaleUpdatePacket(int[] _syncedObjectUUIDs, Vector3[] _scales) {
            syncedObjectUUIDs = _syncedObjectUUIDs;
            scales = _scales;
        }

        public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
        public Vector3[] Scales { get => scales; set => scales = value; }
    }

    public struct SyncedObjectVec2PosInterpolationPacket {
        private int[] syncedObjectUUIDs;
        private Vector2[] interpolatePositions;

        public SyncedObjectVec2PosInterpolationPacket(int[] _syncedObjectUUIDs, Vector2[] _interpolatePositions) {
            syncedObjectUUIDs = _syncedObjectUUIDs;
            interpolatePositions = _interpolatePositions;
        }

        public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
        public Vector2[] InterpolatePositions { get => interpolatePositions; set => interpolatePositions = value; }
    }

    public struct SyncedObjectVec3PosInterpolationPacket {
        private int[] syncedObjectUUIDs;
        private Vector3[] interpolatePositions;

        public SyncedObjectVec3PosInterpolationPacket(int[] _syncedObjectUUIDs, Vector3[] _interpolatePositions) {
            syncedObjectUUIDs = _syncedObjectUUIDs;
            interpolatePositions = _interpolatePositions;
        }

        public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
        public Vector3[] InterpolatePositions { get => interpolatePositions; set => interpolatePositions = value; }
    }

    public struct SyncedObjectRotZInterpolationPacket {
        private int[] syncedObjectUUIDs;
        private float[] interpolateRotations;

        public SyncedObjectRotZInterpolationPacket(int[] _syncedObjectUUIDs, float[] _interpolateRotations) {
            syncedObjectUUIDs = _syncedObjectUUIDs;
            interpolateRotations = _interpolateRotations;
        }

        public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
        public float[] InterpolateRotations { get => interpolateRotations; set => interpolateRotations = value; }
    }

    public struct SyncedObjectRotInterpolationPacket {
        private int[] syncedObjectUUIDs;
        private Vector3[] interpolateRotations;

        public SyncedObjectRotInterpolationPacket(int[] _syncedObjectUUIDs, Vector3[] _interpolateRotations) {
            syncedObjectUUIDs = _syncedObjectUUIDs;
            interpolateRotations = _interpolateRotations;
        }

        public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
        public Vector3[] InterpolateRotations { get => interpolateRotations; set => interpolateRotations = value; }
    }

    public struct SyncedObjectVec2ScaleInterpolationPacket {
        private int[] syncedObjectUUIDs;
        private Vector2[] interpolateScales;

        public SyncedObjectVec2ScaleInterpolationPacket(int[] _syncedObjectUUIDs, Vector2[] _interpolateScales) {
            syncedObjectUUIDs = _syncedObjectUUIDs;
            interpolateScales = _interpolateScales;
        }

        public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
        public Vector2[] InterpolateScales { get => interpolateScales; set => interpolateScales = value; }
    }

    public struct SyncedObjectVec3ScaleInterpolationPacket {
        private int[] syncedObjectUUIDs;
        private Vector3[] interpolateScales;

        public SyncedObjectVec3ScaleInterpolationPacket(int[] _syncedObjectUUIDs, Vector3[] _interpolateScales) {
            syncedObjectUUIDs = _syncedObjectUUIDs;
            interpolateScales = _interpolateScales;
        }

        public int[] SyncedObjectUUIDs { get => syncedObjectUUIDs; set => syncedObjectUUIDs = value; }
        public Vector3[] InterpolateScales { get => interpolateScales; set => interpolateScales = value; }
    }


    #endregion

    #region Packet Handlers

    public static class PacketHandlers {
       public delegate void PacketHandler(USNL.Package.Packet _packet);
        public static List<PacketHandler> packetHandlers = new List<PacketHandler>() {
            { Welcome },
            { Ping },
            { ServerInfo },
            { DisconnectClient },
            { SyncedObjectInstantiate },
            { SyncedObjectDestroy },
            { SyncedObjectInterpolationMode },
            { SyncedObjectVec2PosUpdate },
            { SyncedObjectVec3PosUpdate },
            { SyncedObjectRotZUpdate },
            { SyncedObjectRotUpdate },
            { SyncedObjectVec2ScaleUpdate },
            { SyncedObjectVec3ScaleUpdate },
            { SyncedObjectVec2PosInterpolation },
            { SyncedObjectVec3PosInterpolation },
            { SyncedObjectRotZInterpolation },
            { SyncedObjectRotInterpolation },
            { SyncedObjectVec2ScaleInterpolation },
            { SyncedObjectVec3ScaleInterpolation },
            { USNL.PacketHandlers.PlayerSpawned },
            { USNL.PacketHandlers.MatchUpdate },
            { USNL.PacketHandlers.Countdown },
            { USNL.PacketHandlers.BirdDeath },
            { USNL.PacketHandlers.PlayerInfo },
            { USNL.PacketHandlers.PlayerReady },
        };

        public static void Welcome(Package.Packet _packet) {
            string welcomeMessage = _packet.ReadString();
            string serverName = _packet.ReadString();
            int clientId = _packet.ReadInt();

            Package.WelcomePacket welcomePacket = new Package.WelcomePacket(welcomeMessage, serverName, clientId);
            Package.PacketManager.instance.PacketReceived(_packet, welcomePacket);
        }

        public static void Ping(Package.Packet _packet) {
            bool placeholder = _packet.ReadBool();

            Package.PingPacket pingPacket = new Package.PingPacket(placeholder);
            Package.PacketManager.instance.PacketReceived(_packet, pingPacket);
        }

        public static void ServerInfo(Package.Packet _packet) {
            string serverName = _packet.ReadString();
            int[] connectedClientsIds = _packet.ReadInts();
            int maxClients = _packet.ReadInt();
            bool serverFull = _packet.ReadBool();

            Package.ServerInfoPacket serverInfoPacket = new Package.ServerInfoPacket(serverName, connectedClientsIds, maxClients, serverFull);
            Package.PacketManager.instance.PacketReceived(_packet, serverInfoPacket);
        }

        public static void DisconnectClient(Package.Packet _packet) {
            string disconnectMessage = _packet.ReadString();

            Package.DisconnectClientPacket disconnectClientPacket = new Package.DisconnectClientPacket(disconnectMessage);
            Package.PacketManager.instance.PacketReceived(_packet, disconnectClientPacket);
        }

        public static void SyncedObjectInstantiate(Package.Packet _packet) {
            string syncedObjectTag = _packet.ReadString();
            int syncedObjectUUID = _packet.ReadInt();
            Vector3 position = _packet.ReadVector3();
            Quaternion rotation = _packet.ReadQuaternion();
            Vector3 scale = _packet.ReadVector3();

            Package.SyncedObjectInstantiatePacket syncedObjectInstantiatePacket = new Package.SyncedObjectInstantiatePacket(syncedObjectTag, syncedObjectUUID, position, rotation, scale);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectInstantiatePacket);
        }

        public static void SyncedObjectDestroy(Package.Packet _packet) {
            int syncedObjectUUID = _packet.ReadInt();

            Package.SyncedObjectDestroyPacket syncedObjectDestroyPacket = new Package.SyncedObjectDestroyPacket(syncedObjectUUID);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectDestroyPacket);
        }

        public static void SyncedObjectInterpolationMode(Package.Packet _packet) {
            bool serverInterpolation = _packet.ReadBool();

            Package.SyncedObjectInterpolationModePacket syncedObjectInterpolationModePacket = new Package.SyncedObjectInterpolationModePacket(serverInterpolation);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectInterpolationModePacket);
        }

        public static void SyncedObjectVec2PosUpdate(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector2[] positions = _packet.ReadVector2s();

            Package.SyncedObjectVec2PosUpdatePacket syncedObjectVec2PosUpdatePacket = new Package.SyncedObjectVec2PosUpdatePacket(syncedObjectUUIDs, positions);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec2PosUpdatePacket);
        }

        public static void SyncedObjectVec3PosUpdate(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] positions = _packet.ReadVector3s();

            Package.SyncedObjectVec3PosUpdatePacket syncedObjectVec3PosUpdatePacket = new Package.SyncedObjectVec3PosUpdatePacket(syncedObjectUUIDs, positions);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec3PosUpdatePacket);
        }

        public static void SyncedObjectRotZUpdate(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            float[] rotations = _packet.ReadFloats();

            Package.SyncedObjectRotZUpdatePacket syncedObjectRotZUpdatePacket = new Package.SyncedObjectRotZUpdatePacket(syncedObjectUUIDs, rotations);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectRotZUpdatePacket);
        }

        public static void SyncedObjectRotUpdate(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] rotations = _packet.ReadVector3s();

            Package.SyncedObjectRotUpdatePacket syncedObjectRotUpdatePacket = new Package.SyncedObjectRotUpdatePacket(syncedObjectUUIDs, rotations);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectRotUpdatePacket);
        }

        public static void SyncedObjectVec2ScaleUpdate(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector2[] scales = _packet.ReadVector2s();

            Package.SyncedObjectVec2ScaleUpdatePacket syncedObjectVec2ScaleUpdatePacket = new Package.SyncedObjectVec2ScaleUpdatePacket(syncedObjectUUIDs, scales);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec2ScaleUpdatePacket);
        }

        public static void SyncedObjectVec3ScaleUpdate(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] scales = _packet.ReadVector3s();

            Package.SyncedObjectVec3ScaleUpdatePacket syncedObjectVec3ScaleUpdatePacket = new Package.SyncedObjectVec3ScaleUpdatePacket(syncedObjectUUIDs, scales);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec3ScaleUpdatePacket);
        }

        public static void SyncedObjectVec2PosInterpolation(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector2[] interpolatePositions = _packet.ReadVector2s();

            Package.SyncedObjectVec2PosInterpolationPacket syncedObjectVec2PosInterpolationPacket = new Package.SyncedObjectVec2PosInterpolationPacket(syncedObjectUUIDs, interpolatePositions);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec2PosInterpolationPacket);
        }

        public static void SyncedObjectVec3PosInterpolation(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] interpolatePositions = _packet.ReadVector3s();

            Package.SyncedObjectVec3PosInterpolationPacket syncedObjectVec3PosInterpolationPacket = new Package.SyncedObjectVec3PosInterpolationPacket(syncedObjectUUIDs, interpolatePositions);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec3PosInterpolationPacket);
        }

        public static void SyncedObjectRotZInterpolation(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            float[] interpolateRotations = _packet.ReadFloats();

            Package.SyncedObjectRotZInterpolationPacket syncedObjectRotZInterpolationPacket = new Package.SyncedObjectRotZInterpolationPacket(syncedObjectUUIDs, interpolateRotations);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectRotZInterpolationPacket);
        }

        public static void SyncedObjectRotInterpolation(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] interpolateRotations = _packet.ReadVector3s();

            Package.SyncedObjectRotInterpolationPacket syncedObjectRotInterpolationPacket = new Package.SyncedObjectRotInterpolationPacket(syncedObjectUUIDs, interpolateRotations);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectRotInterpolationPacket);
        }

        public static void SyncedObjectVec2ScaleInterpolation(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector2[] interpolateScales = _packet.ReadVector2s();

            Package.SyncedObjectVec2ScaleInterpolationPacket syncedObjectVec2ScaleInterpolationPacket = new Package.SyncedObjectVec2ScaleInterpolationPacket(syncedObjectUUIDs, interpolateScales);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec2ScaleInterpolationPacket);
        }

        public static void SyncedObjectVec3ScaleInterpolation(Package.Packet _packet) {
            int[] syncedObjectUUIDs = _packet.ReadInts();
            Vector3[] interpolateScales = _packet.ReadVector3s();

            Package.SyncedObjectVec3ScaleInterpolationPacket syncedObjectVec3ScaleInterpolationPacket = new Package.SyncedObjectVec3ScaleInterpolationPacket(syncedObjectUUIDs, interpolateScales);
            Package.PacketManager.instance.PacketReceived(_packet, syncedObjectVec3ScaleInterpolationPacket);
        }
    }

    #endregion

    #region Packet Send

    public static class PacketSend {
        private static void SendTCPData(USNL.Package.Packet _packet) {
            _packet.WriteLength();
            if (USNL.Package.Client.instance.IsConnected) {
                USNL.Package.Client.instance.Tcp.SendData(_packet);
                NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length());
            }
        }
    
        private static void SendUDPData(USNL.Package.Packet _packet) {
            _packet.WriteLength();
            if (USNL.Package.Client.instance.IsConnected) {
                USNL.Package.Client.instance.Udp.SendData(_packet);
                NetworkDebugInfo.instance.PacketSent(_packet.PacketId, _packet.Length());
            }
        }
    
        public static void WelcomeReceived(int _clientIdCheck) {
            using (Package.Packet _packet = new Package.Packet((int)USNL.Package.ClientPackets.WelcomeReceived)) {
                _packet.Write(_clientIdCheck);

                SendTCPData(_packet);
            }
        }

        public static void Ping(bool _sendPingBack, int _previousPingValue) {
            using (Package.Packet _packet = new Package.Packet((int)USNL.Package.ClientPackets.Ping)) {
                _packet.Write(_sendPingBack);
                _packet.Write(_previousPingValue);

                SendTCPData(_packet);
            }
        }

        public static void ClientInput(int[] _keycodesDown, int[] _keycodesUp) {
            using (Package.Packet _packet = new Package.Packet((int)USNL.Package.ClientPackets.ClientInput)) {
                _packet.Write(_keycodesDown);
                _packet.Write(_keycodesUp);

                SendTCPData(_packet);
            }
        }
    }

    #endregion
}

#region Callbacks

namespace USNL {
    public static class CallbackEvents {
        public delegate void CallbackEvent(object _param);

        public static CallbackEvent[] PacketCallbackEvents = {
            CallOnWelcomePacketCallbacks,
            CallOnPingPacketCallbacks,
            CallOnServerInfoPacketCallbacks,
            CallOnDisconnectClientPacketCallbacks,
            CallOnSyncedObjectInstantiatePacketCallbacks,
            CallOnSyncedObjectDestroyPacketCallbacks,
            CallOnSyncedObjectInterpolationModePacketCallbacks,
            CallOnSyncedObjectVec2PosUpdatePacketCallbacks,
            CallOnSyncedObjectVec3PosUpdatePacketCallbacks,
            CallOnSyncedObjectRotZUpdatePacketCallbacks,
            CallOnSyncedObjectRotUpdatePacketCallbacks,
            CallOnSyncedObjectVec2ScaleUpdatePacketCallbacks,
            CallOnSyncedObjectVec3ScaleUpdatePacketCallbacks,
            CallOnSyncedObjectVec2PosInterpolationPacketCallbacks,
            CallOnSyncedObjectVec3PosInterpolationPacketCallbacks,
            CallOnSyncedObjectRotZInterpolationPacketCallbacks,
            CallOnSyncedObjectRotInterpolationPacketCallbacks,
            CallOnSyncedObjectVec2ScaleInterpolationPacketCallbacks,
            CallOnSyncedObjectVec3ScaleInterpolationPacketCallbacks,
            CallOnPlayerSpawnedPacketCallbacks,
            CallOnMatchUpdatePacketCallbacks,
            CallOnCountdownPacketCallbacks,
            CallOnBirdDeathPacketCallbacks,
            CallOnPlayerInfoPacketCallbacks,
            CallOnPlayerReadyPacketCallbacks,
        };

        public static event CallbackEvent OnConnected;
        public static event CallbackEvent OnDisconnected;

        public static event CallbackEvent OnWelcomePacket;
        public static event CallbackEvent OnPingPacket;
        public static event CallbackEvent OnServerInfoPacket;
        public static event CallbackEvent OnDisconnectClientPacket;
        public static event CallbackEvent OnSyncedObjectInstantiatePacket;
        public static event CallbackEvent OnSyncedObjectDestroyPacket;
        public static event CallbackEvent OnSyncedObjectInterpolationModePacket;
        public static event CallbackEvent OnSyncedObjectVec2PosUpdatePacket;
        public static event CallbackEvent OnSyncedObjectVec3PosUpdatePacket;
        public static event CallbackEvent OnSyncedObjectRotZUpdatePacket;
        public static event CallbackEvent OnSyncedObjectRotUpdatePacket;
        public static event CallbackEvent OnSyncedObjectVec2ScaleUpdatePacket;
        public static event CallbackEvent OnSyncedObjectVec3ScaleUpdatePacket;
        public static event CallbackEvent OnSyncedObjectVec2PosInterpolationPacket;
        public static event CallbackEvent OnSyncedObjectVec3PosInterpolationPacket;
        public static event CallbackEvent OnSyncedObjectRotZInterpolationPacket;
        public static event CallbackEvent OnSyncedObjectRotInterpolationPacket;
        public static event CallbackEvent OnSyncedObjectVec2ScaleInterpolationPacket;
        public static event CallbackEvent OnSyncedObjectVec3ScaleInterpolationPacket;
        public static event CallbackEvent OnPlayerSpawnedPacket;
        public static event CallbackEvent OnMatchUpdatePacket;
        public static event CallbackEvent OnCountdownPacket;
        public static event CallbackEvent OnBirdDeathPacket;
        public static event CallbackEvent OnPlayerInfoPacket;
        public static event CallbackEvent OnPlayerReadyPacket;

        public static void CallOnConnectedCallbacks(object _param) { if (OnConnected != null) { OnConnected(_param); } }
        public static void CallOnDisconnectedCallbacks(object _param) { if (OnDisconnected != null) { OnDisconnected(_param); } }

        public static void CallOnWelcomePacketCallbacks(object _param) { if (OnWelcomePacket != null) { OnWelcomePacket(_param); } }
        public static void CallOnPingPacketCallbacks(object _param) { if (OnPingPacket != null) { OnPingPacket(_param); } }
        public static void CallOnServerInfoPacketCallbacks(object _param) { if (OnServerInfoPacket != null) { OnServerInfoPacket(_param); } }
        public static void CallOnDisconnectClientPacketCallbacks(object _param) { if (OnDisconnectClientPacket != null) { OnDisconnectClientPacket(_param); } }
        public static void CallOnSyncedObjectInstantiatePacketCallbacks(object _param) { if (OnSyncedObjectInstantiatePacket != null) { OnSyncedObjectInstantiatePacket(_param); } }
        public static void CallOnSyncedObjectDestroyPacketCallbacks(object _param) { if (OnSyncedObjectDestroyPacket != null) { OnSyncedObjectDestroyPacket(_param); } }
        public static void CallOnSyncedObjectInterpolationModePacketCallbacks(object _param) { if (OnSyncedObjectInterpolationModePacket != null) { OnSyncedObjectInterpolationModePacket(_param); } }
        public static void CallOnSyncedObjectVec2PosUpdatePacketCallbacks(object _param) { if (OnSyncedObjectVec2PosUpdatePacket != null) { OnSyncedObjectVec2PosUpdatePacket(_param); } }
        public static void CallOnSyncedObjectVec3PosUpdatePacketCallbacks(object _param) { if (OnSyncedObjectVec3PosUpdatePacket != null) { OnSyncedObjectVec3PosUpdatePacket(_param); } }
        public static void CallOnSyncedObjectRotZUpdatePacketCallbacks(object _param) { if (OnSyncedObjectRotZUpdatePacket != null) { OnSyncedObjectRotZUpdatePacket(_param); } }
        public static void CallOnSyncedObjectRotUpdatePacketCallbacks(object _param) { if (OnSyncedObjectRotUpdatePacket != null) { OnSyncedObjectRotUpdatePacket(_param); } }
        public static void CallOnSyncedObjectVec2ScaleUpdatePacketCallbacks(object _param) { if (OnSyncedObjectVec2ScaleUpdatePacket != null) { OnSyncedObjectVec2ScaleUpdatePacket(_param); } }
        public static void CallOnSyncedObjectVec3ScaleUpdatePacketCallbacks(object _param) { if (OnSyncedObjectVec3ScaleUpdatePacket != null) { OnSyncedObjectVec3ScaleUpdatePacket(_param); } }
        public static void CallOnSyncedObjectVec2PosInterpolationPacketCallbacks(object _param) { if (OnSyncedObjectVec2PosInterpolationPacket != null) { OnSyncedObjectVec2PosInterpolationPacket(_param); } }
        public static void CallOnSyncedObjectVec3PosInterpolationPacketCallbacks(object _param) { if (OnSyncedObjectVec3PosInterpolationPacket != null) { OnSyncedObjectVec3PosInterpolationPacket(_param); } }
        public static void CallOnSyncedObjectRotZInterpolationPacketCallbacks(object _param) { if (OnSyncedObjectRotZInterpolationPacket != null) { OnSyncedObjectRotZInterpolationPacket(_param); } }
        public static void CallOnSyncedObjectRotInterpolationPacketCallbacks(object _param) { if (OnSyncedObjectRotInterpolationPacket != null) { OnSyncedObjectRotInterpolationPacket(_param); } }
        public static void CallOnSyncedObjectVec2ScaleInterpolationPacketCallbacks(object _param) { if (OnSyncedObjectVec2ScaleInterpolationPacket != null) { OnSyncedObjectVec2ScaleInterpolationPacket(_param); } }
        public static void CallOnSyncedObjectVec3ScaleInterpolationPacketCallbacks(object _param) { if (OnSyncedObjectVec3ScaleInterpolationPacket != null) { OnSyncedObjectVec3ScaleInterpolationPacket(_param); } }
        public static void CallOnPlayerSpawnedPacketCallbacks(object _param) { if (OnPlayerSpawnedPacket != null) { OnPlayerSpawnedPacket(_param); } }
        public static void CallOnMatchUpdatePacketCallbacks(object _param) { if (OnMatchUpdatePacket != null) { OnMatchUpdatePacket(_param); } }
        public static void CallOnCountdownPacketCallbacks(object _param) { if (OnCountdownPacket != null) { OnCountdownPacket(_param); } }
        public static void CallOnBirdDeathPacketCallbacks(object _param) { if (OnBirdDeathPacket != null) { OnBirdDeathPacket(_param); } }
        public static void CallOnPlayerInfoPacketCallbacks(object _param) { if (OnPlayerInfoPacket != null) { OnPlayerInfoPacket(_param); } }
        public static void CallOnPlayerReadyPacketCallbacks(object _param) { if (OnPlayerReadyPacket != null) { OnPlayerReadyPacket(_param); } }
    }
}

#endregion
