using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSelection : MonoBehaviour {
    [SerializeField] private bool active = false;
    [Space]
    [SerializeField] private GameObject togglableIsReady;
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private GameObject menuParent;
    [Space]
    [SerializeField] private GameObject[] deactivateIfNotActive;

    int characterId = 0;

    bool ready = false;

    public GameObject TogglableIsReady { get => togglableIsReady; set => togglableIsReady = value; }
    public TextMeshProUGUI UsernameText { get => usernameText; set => usernameText = value; }

    private void Awake() {
        Toggle(false);
    }

    public void OnReadyButton() {
        if (!active) return;
        
        ready = !ready;
        togglableIsReady.SetActive(ready);
        USNL.PacketSend.PlayerReady(ready);
    }

    public void OnCharacterLeftButton() {
        if (!active) return;
        
        characterId--;
        if (characterId < 0) characterId = LobbyController.instance.TotalCharacters - 1;
        USNL.PacketSend.PlayerSetupInfo(PlayerPrefs.GetString("Username", "Username"), characterId);
    }

    public void OnCharacterRightButton() {
        if (!active) return;
        
        characterId++;
        if (characterId >= LobbyController.instance.TotalCharacters) characterId = 0;
        USNL.PacketSend.PlayerSetupInfo(PlayerPrefs.GetString("Username", "Username"), characterId);
    }

    public void Toggle(bool _toggle) {
        if (_toggle) {
            menuParent.SetActive(true);
            
            for (int i = 0; i < deactivateIfNotActive.Length; i++)
                deactivateIfNotActive[i].SetActive(true);
        } else {
            usernameText.text = "";
            menuParent.SetActive(false);
            
            for (int i = 0; i < deactivateIfNotActive.Length; i++)
                deactivateIfNotActive[i].SetActive(false);
        }
    }
}
