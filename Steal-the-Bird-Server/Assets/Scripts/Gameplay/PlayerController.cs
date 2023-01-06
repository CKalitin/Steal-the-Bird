using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Vector3 velocity;
    private bool isGrounded;

    [Header("Other")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Health health;

    USNL.ClientInput clientInput;

    private float previousHealth = 0f;

    public int ClientId { get => clientId; set => clientId = value; }

    private void Awake() {
        previousHealth = health.CurrentHealth;
    }

    private void Start() {
        clientInput = USNL.InputManager.instance.GetClientInput(clientId);

        USNL.PacketSend.HealthBar(clientId, clientId, health.CurrentHealth, health.MaxHealth);
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
    }

    private void Update() {
        if (health.CurrentHealth == previousHealth) return;
        previousHealth = health.CurrentHealth;
        
        if (health.CurrentHealth <= 0) {
            GameController.instance.OnPlayerDeath(ClientId);
            Destroy(gameObject);
        }

        USNL.PacketSend.HealthBar(clientId, clientId, health.CurrentHealth, health.MaxHealth);
    }
}
