using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButton : MonoBehaviour {
    public void LoadMainMenu() {
        SceneLoader.instance.LoadMainMenu();
    }
}
