using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] private int clientId;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 500f;
    [SerializeField] private float maxSpeed = 5f;

    [Header("Other")]
    [SerializeField] private Rigidbody rb;

    USNL.ClientInput clientInput;

    public int ClientId { get => clientId; set => clientId = value; }

    private void Start() {
        clientInput = USNL.InputManager.instance.GetClientInput(clientId);
    }

    private void FixedUpdate() {
        Vector3 movementInput = new Vector3();
        
        if (clientInput.GetKey(KeyCode.W)) movementInput.z += 1;
        if (clientInput.GetKey(KeyCode.S)) movementInput.z -= 1;
        if (clientInput.GetKey(KeyCode.D)) movementInput.x += 1;
        if (clientInput.GetKey(KeyCode.A)) movementInput.x -= 1;

        rb.AddForce(movementInput * moveSpeed * Time.fixedDeltaTime, ForceMode.Force);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
    }
}
