using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [Header("Player Controller")]
    [Tooltip("If the player can control their player character")]
    [HideInInspector]
    public bool CanPlayerControlCharacter = true;

    [Tooltip("A collection of local gameobjects to set to enable on start")]
    public List<GameObject> ClientEnableOnStart = new List<GameObject>();
    [Tooltip("A collection of local gameobjects to set to disable on start ")]
    public List<GameObject> ClientDisableOnStart = new List<GameObject>();

    [Header("Character Movement")]
    [HideInInspector]
    public CharacterController CharacterController;

    public float Forward = 1.5f;
    public float Strafe = 1.5f;

    private Vector3 Velocity;

    [Header("Character Animation")]
    public Animator CharacterAnimator;

    [Header("Camera")]
    public GameObject Camera;

    [System.NonSerialized]
    public bool isReady;

    void Start()
    {
        CharacterController = GetComponent<CharacterController>();

        if (hasAuthority)
        {
            ObjectActivationHelper(true, ClientEnableOnStart);
            ObjectActivationHelper(false, ClientDisableOnStart);


            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }
    }

    void Update()
    {
        if (hasAuthority)
        {
            if (CanPlayerControlCharacter)
            {
                MoveCharacter();

                PlayerInput();

                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    RotateCharacter();
                    RotateCameraVertically();
                }
            }
        }
    }

    private void MoveCharacter()
    {
        float Horizontal = Input.GetAxis("Horizontal");
        float Vertical = Input.GetAxis("Vertical");

        CharacterAnimator.SetFloat("Forward", Input.GetAxis("Vertical"));
        CharacterAnimator.SetFloat("Side", Horizontal);

        Vector3 direction = transform.right * Horizontal * Strafe + transform.forward * Input.GetAxis("Vertical") * Forward;
        CharacterController.Move(direction * Time.deltaTime);
    }

    private void RotateCharacter()
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X"));
    }

    private void RotateCameraVertically()
    {
        float xRotation = Camera.transform.localRotation.eulerAngles.x;

        xRotation -= Input.GetAxis("Mouse Y");

        Camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    static void ObjectActivationHelper(bool isEnabled, List<GameObject> gameObjects)
    {
        foreach (GameObject obj in gameObjects)
        {
            obj.SetActive(isEnabled);
        }
    }

    private void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.X) && !isReady)
        {
            isReady = true;
            WaveManager.singleton.ReadyPlayer();
        }

        if (Input.GetKeyDown(KeyCode.X) && isReady)
        {
            isReady = false;
            WaveManager.singleton.UnreadyPlayer();
        }
    }
}
