using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BloodBond {
    public class PatrolRoute : MonoBehaviour
    {
        bool reverse = false;
        public bool Reverse {
            get { return reverse; }
            set { reverse = value; }
        } 
        int curPointID = 1;
        public int CurPointID {
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


        public void Init() {
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
                foreach (Vector3 pp in p) {
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


