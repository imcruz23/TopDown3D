using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSetup : MonoBehaviour
{
    public PlayerController _playerController;
    public PlayerInput _playerInput;
    public FieldOfView _fieldOfView;
    public GameObject _camera;
    public void IsLocalPlayer()
    {
        _playerInput.enabled = true;
        _playerController.enabled = true;
        _fieldOfView.enabled = true;
        _camera.SetActive(true);
    }
}