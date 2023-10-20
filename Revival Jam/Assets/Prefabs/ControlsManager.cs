using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Wire;

public class ControlsManager : Singleton<ControlsManager>
{
    [field: SerializeField] public List<Controls> StartingControls { get; private set; } = new();

    public List<Controls> ConnectedControls { get; private set; } = new();
    public enum Controls { Unknown, Walk, Jump, Joust, Dash }

    public List<Wire> Wires { get; private set; } = new();
    public List<ReceptacleObject> Receptacles { get; private set; } = new();

    private void OnEnable()
    {
        ConnectWireCheck += HandleConnectWireCheck;
        GameManager.AfterStateChange += ConnectStartingControls;
    }

    private void OnDisable()
    {
        ConnectWireCheck -= HandleConnectWireCheck;
        GameManager.AfterStateChange -= ConnectStartingControls;

    }

    void AddControls(params Controls[] controlsToAdd)
    {
        foreach (var control in controlsToAdd)
            Debug.Log($"Control Added : {control}");

        ConnectedControls.AddRange(controlsToAdd);
    }

    void RemoveControls(params Controls[] controlsToRemove)
    {
        foreach (var control in controlsToRemove)
        {
            Debug.Log($"Control Removed : {control}");

            ConnectedControls.Remove(control);
        }
    }

    void HandleConnectWireCheck(ChangeControlsEventArgs eventArgs)
    {
        AddControls(eventArgs.ControlToAdd);
        RemoveControls(eventArgs.ControlToRemove);
    }

    void ConnectStartingControls(GameManager.GameState gameState)
    {
        if (!(gameState == GameManager.GameState.StartGame))
            return;

        Debug.Log("Connecting starting controls");

        StartCoroutine(ConnectStartingWires());
        
    }

    public static bool IsControlConnected(Controls controlToCheck)
    {
        if (Instance == null)
            Debug.Log("Instance is null");
        
        bool isControlConnected = (Instance == null || Instance.ConnectedControls.Contains(controlToCheck));

        Debug.Log($"IsControlConnected {isControlConnected}");

        return isControlConnected;
    }

    IEnumerator ConnectStartingWires()
    {
        while (Wires.Count == 0)
        {
            Debug.Log("Waiting for wires to load");
            yield return null;
        }

        for (int i = 0; i < StartingControls.Count(); i++)
        {
            if (i > Wires.Count - 1)
            {
                throw new IndexOutOfRangeException("Not enough wires to comply with starting controls");
            }

            if (StartingControls[i] == Controls.Unknown)
            {
                throw new NotImplementedException("Unknown should not be included in starting wire controls.");
            }

            Wires[i].ManuallyConnect(Receptacles.First(r => r.LinkedControl == StartingControls[i]));

            var control = Receptacles.First(r => r.LinkedControl == StartingControls[i]);
        }
    }
}
