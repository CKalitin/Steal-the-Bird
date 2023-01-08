using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersScriptableObject : MonoBehaviour {
    [SerializeField] private GameObject[] characters;

    public GameObject[] Characters { get => characters; set => characters = value; }
}
