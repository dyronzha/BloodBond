using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BloodBond {
    public class PatrolRoute : MonoBehaviour
    {
        int curPointID = 1;
        public int CurPointID {
            get { return curPointID; }
            set { curPointID = value; }
        }
        Vector3[] points;
        public PathFinder.Path path;

        public Vector3 StartPosition{
            get { return points[0]; }
        }

        public enum RouteType
        {
            Pingpong, Cycle, Rotate
        }


        public RouteType routeType;

        [SerializeField]
        private int lookRoundNum;

        private void Awake()
        {
            points = new Vector3[transform.childCount];
            for (int i = 0; i < points.Length; i++) {
                points[i] = transform.GetChild(i).position;
            }
            path = new PathFinder.Path(points, 1.0f);
        }

    }
}


