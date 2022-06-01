using UnityEngine;
using Mirror;

public class PlayerShoot : NetworkBehaviour
{
    public PlayerWeapons weapon;
    private InputManager inputManager;

    [SerializeField]
    public Camera cam;

    [SerializeField]
    private LayerMask mask;


    void Start()
    {
        if (cam == null)
        {
            print("Pas de camera renseigné sur le yteme de tir");
            this.enabled = false;
        }
    }

    private void Update()
    {
        if (inputManager.Player.Fire.IsPressed())
        {
            Shoot();
        }

    }

    [Client]
    private void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weapon.range, mask))
        {
            if (hit.collider.tag == "player")
            {
                CmdPlayerShot(hit.collider.name, weapon.damage);
            }
        }
    }

    [Command]
    private void CmdPlayerShot(string playerId, float damage)
    {
        print(playerId + " a ete touché.");

        Player player = GameManager.GetPlayer(playerId);
        player.RpcTakeDamage(damage);
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
