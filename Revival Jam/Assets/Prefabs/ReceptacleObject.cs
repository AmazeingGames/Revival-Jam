using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Wire;
using static ControlsManager;

//[ExecuteInEditMode]
public class ReceptacleObject : MonoBehaviour
{
    [field: SerializeField] public Transform WirePosition { get; private set; }
    [SerializeField] TextMeshProUGUI displayText;
    [field: SerializeField] public Controls LinkedControl { get; private set; }

    private void OnEnable()
    {
        ConnectWireCheck += HandleConnectWireCheck;
    }

    private void OnDisable()
    {
        ConnectWireCheck -= HandleConnectWireCheck;
    }


    private void Awake()
    {
        ControlsManager.Instance.Receptacles.Add(this);
    }

    private void Start()
    {
        ControlsManager.Instance.Receptacles.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        displayText.text = LinkedControl.ToString();
    }

    void HandleConnectWireCheck(ChangeControlsEventArgs eventArgs)
    {
        if (eventArgs.NewWireReceptacle != gameObject)
            return;
    }
}
