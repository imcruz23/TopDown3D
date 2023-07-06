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

    public float _meshResolution;
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    public int _edgeResolveIterations;

    public float _edgeDistanceThreshold;

    public float _maskCutawayDist = 0.1f;

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine(nameof(FindTargetsWithDelay), .2f);
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
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
                }
            }
        }
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(_viewAngle * _meshResolution); // Cantidad de rayos
        float stepAngleSize = _viewAngle / stepCount; // Aumento de angulo
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        // Ver los puntos que impactan con algo
        for(int i = 0; i <= stepCount; ++i)
        {
            float angle = transform.eulerAngles.y - _viewAngle/2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            if (i > 0)
            {
                bool edgeDstTreshholdExceeded = Mathf.Abs(oldViewCast._dst - newViewCast._dst) > _edgeDistanceThreshold;
                if(oldViewCast._hit != newViewCast._hit || (oldViewCast._hit && newViewCast._hit && edgeDstTreshholdExceeded))
                {
                    // Buscar edge
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge._pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge._pointA);
                    }
                    if (edge._pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge._pointB);
                    }
                }
            }
            viewPoints.Add(newViewCast._point);
            oldViewCast = newViewCast;
        }

        // Dibujar triangulos
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for(int i = 0; i < vertexCount - 1; ++i) // -1 porque ya se ha puesto uno
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]) + (Vector3.forward * _maskCutawayDist);

            if (i < vertexCount-2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo p_minViewCast, ViewCastInfo p_maxViewCast)
    {
        float minAngle = p_minViewCast._angle;
        float maxAngle = p_maxViewCast._angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for( int i = 0; i < _edgeResolveIterations; ++i)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstTreshholdExceeded = Mathf.Abs(p_minViewCast._dst - newViewCast._dst) > _edgeDistanceThreshold;
            if (newViewCast._hit == p_minViewCast._hit && !edgeDstTreshholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast._point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast._point;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float p_globalAngle)
    {
        Vector3 dir = DirFromAngle(p_globalAngle, true);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, dir, out hit, _viewRadius, _obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, p_globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * _viewRadius, _viewAngle, p_globalAngle);
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

    public struct ViewCastInfo
    {
        public bool _hit;
        public Vector3 _point;
        public float _dst;
        public float _angle;

        public ViewCastInfo(bool p_hit, Vector3 p_point, float p_dst, float p_angle)
        {
            _hit = p_hit;
            _point = p_point;
            _dst = p_dst;
            _angle = p_angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 _pointA;
        public Vector3 _pointB;
        public EdgeInfo(Vector3 p_pointA, Vector3 p_pointB) 
        {
            _pointA = p_pointA;
            _pointB = p_pointB;
        }
    }
}
