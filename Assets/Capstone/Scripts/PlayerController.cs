using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : Character
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

        public float JumpHeight = 1f;

        [Tooltip("The layers that count as ground")]
        public LayerMask GroundMask;
        [Tooltip("The layers that count as a jumpable surface")]
        public LayerMask JumpMask;

        [Tooltip("Places where we shall check to see if the player is on the floor")]
        public List<Transform> FeetPoints;

        //Private
        private Vector3 Velocity;
        private bool IsGrounded;
        private bool CanJump;

    [Header("Character Animation")]
        public Animator CharacterAnimator;

    [Header("Camera")]
        public GameObject Camera;


    new void Start()
    {

        if (isServer)
        {
            base.Start();
        }

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

                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    RotateCharacter();
                    RotateCameraVertically();
                }
            }

            ApplyCharacterGravity();
            CheckGround();

        }
    }

    private void MoveCharacter()
    {
        float Horizontal = Input.GetAxis("Horizontal");
        float Vertical = Input.GetAxis("Vertical");

        //Updates animator
        CharacterAnimator.SetFloat("Forward", Input.GetAxis("Vertical"));
        CharacterAnimator.SetFloat("Side", Horizontal);

        //Moves the character
        Vector3 direction = transform.right * Horizontal * Strafe + transform.forward * Input.GetAxis("Vertical") * Forward;
        CharacterController.Move(direction * Time.deltaTime);

        //Deals with other types of movement
        if (Input.GetButtonDown("Jump"))
            Jump();

    }

    private void ApplyCharacterGravity()
    {
        if (!(IsGrounded && Velocity.y < 0))
        {
            Velocity += Physics.gravity * Time.deltaTime;
            CharacterController.Move(Velocity * Time.deltaTime);
        } else
        {
            Velocity.y = -2f;
        }
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

    private void Jump()
    {
        if (CanJump)
        {
            Velocity.y = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y); //TODO: Store this into a variable because this is computationally costly
            CharacterController.Move(Velocity * Time.deltaTime);
        }
    }

    private void CheckGround()
    {
        bool temp = false;

        temp = Physics.CheckSphere(transform.position, 0.05f, GroundMask);

        foreach (Transform trans in FeetPoints) {
            if (Physics.CheckSphere(trans.position, 0.05f, GroundMask))
            {
                temp = true;
                break;
            }
        }

        IsGrounded = temp;

        Debug.Log(IsGrounded);

        temp = Physics.CheckSphere(transform.position, 0.05f, JumpMask);

        foreach (Transform trans in FeetPoints)
        {
            if (Physics.CheckSphere(trans.position, 0.05f, JumpMask))
            {
                temp = true;
                break;
            }
        }

        CanJump = temp;

    }

    static void ObjectActivationHelper(bool isEnabled, List<GameObject> gameObjects)
    {
        foreach (GameObject obj in gameObjects)
        {
            obj.SetActive(isEnabled);
        }
    }
}
