using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorController : PlayerController
{
    [Header("Spectator")]
    public float VerticalSpeed = 5;

    protected new void Update()
    {
        if (hasAuthority)
        {
            if (CanPlayerControlCharacter)
            {
                MoveCharacter();

                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    RotateCharacter();
                    RotateCameraVertically();
                }
            }
        }
    }

    protected new void MoveCharacter()
    {
        float Horizontal = Input.GetAxis("Horizontal");
        float Vertical = Input.GetAxis("Vertical");

        Vector3 direction = transform.right * Horizontal * Strafe + transform.forward * Input.GetAxis("Vertical") * Forward;
        
        if (Input.GetButton("Jump"))
            direction = direction + transform.up * VerticalSpeed;

        if (Input.GetButton("Crouch"))
            direction = direction + transform.up * -VerticalSpeed;

        CharacterController.Move(direction * Time.deltaTime);
    }
}
