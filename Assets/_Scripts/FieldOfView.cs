using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float _viewRadius;

    [Range(0,360)]
    public float _viewAngle;

    public LayerMask _targetMask;
    public LayerMask _obstacleMask;

    [HideInInspector]
    public List<Transform> _visibleTargets = new List<Transform>();

    private void Start()
    {
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    IEnumerator FindTargetsWithDelay(float p_delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(p_delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        _visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _targetMask);

        for(int i = 0; i < targetsInViewRadius.Length; ++i)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized; // Vector direccion

            if(Vector3.Angle(transform.forward,dirToTarget) < _viewAngle / 2) // Esta dentro del rango
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if(!Physics.Raycast(transform.position, dirToTarget, distToTarget, _obstacleMask))
                {
                    _visibleTargets.Add(target);
                    Debug.Log("No hay nada en medio");
                }
            }
        }
    }

    /// <summary>
    /// Dado un angulo, se transforma en un vector direccion
    /// </summary>
    /// <param name="p_angle">En angulo que se quiere convertir en Vector. RADIANES</param>
    /// <returns></returns>
    public Vector3 DirFromAngle(float p_angle, bool p_angleIsGlobal)
    {
        if(!p_angleIsGlobal)
            p_angle += transform.eulerAngles.y; // Transformar un angulo local en global

        return new Vector3(Mathf.Sin(p_angle * Mathf.Deg2Rad), 0, Mathf.Cos(p_angle * Mathf.Deg2Rad));
    }
}
