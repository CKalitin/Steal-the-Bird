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
    [SerializeField] private CharacterController cc;

    USNL.ClientInput clientInput;

    public int ClientId { get => clientId; set => clientId = value; }

    private void Start() {
        clientInput = USNL.InputManager.instance.GetClientInput(clientId);
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
        cc.Move(move * moveSpeed * Time.deltaTime);

        velocity.y -= gravitySpeed * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}