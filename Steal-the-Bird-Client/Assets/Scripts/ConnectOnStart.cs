using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectOnStart : MonoBehaviour {

    [Header("Connection")]
    [SerializeField] private int port;
    [SerializeField] private float timeoutDelay;

    [Header("GUI")]
    [SerializeField] private GameObject[] activateOnTimeout;
    
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
        USNL.PacketSend.PlayerSetupInfo(PlayerPrefs.GetString("Username", "Username"), PlayerPrefs.GetInt("Character Id", 0));
    }

    private IEnumerator Timeout() {
        yield return new WaitForSeconds(timeoutDelay);
        
        if (USNL.ClientManager.instance.IsConnected) yield break;

        for (int i = 0; i < activateOnTimeout.Length; i++)
            activateOnTimeout[i].SetActive(true);
    }
}
