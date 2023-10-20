using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FocusStation;

[RequireComponent(typeof(UnityEngine.CharacterController))]
[AddComponentMenu("Controls Script/Fps Input")]

public class FPSInput : StaticInstance<FPSInput> 
{
    public float speed = 8f;
    public float gravity = -9f;

    private UnityEngine.CharacterController charController;

    public bool CanWalk { get; private set; } = true;

    private void OnEnable()
    {
        ConnectToStation += HandleConnectToStation;
    }

    private void OnDisable()
    {
        ConnectToStation -= HandleConnectToStation;
    }

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<UnityEngine.CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();  
    }

    void MovePlayer()
    {
        int walk = CanWalk ? 1 : 0;

        float deltaX = Input.GetAxis("Horizontal") * speed * walk;
        float deltaZ = Input.GetAxis("Vertical") * speed * walk;

        Vector3 movement = new(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed);

        movement.y = gravity;
        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);

        charController.Move(movement);
    }

    void HandleConnectToStation(ConnectEventArgs eventArgs)
    {
        CanWalk = !eventArgs.IsConnecting;
    }
}
