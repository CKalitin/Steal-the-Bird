using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USNL;

public class PlayerController : MonoBehaviour {
    [SerializeField] private int clientId;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [Space]
    [SerializeField] private float gravitySpeed = 0.05f;
    [Space]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance;
    [SerializeField] private LayerMask groundMask;
    [Space]
    [SerializeField] private Transform[] syncPos;
    [SerializeField] private Transform[] syncRot;

    private Vector3 velocity;
    private bool isGrounded;

    [Header("Other")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerWeaponController playerWeaponController;
    [SerializeField] private USNL.SyncedObject syncedObject;
    [SerializeField] private Health health;

    USNL.ClientInput clientInput;

    private float previousHealth = 0f;

    public int ClientId { get => clientId; set => clientId = value; }
    public PlayerWeaponController PlayerWeaponController { get => playerWeaponController; set => playerWeaponController = value; }
    public SyncedObject SyncedObject { get => syncedObject; set => syncedObject = value; }

    private void Awake() {
        previousHealth = health.CurrentHealth;
    }

    private void Start() {
        clientInput = USNL.InputManager.instance.GetClientInput(clientId);

        USNL.PacketSend.HealthBar(clientId, health.CurrentHealth, health.MaxHealth);
    }

    private void FixedUpdate() {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded & velocity.y < 0) velocity.y = -1f * Time.deltaTime;
        
        Vector3 movementInput = new Vector3();
        
        if (clientInput.GetKey(KeyCode.W)) movementInput.z += 1;
        if (clientInput.GetKey(KeyCode.S)) movementInput.z -= 1;
        if (clientInput.GetKey(KeyCode.D)) movementInput.x += 1;
        if (clientInput.GetKey(KeyCode.A)) movementInput.x -= 1;

        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.z;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        velocity.y -= gravitySpeed * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        for (int i = 0; i < syncPos.Length; i++) {
            syncPos[i].position = transform.position;
        }
    }

    private void Update() {
        if (health.CurrentHealth == previousHealth) return;
        previousHealth = health.CurrentHealth;
        
        if (health.CurrentHealth <= 0) {
            GameController.instance.OnPlayerDeath(ClientId);
            Destroy(transform.parent.gameObject);
        }

        USNL.PacketSend.HealthBar(clientId, health.CurrentHealth, health.MaxHealth);
    }

    public void AimPlayer(Vector3 _lookAt) {
        for (int i = 0; i < syncRot.Length; i++) {
            syncRot[i].LookAt(_lookAt);
            syncRot[i].rotation = Quaternion.Euler(0f, syncRot[i].eulerAngles.y, 0f);
        }
    }
}
