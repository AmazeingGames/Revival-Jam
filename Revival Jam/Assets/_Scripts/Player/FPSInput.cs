using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FocusStation;

[RequireComponent(typeof(UnityEngine.CharacterController))]
[AddComponentMenu("Controls Script/Fps Input")]

public class FPSInput : StaticInstance<FPSInput> 
{
    [Header("Walk")]
    [SerializeField] float speed = 8f;
    [SerializeField] float gravity = -9f;

    [Header("Sound FX")]
    [SerializeField] float timeBetweenWalkSounds;

    float walkSoundTimer;
    float horizontalInput;
    float verticalInput;

    Vector3 movement;

    float deltaX;
    float deltaZ;

    private UnityEngine.CharacterController charController;
    [SerializeField] GameObject controlsPanel;
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
        CheckWalkSound();

        UpdateTimers();
        GetInput();
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!controlsPanel)
            {
                controlsPanel = GameObject.Find("Controls Panel");
            }

            if (!controlsPanel.activeSelf)
                controlsPanel.SetActive(true);
            else controlsPanel.SetActive(false);

            //if (Time.timeScale != 0) Time.timeScale = 0;
            //else if (Time.timeScale != 1) Time.timeScale = 1;
        }

    }

    void UpdateTimers()
    {
        walkSoundTimer -= Time.deltaTime;
    }

    void CheckWalkSound()
    {
        if (Mathf.Abs(horizontalInput) < .1f && Mathf.Abs(verticalInput) < .1f)
            return;

        if (Mathf.Abs(deltaX) < .1f && Mathf.Abs(deltaZ) < .1f)
            return;

        if (walkSoundTimer > 0)
            return;

        //CheckDebug(Mathf.Abs(rigidbody.velocity.x));

        PlayWalkSound();
    }

    void PlayWalkSound()
    {
        if (AudioManager.Instance == null)
            return;

        walkSoundTimer = timeBetweenWalkSounds;
        AudioManager.Instance.TriggerAudioClip(AudioManager.EventSounds.Player3DFootsteps, transform);
    }

    void MovePlayer()
    {
        int walk = CanWalk ? 1 : 0;

        deltaX = horizontalInput * speed * walk;
        deltaZ = verticalInput * speed * walk;

        movement = new(deltaX, 0, deltaZ);
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
