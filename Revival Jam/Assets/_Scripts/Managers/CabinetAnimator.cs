using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static ToolManager;
using static ItemAndAbilityManager;

public class CabinetAnimator : MonoBehaviour
{
    //Maybe turn this data into scriptable objects to make it easier to edit
    [Header("Joystick Movement")]
    [SerializeField] Transform joystick;
    [SerializeField] float maxJoystickRotation;
    [SerializeField] float joyStickSpeed;
    [SerializeField] AnimationCurve joystickCurve;

    float joystickCurrent;
    float joystickTarget;
    float horizontalInput;

    [Header("Jammed Button")]
    [SerializeField] Transform button;

    [Header("Wiring Panel")]
    [SerializeField] Transform panel;
    [SerializeField] bool closePanelOnLeave;
    [SerializeField] float maxPanelRotation;
    [SerializeField] float panelPrySpeed = 5f;
    [SerializeField] float panelOpenSpeed;
    [SerializeField] float panelCloseSpeed;

    [SerializeField] AnimationCurve panelCurve;

    float panelCurrent;
    float panelTarget;
    float panelSpeed;

    private void OnEnable()
    {
        Interface.UseItem += HandleUseItem;
        MovementManager.ConnectToStation += HandleConnectToStation;
    }

    private void OnDisable()
    {
        Interface.UseItem -= HandleUseItem;
        MovementManager.ConnectToStation -= HandleConnectToStation;
    }

    // Update is called once per frame
    void Update()
    {
#if DEBUG
        if (Input.GetKeyDown(KeyCode.C))
            SetPanelTarget(0);
        if (Input.GetKeyDown(KeyCode.V))
            SetPanelTarget(1);
#endif
        MoveJoystick();
        RotatePanel();
    }

    //1 = Open
    //0 = Closed
    void SetPanelTarget(float target, bool isPrying = false)
    {
        panelTarget = target;

        panelSpeed = panelTarget > .5f ? panelOpenSpeed : panelCloseSpeed;

        if (isPrying)
            panelSpeed = panelPrySpeed;
    }

    void HandleConnectToStation(PlayerFocus.FocusedOn stationType)
    {
        switch (stationType)
        {
            case PlayerFocus.FocusedOn.Circuitry:
                if (ToolManager.Instance.HasUsedTool(ItemsAndAbilities.Crowbar))
                    SetPanelTarget(1);
                break;
            
            default: 
                if (closePanelOnLeave)
                    SetPanelTarget(0);
                break;
        }
    }

    //To Do: Move this into a generic base function to reduce duplicate code
    //Could turn this into a coroutine instead to improve performance
    void RotatePanel()
    {
        Vector3 maxRotation = new(0, maxPanelRotation, 0);
        Vector3 minRotation = new(0, 0, 0);

        panelCurrent = Mathf.MoveTowards(panelCurrent, panelTarget, panelSpeed * Time.deltaTime);

        panel.localRotation = Quaternion.Lerp(Quaternion.Euler(minRotation), Quaternion.Euler(maxRotation), panelCurve.Evaluate(panelCurrent));
    }

    void MoveJoystick()
    {
        var maxRotation = new Vector3(0, 180, maxJoystickRotation);
        var minRotation = new Vector3(0, 180, -maxJoystickRotation);

        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (PlayerFocus.Instance != null && PlayerFocus.Instance.Focused != PlayerFocus.FocusedOn.Arcade)
            horizontalInput = 0;

        joystickTarget = .5f * (horizontalInput + 1);

        joystickCurrent = Mathf.MoveTowards(joystickCurrent, joystickTarget, joyStickSpeed * Time.deltaTime);
        
        joystick.localRotation = Quaternion.Lerp(Quaternion.Euler(minRotation), Quaternion.Euler(maxRotation), joystickCurve.Evaluate(joystickCurrent));
    }

    void HandleUseItem(ItemData itemData)
    {
        switch (itemData.ItemType)
        {
            case ItemsAndAbilities.Screwdriver:
            case ItemsAndAbilities.Wrench:
                FixButton();
                break;

            case ItemsAndAbilities.Crowbar:
                SetPanelTarget(1, isPrying: true);
                break;
        }
    }

    //When the player fixes the jammed button with the tool, returns it to its normal visuals
    void FixButton()
    {
        button.localRotation = Quaternion.Euler(Vector3.zero);
    }

}
