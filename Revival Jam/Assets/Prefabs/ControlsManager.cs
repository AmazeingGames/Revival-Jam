using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Wire;

public class ControlsManager : Singleton<ControlsManager>
{
    [field: SerializeField] public List<Controls> StartingControls { get; private set; } = new();

    public List<Controls> ConnectedControls { get; private set; } = new();
    public enum Controls { Unknown, WalkLeft, WalkRight, Jump, Joust }

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

    // Updates controls when we plug or unplug a wire
    void HandleConnectWireCheck(ChangeControlsEventArgs eventArgs)
    {
        ConnectedControls.Add(eventArgs.ControlToAdd);
        ConnectedControls.Remove(eventArgs.ControlToRemove);
    }

    // When the game starts we need the player to be able to perform basic actions, like walking
    void ConnectStartingControls(GameManager.GameState gameState)
    {
        if (gameState != GameManager.GameState.StartGame)
            return;

        StartCoroutine(ConnectStartingWires());
    }

    IEnumerator ConnectStartingWires()
    {
        // Waits for the scene to load in
        while (Wires.Count == 0)
            yield return null;

        for (int i = 0; i < StartingControls.Count(); i++)
        {
            if (i > Wires.Count - 1)
                throw new IndexOutOfRangeException("Not enough wires to comply with starting controls");

            if (StartingControls[i] == Controls.Unknown)
                throw new NotImplementedException("Starting controls not configured properly.");

            Wires[i].ManuallyConnect(Receptacles.First(r => r.LinkedControl == StartingControls[i]));
        }
    }

    public static bool IsControlConnected(Controls controlToCheck)
        => (Instance == null || Instance.ConnectedControls.Contains(controlToCheck));
}
