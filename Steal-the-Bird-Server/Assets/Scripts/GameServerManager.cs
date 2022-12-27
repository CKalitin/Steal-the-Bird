using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameServerManager : MonoBehaviour {
    private void Start() {
        USNL.ServerManager.instance.StartServer();
    }
}
