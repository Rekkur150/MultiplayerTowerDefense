using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using TMPro;
using Steamworks;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : Character
{

        [Tooltip("If the player can control their player character")]
        [HideInInspector]
        public bool CanPlayerControlCharacter = true;

    [Header("Player Controller")]
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
        private Vector3 GravityVelocity;
        private bool IsGrounded;
        private bool CanJump;

    [SyncVar(hook = nameof(UpdateUsernameText))]
    private string username;
    public TMP_Text usernameText;

    [Header("Character Animation")]
        public Animator CharacterAnimator;

    [Header("Camera")]
        public GameObject Camera;

    void Start()
    {

        if (isServer)
        {
            base.Awake();
        }

        CharacterController = GetComponent<CharacterController>();

        if (hasAuthority)
        {

            if (ClientPlayerManager.singleton == null)
                StartCoroutine("WaitForClientPlayerManager");
            else
                ClientPlayerManager.singleton.PlayerCharacter = this;

            ObjectActivationHelper(true, ClientEnableOnStart);
            ObjectActivationHelper(false, ClientDisableOnStart);


            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            SetUsername(SteamFriends.GetPersonaName());
        }

    }

    private void OnEnable()
    {
        if (hasAuthority)
            ClientPlayerManager.singleton.PlayerCharacter = this;
    }

    protected void Update()
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

    [ClientCallback]
    protected void FixedUpdate()
    {
        if (hasAuthority)
        {
            SetVelocity(CharacterController.velocity);
        }
    }

    protected void MoveCharacter()
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

    protected void ApplyCharacterGravity()
    {
        if (!(IsGrounded && GravityVelocity.y < 0))
        {
            GravityVelocity += Physics.gravity * Time.deltaTime;
            CharacterController.Move(GravityVelocity * Time.deltaTime);
        } else
        {
            GravityVelocity.y = -2f;
        }
    }

    protected void RotateCharacter()
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X"));
    }

    protected void RotateCameraVertically()
    {
        float xRotation = Camera.transform.localRotation.eulerAngles.x;

        xRotation -= Input.GetAxis("Mouse Y");

        Camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    protected void Jump()
    {
        if (CanJump)
        {
            GravityVelocity.y = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y); //TODO: Store this into a variable because this is computationally costly
            CharacterController.Move(GravityVelocity * Time.deltaTime);
        }
    }

    protected void CheckGround()
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
    protected override void Died()
    {
        if (OnDeath != null)
        {
            OnDeath(this, new MyEventArgs());
        }

        ClientPlayerManager.singleton.ClientCharacterDied(gameObject);
    }

    private IEnumerator WaitForClientPlayerManager()
    {
        while (true)
        {
            if (ClientPlayerManager.singleton != null)
            {
                ClientPlayerManager.singleton.PlayerCharacter = this;
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }


    public delegate void MyEventHandler(object source, MyEventArgs e);
    public event MyEventHandler OnDeath;


    public class MyEventArgs : EventArgs
    {

    }

    [Command(requiresAuthority = false)]
    private void SetUsername(string newName)
    {
        username = newName;
    }

    private void UpdateUsernameText(string oldUsername, string newUsername)
    {
        if (usernameText == null)
            return;

        usernameText.text = newUsername;
    }

}
