using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BloodBond {
    [ExecuteInEditMode]
    public class PatrolRoute : MonoBehaviour
    {
        bool reverse = false;
        public bool Reverse
        {
            get { return reverse; }
            set { reverse = value; }
        }
        int curPointID = 1;
        public int CurPointID
        {
            get { return curPointID; }
            set { curPointID = value; }
        }
        Vector3[] points;
        public PathFinder.Path path;
        public PathFinder.Path reversePath;

        public Vector3 StartPosition;

        public enum RouteType
        {
            Pingpong, Cycle, Rotate
        }


        public RouteType routeType;
        [SerializeField]
        private int lookRoundNum;
        public int LookRoundNum { get { return lookRoundNum; } }

        [SerializeField]
        public PatrolPoint[] patrolPoints;

#if UNITY_EDITOR
        //void OnDrawGizmosSelected()
        //{
        //    if (!pointOnce) {
        //        pointTransforms = new Transform[transform.childCount];

        //        for (int i = 0; i < pointTransforms.Length; i++)
        //        {
        //            pointTransforms[i] = transform.GetChild(i);
        //        }
        //        pointOnce = true;
        //    }

        //    if (pointTransforms != null && pointTransforms.Length > 1)
        //    {

        //        Gizmos.color = Color.blue;
        //        for (int i = 0; i < pointTransforms.Length - 1; i++)
        //        {
        //            Gizmos.DrawLine(pointTransforms[i].position, pointTransforms[i + 1].position);
        //        }
        //    }
        //}
        bool awake = false;
        
        private void Update()
        {
            if (!awake) {
                awake = true;
                if (transform.childCount > 1)
                {
                    patrolPoints = new PatrolPoint[transform.childCount];
                    for (int i = 0; i < patrolPoints.Length; i++)
                    {
                        patrolPoints[i] = new PatrolPoint();
                        patrolPoints[i].transform = transform.GetChild(i);
                    }
                }
            }
            if (patrolPoints != null && patrolPoints.Length > 1)
            {
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    if(i < patrolPoints.Length - 1) Debug.DrawLine(patrolPoints[i].transform.position, patrolPoints[i + 1].transform.position, Color.cyan);
                    if (patrolPoints[i].lookAround) Debug.DrawRay(patrolPoints[i].transform.position, patrolPoints[i].transform.forward, Color.yellow);
                }
            }
        }
#else
        private void Awake()
        {
            if (transform.childCount > 1)
            {
                patrolPoints = new PatrolPoint[transform.childCount];
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    patrolPoints[i] = new PatrolPoint();
                    patrolPoints[i].transform = transform.GetChild(i);
                }
            }
        }

#endif

        public void Init()
        {
            points = new Vector3[transform.childCount];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = transform.GetChild(i).position;
            }
            StartPosition = points[0];

            if (routeType == RouteType.Cycle)
            {
                Vector3[] p = new Vector3[points.Length + 1];
                points.CopyTo(p, 0);
                p[p.Length - 1] = points[0];
                path = new PathFinder.Path(p, 1.0f);
                foreach (Vector3 pp in p)
                {
                    Debug.Log(pp);
                }
            }
            else if (routeType == RouteType.Pingpong)
            {
                foreach (Vector3 pp in points)
                {
                    Debug.Log(pp);
                }
                path = new PathFinder.Path(points, 1.0f);
                System.Array.Reverse(points);
                reversePath = new PathFinder.Path(points, 1.0f);

            }
        }





    }
}

[System.Serializable]
public class PatrolPoint {
    public bool lookAround;
    public Transform transform;

}
