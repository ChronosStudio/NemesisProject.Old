using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpIntensity = 3f;
    private bool isGrounded = true;
    private Collider _collider;

    [SerializeField] private float mouseSensitivityX = 3f;

    [SerializeField] private float mouseSensitivityY = 3f;

    [SerializeField] private float thrusterForce = 1000;

    [SerializeField] private float thrusterFuelBurnSpeed = 1f;
    [SerializeField] private float thrusterFuelRegenSpeed = 0.3f;
    private float thrusterFuelAmount = 1f;
    CharacterController characterController;
    private bool jetpack = false;
    [SerializeField] private float double_click_time = 0.2f;
    private float lastPressTime;
    private bool jetpackUsable;
    private Vector3 jumpHeight;
    private Vector3 jumpStartPos;

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

    private Animator animator;
    [SerializeField] private Camera cam;
    private float currentCameraRotationX = 0f;
    [SerializeField] private float cameraRotationLimit = 85f;
    private Rigidbody rb;
  

    private InputManager inputManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        IsGrounded();
        //if(PauseMenu.isOn)
        //{
        //    if (Cursor.lockState != CursorLockMode.None)
        //    {
        //        Cursor.lockState = CursorLockMode.None;
        //    }

        //    motor.Move(Vector3.zero);
        //    motor.Rotate(Vector3.zero);
        //    motor.RotateCamera(0f);
        //    motor.ApplyThruster(Vector3.zero);

        //    return;
        //}


        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Calculer la vélocité (vitesse) du mouvement de notre joueur
        float xMov = inputManager.Player.Move.ReadValue<Vector2>().x;
        float zMov = inputManager.Player.Move.ReadValue<Vector2>().y;
        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;
        Vector3 velocity = (moveHorizontal + moveVertical) * speed;

        
        // Calcul de la force du jetpack / thruster
        Vector3 thrusterVelocity = Vector3.zero;
        if (inputManager.Player.Jetpack.ReadValue<float>() != 0 && thrusterFuelAmount > 0)
        {
            if (thrusterFuelAmount >= 0.01f)
            {
                thrusterVelocity = Vector3.up * thrusterForce * 0.01f;
            }
            Jetpack(thrusterVelocity);
        }
        else if (isGrounded)
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

        Move(velocity, thrusterVelocity);

        print("thrusterFuelAmount: "+thrusterFuelAmount*100+"%");

        // On calcule la rotation Y du joueur en un Vector3
        float yRot = inputManager.Player.Rotation.ReadValue<Vector2>().x/20;
        Vector3 rotationY = new Vector3(0, yRot, 0) * mouseSensitivityX;

        // On calcule la rotation X du joueur en un Vector3
        float xRot = inputManager.Player.Rotation.ReadValue<Vector2>().y/20;
        float rotationX = xRot * mouseSensitivityY;
        Rotate(rotationY, rotationX);



    }

    private void Move(Vector3 velocity, Vector3 thrusterVelocity)
    {
        if (velocity != Vector3.zero)
        {
            // Faire sprinter le joueur
            if (inputManager.Player.Sprint.ReadValue<float>() == 1)
            {
                animator.SetFloat("Speed", velocity.z * 4);
                rb.MovePosition(rb.position + velocity * 2 * Time.fixedDeltaTime);
            }
            // Faire avancer le joueur
            else
            {
                animator.SetFloat("Speed", velocity.z * 2);
                rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
            }
        }


        if (inputManager.Player.Jump.triggered)
        {
            if (!jetpackUsable)
            {
                jumpStartPos = rb.position;
            }

            float timeSinceLastPress = Time.time - lastPressTime;
            if (timeSinceLastPress <= double_click_time)
            {
                jetpackUsable = true;
                
            }
            else
            {
                jetpackUsable = false;
                if (!isGrounded)
                {
                    rb.useGravity = true;
                }
                if (isGrounded && inputManager.Player.Jump.triggered)
                {
                    rb.useGravity = true;
                    lastPressTime = Time.time;
                    rb.velocity = Vector3.up * jumpIntensity;
                }
            }
        }
        if (!inputManager.Player.Jump.IsPressed()) 
        {
            rb.useGravity = true;
            jetpack = false;
            jetpackUsable = false;
        }
    }

    private void Jetpack(Vector3 thrusterVelocity)
    {
        if (thrusterVelocity != Vector3.zero && thrusterFuelAmount > 0 && jetpackUsable)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if (inputManager.Player.Jetpack.ReadValue<float>() != 0 && thrusterFuelAmount >0)
            {
                jumpHeight = rb.position - jumpStartPos;
                rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, 0, 9), rb.velocity.z);
                rb.useGravity = false;

                if (jumpHeight.y > 3.5 || jetpack)
                {
                    rb.velocity = Vector3.zero;
                    jetpack = true;
                }
                else if (jumpHeight.y < 3.5)
                {
                    rb.velocity += thrusterVelocity;
                }
            }


        }
        else
        {
            rb.useGravity = true;
            
        }

    }
    private void Rotate(Vector3 RotationY, float RotationX)
    {
        // On calcule la rotation de la caméra
        rb.MoveRotation(rb.rotation * Quaternion.Euler(RotationY));
        currentCameraRotationX -= RotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        // On applique la rotation de la caméra
        cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    // Vérifier si le personnage touche le sol
    private void IsGrounded()
    {
        float distToGround;
        distToGround = _collider.bounds.extents.y;
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, (float)(distToGround + 0.1));
    }

    private void Awake()
    {
        inputManager = new InputManager();
    }
    private void OnEnable()
    {
        inputManager.Player.Enable();
    }
    private void OnDisable()
    {
        inputManager.Player.Disable();
    }

}
