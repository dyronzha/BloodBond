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
        int lastPointID = 0;
        int curPointID = 1;
        public int CurPointID
        {
            get { return curPointID; }
            set {
                lastPointID = curPointID; 
                curPointID = value; 
            }
        }
        int curIDOffset = 0;

        Vector3[] points;
        public PathFinder.Path path;
        public PathFinder.Path reversePath;

        [HideInInspector]
        public Vector3 StartPosition;

        public enum RouteType
        {
            Pingpong, Cycle, Rotate
        }


        public RouteType routeType;

        [HideInInspector]
        public PatrolPoint[] patrolPoints; //單純方便修改的mono，初始化會把資訊統抓出來，destroy掉


        private bool[] lookArounds;
        public bool CurLookAround {
            get { return lookArounds[Mathf.Abs(curIDOffset - curPointID)]; }
        }
        public bool LastLookAround
        {
            get {
                return lookArounds[Mathf.Abs( curIDOffset - lastPointID)];}
        }
        private int[] lookNums;
        public int CurLookNum
        {
            get { return lookNums[Mathf.Abs(curIDOffset - curPointID)]; }
        }
        public int LastLookNum
        {
            get{return lookNums[Mathf.Abs(curIDOffset - lastPointID)]; }
        }
        private Vector3[] lookForwards;
        public Vector3 CurLookForward
        {
            get { return lookForwards[Mathf.Abs(curIDOffset - curPointID)]; }
        }
        public Vector3 LastLookForward
        {
            get{
                return lookForwards[Mathf.Abs(curIDOffset - lastPointID)];}
        }


        [HideInInspector]
        public bool drawOnce = false;

#if UNITY_EDITOR
        //需要重新enable來載入新的點
        private void OnDisable()
        {
            drawOnce = false;
        }
        //bool awake = false;

        //private void Update()
        //{
        //    if (!awake) {
        //        awake = true;
        //        if (transform.childCount > 1)
        //        {
        //            patrolPoints = new PatrolPoint[transform.childCount];
        //            for (int i = 0; i < patrolPoints.Length; i++)
        //            {
        //                patrolPoints[i] = new PatrolPoint();
        //                patrolPoints[i].transform = transform.GetChild(i);
        //            }
        //        }
        //    }
        //    if (patrolPoints != null && patrolPoints.Length > 1)
        //    {
        //        for (int i = 0; i < patrolPoints.Length; i++)
        //        {
        //            if(i < patrolPoints.Length - 1) Debug.DrawLine(patrolPoints[i].transform.position, patrolPoints[i + 1].transform.position, Color.cyan);
        //            if (patrolPoints[i].lookAround) Debug.DrawRay(patrolPoints[i].transform.position, patrolPoints[i].transform.forward, Color.yellow);
        //        }
        //    }
        //}
#endif
        
        //建立路線
        public void Init()
        {
            patrolPoints = new PatrolPoint[transform.childCount];
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                patrolPoints[i] = transform.GetChild(i).GetComponent<PatrolPoint>();
            }
            StartPosition = new Vector3(patrolPoints[0].transform.position.x, transform.position.y, patrolPoints[0].transform.position.z);
            if (routeType == RouteType.Rotate) {
                points = new Vector3[patrolPoints.Length];
                lookArounds = new bool[patrolPoints.Length];
                lookNums = new int[patrolPoints.Length];
                lookForwards = new Vector3[patrolPoints.Length];
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    points[i] = new Vector3(patrolPoints[i].transform.position.x, transform.position.y, patrolPoints[i].transform.position.z);
                    lookArounds[i] = patrolPoints[i].lookAround;
                    lookNums[i] = patrolPoints[i].lookNum;
                    lookForwards[i] = patrolPoints[i].transform.forward;
                }
            }
            else if (routeType == RouteType.Cycle)
            {
                points = new Vector3[patrolPoints.Length+1];
                lookArounds = new bool[patrolPoints.Length + 1];
                lookNums = new int[patrolPoints.Length + 1];
                lookForwards = new Vector3[patrolPoints.Length + 1];
                
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    points[i] = new Vector3(patrolPoints[i].transform.position.x, transform.position.y, patrolPoints[i].transform.position.z);
                    lookArounds[i] = patrolPoints[i].lookAround;
                    lookNums[i] = patrolPoints[i].lookNum;
                    lookForwards[i] = patrolPoints[i].transform.forward;
                    if (lookArounds[i] && lookNums[i] < 1) lookNums[i] = 1;
                }
                points[patrolPoints.Length] = patrolPoints[0].transform.position;
                lookArounds[patrolPoints.Length] = patrolPoints[0].lookAround;
                lookNums[patrolPoints.Length] = patrolPoints[0].lookNum;
                lookForwards[patrolPoints.Length] = patrolPoints[0].transform.forward;

                path = new PathFinder.Path(points, 0.5f);
            }
            else if (routeType == RouteType.Pingpong) {
                points = new Vector3[patrolPoints.Length];
                lookArounds = new bool[patrolPoints.Length];
                lookNums = new int[patrolPoints.Length];
                lookForwards = new Vector3[patrolPoints.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = new Vector3(patrolPoints[i].transform.position.x, transform.position.y, patrolPoints[i].transform.position.z);
                    lookArounds[i] = patrolPoints[i].lookAround;
                    lookNums[i] = patrolPoints[i].lookNum;
                    lookForwards[i] = patrolPoints[i].transform.forward;
                    if (lookArounds[i] && lookNums[i] < 1) lookNums[i] = 1;
                }
                path = new PathFinder.Path(points, 0.5f);
                System.Array.Reverse(points);
                reversePath = new PathFinder.Path(points, 0.5f);
            }
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Destroy(patrolPoints[i]);
            }
#if !UNITY_EDITOR
            for (int i = 0; i < patrolPoints.Length; i++) {
                Destroy(patrolPoints[i]);
            }
#endif

        }

        public bool SetReversePath()
        {
            reverse = !reverse;
            if (reverse) curIDOffset = lookArounds.Length - 1;
            else curIDOffset = 0;
            return reverse;
        }

    }


    //[System.Serializable]
    //public class PatrolPoint
    //{
    //    public bool lookAround;
    //    public Transform transform;

    //}


#if UNITY_EDITOR
    public class MyScriptGizmoDrawer
    {
        [UnityEditor.DrawGizmo(UnityEditor.GizmoType.NonSelected | UnityEditor.GizmoType.Selected | UnityEditor.GizmoType.Active)]
        static void DrawPatrolRouteGizmo(PatrolRoute scr, UnityEditor.GizmoType gizmoType)
        {
            if (Application.isPlaying) return;
            if (!scr.drawOnce)  //初始化參照所有子物件為點
            {
                
                if (scr.transform.childCount > 1)
                {
                    scr.patrolPoints = new PatrolPoint[scr.transform.childCount];
                    for (int i = 0; i < scr.patrolPoints.Length; i++)
                    {
                        scr.patrolPoints[i] = scr.transform.GetChild(i).GetComponent<PatrolPoint>();
                        
                    }
                }
                scr.drawOnce = true;
            }

            if (scr.patrolPoints != null && scr.patrolPoints.Length > 1)
            {
                for (int i = 0; i < scr.patrolPoints.Length; i++)
                {
                    Gizmos.color = Color.cyan;
                    //Debug.Log(i + "    " + scr.patrolPoints[i].name);
                    if (i < scr.patrolPoints.Length - 1) Gizmos.DrawLine(scr.patrolPoints[i].transform.position, scr.patrolPoints[i + 1].transform.position);
                    Gizmos.color = new Color(255, 0, 100);
                    if (scr.patrolPoints[i].lookAround) Gizmos.DrawRay(scr.patrolPoints[i].transform.position, scr.patrolPoints[i].transform.forward);
                }
                if (scr.routeType == PatrolRoute.RouteType.Cycle) {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(scr.patrolPoints[scr.patrolPoints.Length-1].transform.position, scr.patrolPoints[0].transform.position);
                }
            }
        }
    }
#endif
}


