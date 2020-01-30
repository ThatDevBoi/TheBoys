using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("FieldOfView")]
    public float viewRadius;
    [Range (0, 360)]   // 0 to 360 rotation
    public float viewAngle;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public List<Transform> visableTargets = new List<Transform>();

    [Header("FOV Visualization")]
    public float meshResolution;
    public int edgeResolveInteration;
    public float edgeDistanceThreshold;
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    [Header("Script Reference")]
    [HideInInspector]
    public AI script;

    GameObject player;

    private void Start()
    {
        //script = gameObject.GetComponent<AI>();
        player = GameObject.Find("PC");
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        // Functions
        StartCoroutine("FindTargetWithDelay", 0.2f);
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    // Find objects with a delay
    IEnumerator FindTargetWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    /// <summary>
    /// Find the relevant targets that are in the angle
    /// </summary>
    void FindVisibleTargets()
    {
        if(this.gameObject.name == "Enemy")
        {
            script = this.gameObject.GetComponent<AI>();
            script.PC_in_FOV = false;
        }

        visableTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for(int i =0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);
               
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    visableTargets.Add(target);

                    if (visableTargets.Contains(player.transform))
                    {
                        script.PC_in_FOV = true;
                    }
                    else return;
                }
            }
        }
    }

    void DrawFieldOfView()
    {
        // int of how many angles we will shoot out from our current angle
        // We also need to round the float to a whole number
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        // A list of all the viewcast that will hit something
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        // Loo[ through each of the steps
        for(int i = 0; i <=stepCount; i++)
        {
            // our current angle subtracts the angle 
            // plus the step angle size and multiply the current amount of stepcounts
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if(i > 0)
            {
                bool edgedistThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgedistThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if(edge.PointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointA);
                    }
                    if(edge.PointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointB);
                    }
                }
            }

            // Add positions to the List of possible hit points
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;

        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for(int i = 0; i < vertexCount-1;i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            // Trangles for the mesh will be decinging which angle at what position/point is first
            // Transgles figure out what is where in triangles. So seeing several points and the angle view
            // 0, 1, 2 / 0, 2, 3 / 0, 3, 4
            // 0 is the player the rest are enemies 
            if(i < vertexCount - 2)
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

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 MaxPoint = Vector3.zero;

        for(int i = 0; i < edgeResolveInteration; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgedistThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgedistThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                MaxPoint = newViewCast.point;
            }
        }
        return new EdgeInfo(minPoint, MaxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = directionFromAngle(globalAngle, true);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 directionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        // Triganometry
        // x = angle
        // 90 - x // sin (90 - x) = cos(x)
        // Rotate 360 - 0, 90, 180, 270 {Loops}
        // 90 - 0 = 90 // 90 - 90 = 0 // 90 - 180 = -90 // 90 - 270 = -180
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    /// <summary>
    /// A structure of information will pass into a function for raycasting
    /// </summary>
    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
        {
            hit = _hit;
            point = _point;
            distance = _distance;
            angle = _angle;
        }
    }

    /// <summary>
    /// Holds logic that tells angle to see what obsticles edge is 
    /// </summary>
    public struct EdgeInfo
    {
        public Vector3 PointA;
        public Vector3 PointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            PointA = _pointA;
            PointB = _pointB;
        }
    }
}
