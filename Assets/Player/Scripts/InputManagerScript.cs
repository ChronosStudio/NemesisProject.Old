using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerScript : MonoBehaviour
{
    private InputManager inputManager;

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
