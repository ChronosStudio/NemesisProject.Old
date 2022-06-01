using UnityEngine;
using Mirror;

public class PlayerShoot : NetworkBehaviour
{
    public PlayerWeapons weapon;

    [SerializeField]
    public Camera cam;

    [SerializeField]
    private LayerMask mask;


    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("Pas de camera renseigné sur le yteme de tir");
            this.enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
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
            if(hit.collider.tag == "player")
            {
                CmdPlayerShot(hit.collider.name, weapon.damage);
            }
        }
    }

    [Command]
    private void CmdPlayerShot(string playerId, float damage)
    {
        Debug.Log(playerId + " a ete touché.");

        Player player = GameManager.GetPlayer(playerId);
        player.RpcTakeDamage(damage);
    }
}
