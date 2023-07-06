using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private float _attackRate = 1f;
    private float _timeSinceShot;

    private void Update()
    {
        _timeSinceShot += Time.deltaTime;
    }
    public void Attack()
    {
        if(_timeSinceShot >= _attackRate)
        {
            _timeSinceShot = 0;
            Debug.Log("PIUM");
        }
    }
}
