using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectOnStart : MonoBehaviour {

    [Header("Connection")]
    [SerializeField] private int port;
    [SerializeField] private float timeoutDelay;

    [Header("GUI")]
    [SerializeField] private GameObject[] activateOnConnect;
    [SerializeField] private GameObject[] deactivateOnConnect;
    [Space]
    [SerializeField] private GameObject[] activateOnAwake;
    [SerializeField] private GameObject[] deactivateOnAwake;
    [Space]
    [SerializeField] private GameObject[] activateOnTimeout;

    private void Awake() {
        for (int i = 0; i < activateOnAwake.Length; i++)
            activateOnAwake[i].SetActive(true);

        for (int i = 0; i < deactivateOnAwake.Length; i++)
            deactivateOnAwake[i].SetActive(false);
    }

    private void Start() {
        USNL.ClientManager.instance.ConnectToServer(PlayerPrefs.GetInt("HostId"), port);
        StartCoroutine(Timeout());
    }
    
    private void OnEnable() {
        USNL.CallbackEvents.OnConnected += OnConnected;
    }

    private void OnDisable() {
        USNL.CallbackEvents.OnConnected -= OnConnected;
    }

    private void OnConnected(object _object) {
        for (int i = 0; i < activateOnConnect.Length; i++)
            activateOnConnect[i].SetActive(true);

        for (int i = 0; i < deactivateOnConnect.Length; i++)
            deactivateOnConnect[i].SetActive(false);

        USNL.PacketSend.PlayerSetupInfo(PlayerPrefs.GetString("Username", "Username"), PlayerPrefs.GetInt("Character Id", 0));
    }

    private IEnumerator Timeout() {
        yield return new WaitForSeconds(timeoutDelay);
        
        if (USNL.ClientManager.instance.IsConnected) yield break;

        for (int i = 0; i < activateOnTimeout.Length; i++)
            activateOnTimeout[i].SetActive(true);
    }
}
