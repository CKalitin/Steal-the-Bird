using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParent : MonoBehaviour {
    [SerializeField] PlayerController playerController;

    public PlayerController PlayerController { get => playerController; set => playerController = value; }
}
