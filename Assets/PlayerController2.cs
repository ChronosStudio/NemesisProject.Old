using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    //Camera
    public Camera playerCamera;

    //Composant qui permet de faire bouger le joueur
    CharacterController characterController;

    //Vitesse de marche
    public float walkingSpeed = 7.5f;

    //Vitesse de course
    public float runningSpeed = 15f;

    //Vitesse de saut
    public float jumpSpeed = 8f;

    //Gravité
    [SerializeField]
    public float gravity = 20f;

    //Déplacement
    Vector3 moveDirection;

    //Marche ou court ?
    private bool isRunning = false;

    //Rotation de la caméra
    float rotationX = 0;
    public float rotationSpeed = 2.0f;
    public float rotationXLimit = 45.0f;


    // Start is called before the first frame update
    void Start()
    {
        //Cache le curseur de la souris
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Calcule les directions
        //forward = avant/arrière
        //right = droite/gauche
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        //Est-ce qu'on appuie sur un bouton de direction ?

        // Z = axe arrière/avant
        float speedZ = Input.GetAxis("Vertical");

        // X = axe gauche/droite
        float speedX = Input.GetAxis("Horizontal");

        // Y = axe haut/bas
        float speedY = moveDirection.y;


        //Est-ce qu'on appuie sur le bouton pour courir (ici : Shift Gauche) ?
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //En train de courir
            isRunning = true;
        }
        else
        {
            //En train de marcher
            isRunning = false;
        }

        // Est-ce que l'on court ?
        if (isRunning)
        {
            //Multiplie la vitesse par la vitesse de course
            speedX = speedX * runningSpeed;
            speedZ = speedZ * runningSpeed;
        }
        else
        {
            //Multiplie la vitesse par la vitesse de marche
            speedX = speedX * walkingSpeed;
            speedZ = speedZ * walkingSpeed;
        }



        //Calcul du mouvement
        //forward = axe arrière/avant
        //right = axe gauche/droite
        moveDirection = forward * speedZ + right * speedX;


        // Est-ce qu'on appuie sur le bouton de saut (ici : Espace)
        if (Input.GetButton("Jump") && characterController.isGrounded)
        {

            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = speedY;
        }


        //Si le joueur ne touche pas le sol
        if (!characterController.isGrounded)
        {
            //Applique la gravité * deltaTime
            //Time.deltaTime = Temps écoulé depuis la dernière frame
            moveDirection.y -= gravity * Time.deltaTime;
        }


        //Applique le mouvement
        characterController.Move(moveDirection * Time.deltaTime);



        //Rotation de la caméra

        //Input.GetAxis("Mouse Y") = mouvement de la souris haut/bas
        //On est en 3D donc applique ("Mouse Y") sur l'axe de rotation X 
        rotationX += -Input.GetAxis("Mouse Y") * rotationSpeed;

        //La rotation haut/bas de la caméra est comprise entre -45 et 45 
        //Mathf.Clamp permet de limiter une valeur
        //On limite rotationX, entre -rotationXLimit et rotationXLimit (-45 et 45)
        rotationX = Mathf.Clamp(rotationX, -rotationXLimit, rotationXLimit);


        //Applique la rotation haut/bas sur la caméra
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);


        //Input.GetAxis("Mouse X") = mouvement de la souris gauche/droite
        //Applique la rotation gauche/droite sur le Player
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * rotationSpeed, 0);
    }
}