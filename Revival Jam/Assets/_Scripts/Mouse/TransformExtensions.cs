using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public enum Axes { Horizontal, Vertical }

    //Updates the Transform along with some movement
    //Either sets the position directly or adds the position
    public static void FollowMovement(this Transform followingTransform, Vector2 movementToFollow, float sensitivity, bool setPosition, out Vector2 AddAmount)
    {
        //Gets the amount to move the transform by
        float addXAmount = (movementToFollow.x / 188) * Time.deltaTime * sensitivity;
        float addYAmount = (movementToFollow.y / 188) * Time.deltaTime * sensitivity;

        AddAmount = new Vector2(addXAmount, addYAmount);

        Vector2 newPosition = new(followingTransform.position.x + addXAmount, followingTransform.position.y + addYAmount);

        //Updates the transform position
        if (setPosition)
            followingTransform.position = new Vector3(newPosition.x, newPosition.y, followingTransform.position.z);
        else
            followingTransform.position += new Vector3(addXAmount, addYAmount, 0);
    }

    public static void ClampToBounds(this Transform transformToClamp, Vector2 positiveBounds, Vector2 negativeBounds)
    {
        Vector3 clampedPosition = transformToClamp.position;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, negativeBounds.x, positiveBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, negativeBounds.y, positiveBounds.y);

        transformToClamp.position = clampedPosition;
    }


    //Gets the [normalized] [raw] input of the mouse
    public static Vector2 GetMouseInput(bool getRawAxis = true, bool normalizeVector = true)
    {
        float mouseX;
        float mouseY;

        if (getRawAxis)
        {
            mouseX = Input.GetAxisRaw("Mouse X");
            mouseY = Input.GetAxisRaw("Mouse Y");
        }
        else
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
        }

        Vector2 mouseInput = new(mouseX, mouseY);

        //Always normalize vector; makes movement s m o o t h
        if (normalizeVector)
            mouseInput = mouseInput.normalized;

        return mouseInput;
    }

    public static Vector2 PositionalDifference(this Transform transform, Vector2 lastPosition) 
    {
        float differenceX = transform.position.x - lastPosition.x;
        float differenceY = transform.position.y - lastPosition.y;

        return new (differenceX, differenceY);
    }
}
