using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class MapInteract : MonoBehaviour
    {
        int currentID = 0;
        Vector2 playerPosV2;
        Transform player;
        PathFinder.Line[] pointLines;

        [HideInInspector]
        public Drink[] Drinks = new Drink[0];

        [System.Serializable]
        public struct Drink
        {
            public string Name;
            public float Price;
            public Color Color;
            public int aaa;
        }

        [HideInInspector]
        public VCameraPoint[] VCameraPoints = new VCameraPoint[0];

        [System.Serializable]
        public struct VCameraPoint
        {
            public Transform colliderPoint;
            public float distance;
            public GameObject lastVCamera;
            public GameObject nextVCamera;
            public Vector2 positionV2;
            public bool reverse;
        }


        [HideInInspector]
        public interactPoint[] interactPoints = new interactPoint[0];

        [System.Serializable]
        public struct interactPoint
        {
            public Collider colliderPoint;
            public InteractType interactType;
            public UnityEngine.Playables.PlayableDirector timeline;
            public float textShowTime;
            public string infoText;
        }

        // Start is called before the first frame update
        private void Awake()
        {
            player = GameObject.Find("Karol").transform;
            pointLines = new PathFinder.Line[VCameraPoints.Length];
            for (int i = 0; i < pointLines.Length; i++) {
                VCameraPoints[i].positionV2 = new Vector2(VCameraPoints[i].colliderPoint.position.x, VCameraPoints[i].colliderPoint.position.z);
                Vector2 fwdV2 = new Vector2(VCameraPoints[i].colliderPoint.forward.x, VCameraPoints[i].colliderPoint.forward.z);
                pointLines[i] = new PathFinder.Line(VCameraPoints[i].positionV2, VCameraPoints[i].positionV2 - fwdV2);
                
            }
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            playerPosV2 = new Vector2(player.position.x, player.position.z);
            if (currentID < pointLines.Length) {
                Vector2 dif = VCameraPoints[currentID].positionV2 - playerPosV2;
                //判斷下一個攝影機切換點
                //Debug.Log("dif " + Vector2.SqrMagnitude(dif) + "  cross line " + pointLines[currentID].HasCrossedLine(playerPosV2));
                if (Vector2.SqrMagnitude(dif) < VCameraPoints[currentID].distance * VCameraPoints[currentID].distance * 0.25f && CrossLine())   //成0.25是distance要一半0.5*0.5
                {
                    VCameraPoints[currentID].nextVCamera.SetActive(true);
                    VCameraPoints[currentID].lastVCamera.SetActive(false);
                    VCameraPoints[currentID].reverse = true;
                    currentID++;
                }
            }
            //判斷上一個攝影機切換點
            if (currentID > 0) {
                Vector2 dif = VCameraPoints[currentID - 1].positionV2 - playerPosV2;
                //Debug.Log("current id" + currentID + "    dif " + Vector2.SqrMagnitude(dif) + "  cross line " + pointLines[currentID-1].HasCrossedLine(playerPosV2));
                if (Vector2.SqrMagnitude(dif) < VCameraPoints[currentID-1].distance * VCameraPoints[currentID-1].distance*0.25f && CrossLastLine())
                {
                    VCameraPoints[currentID-1].nextVCamera.SetActive(false);
                    VCameraPoints[currentID-1].lastVCamera.SetActive(true);
                    VCameraPoints[currentID - 1].reverse = false;
                    currentID--;
                }
            }
        }

        bool CrossLine() {
            return (VCameraPoints[currentID].reverse != pointLines[currentID].HasCrossedLine(playerPosV2));
        }
        bool CrossLastLine()
        {
            return (VCameraPoints[currentID-1].reverse != pointLines[currentID-1].HasCrossedLine(playerPosV2));
        }

    }

    public enum InteractType
    {
        CameraMove, InfoText, MoveAndText
    }


#if UNITY_EDITOR
    public class MapInteractGizmoDrawer
    {
        [UnityEditor.DrawGizmo(UnityEditor.GizmoType.NonSelected | UnityEditor.GizmoType.Selected | UnityEditor.GizmoType.Active)]
        static void DrawPatrolRouteGizmo(MapInteract scr, UnityEditor.GizmoType gizmoType)
        {
            if (Application.isPlaying) return;
            if (scr.VCameraPoints != null && scr.VCameraPoints.Length > 0)
            {
                for (int i = 0; i < scr.VCameraPoints.Length; i++)
                {
                    if (scr.VCameraPoints[i].colliderPoint != null && scr.VCameraPoints[i].distance > .0f)
                    {
                        Gizmos.color = Color.red;
                        Transform point = scr.VCameraPoints[i].colliderPoint;
                        Gizmos.DrawLine(point.position + 0.5f * scr.VCameraPoints[i].distance * point.right, point.position - 0.5f * scr.VCameraPoints[i].distance * point.right);
                    }
                }
            }
        }
    }
#endif
}

