using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinetAnimator : MonoBehaviour
{
    [Header("Joystick Movement")]
    [SerializeField] Transform joystick;
    [SerializeField] float maxZRotation;
    [SerializeField] float speed;
    [SerializeField] AnimationCurve curve;

    Vector3 maxRotation = new();
    Vector3 minRotation = new();

    float current;
    float target;
    float horizontalInput;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        MoveJoystick();
    }

    void MoveJoystick()
    {

        maxRotation = new Vector3(0, 180, maxZRotation);
        minRotation = new Vector3(0, 180, -maxZRotation);

        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (PlayerFocus.Instance != null && PlayerFocus.Instance.Focused != PlayerFocus.FocusedOn.Arcade)
            horizontalInput = 0;



            target = .5f * (horizontalInput + 1);

        current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
        
        joystick.localRotation = Quaternion.Lerp(Quaternion.Euler(minRotation), Quaternion.Euler(maxRotation), curve.Evaluate(current));
    }


}
