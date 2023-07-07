using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Transform _parentPos;
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
        transform.position = new Vector3(_parentPos.position.x, _parentPos.position.y + 90f, _parentPos.position.z);
    }
}
