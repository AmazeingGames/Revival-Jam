using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class ReceptacleObject : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI displayText;
    [field: SerializeField] public Control LinkedControl { get; private set; }

    public enum Control { Walk, Jump, Joust, Dash }

    public static event Action<Control> OnConnection;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.IsPlaying(gameObject))
        {
            displayText.text = LinkedControl.ToString();
        }
    }
}
