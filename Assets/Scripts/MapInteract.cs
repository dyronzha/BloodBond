using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class MapInteract : MonoBehaviour
    {
        int cameraCurrentID = 0, currentInteractID, progressID = 0;
        Vector2 playerPosV2;
        Transform player;
        PathFinder.Line[] cameraPointLines, progressPointLine;
        public DialogueManager _dialoguemanager;
        public MainEventIO _maineventio;
        public RandomEventIO _randomeventio;

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
            public float heightY;
        }


        [HideInInspector]
        public interactPoint[] interactPoints = new interactPoint[0];

        [System.Serializable]
        public struct interactPoint
        {
            public BoxCollider colliderPoint;
            public InteractType interactType;
            public UnityEngine.Playables.PlayableDirector timeline;
            public float textShowTime;
            public string infoText;
        }


        public ProgressPoint[] ProgressPoints = new ProgressPoint[0];
        [System.Serializable]
        public struct ProgressPoint
        {
            public Transform Point;
            public int progressID;
            public float distance;
        }

        EnemyManager enemyManager;

        // Start is called before the first frame update
        private void Awake()
        {
            player = GameObject.Find("Karol").transform;
            cameraPointLines = new PathFinder.Line[VCameraPoints.Length];
            for (int i = 0; i < cameraPointLines.Length; i++) {
                VCameraPoints[i].positionV2 = new Vector2(VCameraPoints[i].colliderPoint.position.x, VCameraPoints[i].colliderPoint.position.z);
                VCameraPoints[i].heightY = VCameraPoints[i].colliderPoint.position.y;
                Vector2 fwdV2 = new Vector2(VCameraPoints[i].colliderPoint.forward.x, VCameraPoints[i].colliderPoint.forward.z);
                cameraPointLines[i] = new PathFinder.Line(VCameraPoints[i].positionV2, VCameraPoints[i].positionV2 - fwdV2);
                
            }

            progressPointLine = new PathFinder.Line[ProgressPoints.Length];
            for (int i = 0; i < progressPointLine.Length; i++) {
                Vector2 v2 = new Vector2(ProgressPoints[i].Point.position.x, ProgressPoints[i].Point.position.z);
                Vector2 fwdV2 = new Vector2(ProgressPoints[i].Point.forward.x, ProgressPoints[i].Point.forward.z);
                progressPointLine[i] = new PathFinder.Line(v2, (v2 - fwdV2));
            }
            enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();

        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Vector3 playerPos = player.position;
            playerPosV2 = new Vector2(player.position.x, player.position.z);

            //進度點判斷
            if (progressID < ProgressPoints.Length){
                Vector2 diffProgress = new Vector2(ProgressPoints[progressID].Point.position.x - playerPosV2.x, ProgressPoints[progressID].Point.position.z - playerPosV2.y);
                if (diffProgress.sqrMagnitude < 12.0f && progressPointLine[progressID].HasCrossedLine(playerPosV2) && Mathf.Abs(playerPos.y - ProgressPoints[progressID].Point.position.y) < 2.0f)
                {
                    Debug.Log(ProgressPoints[progressID].Point.name);
                    enemyManager.SetActiveArea(ProgressPoints[progressID].progressID);
                    
                    progressID++;
                    if (progressID == 1) AudioManager.SingletonInScene.ChangeBGM(1);
                    else if (progressID == 2) AudioManager.SingletonInScene.ChangeBGM(2);
                    else if (progressID == 3) AudioManager.SingletonInScene.ChangeBGM(3);
                }
            }



            if (cameraCurrentID < cameraPointLines.Length) {
                Vector2 dif = VCameraPoints[cameraCurrentID].positionV2 - playerPosV2;
                //判斷下一個攝影機切換點
                //Debug.Log("dif " + Vector2.SqrMagnitude(dif) + "  cross line " + cameraPointLines[cameraCurrentID].HasCrossedLine(playerPosV2));
                if (Vector2.SqrMagnitude(dif) < VCameraPoints[cameraCurrentID].distance * VCameraPoints[cameraCurrentID].distance * 0.25f && 
                    Mathf.Abs(playerPos.y - VCameraPoints[cameraCurrentID].heightY) < 0.5f && CrossCameraLine())   //成0.25是distance要一半0.5*0.5
                {
                    VCameraPoints[cameraCurrentID].nextVCamera.SetActive(true);
                    VCameraPoints[cameraCurrentID].lastVCamera.SetActive(false);
                    VCameraPoints[cameraCurrentID].reverse = true;
                    cameraCurrentID++;
                }
            }
            //判斷上一個攝影機切換點
            if (cameraCurrentID > 0) {
                Vector2 dif = VCameraPoints[cameraCurrentID - 1].positionV2 - playerPosV2;
                //Debug.Log("current id" + cameraCurrentID + "    dif " + Vector2.SqrMagnitude(dif) + "  cross line " + cameraPointLines[cameraCurrentID-1].HasCrossedLine(playerPosV2));
                if (Vector2.SqrMagnitude(dif) < VCameraPoints[cameraCurrentID-1].distance * VCameraPoints[cameraCurrentID-1].distance*0.25f &&
                    Mathf.Abs(playerPos.y - VCameraPoints[cameraCurrentID-1].heightY) < 0.5f && CrossCameraLastLine())
                {
                    VCameraPoints[cameraCurrentID-1].nextVCamera.SetActive(false);
                    VCameraPoints[cameraCurrentID-1].lastVCamera.SetActive(true);
                    VCameraPoints[cameraCurrentID - 1].reverse = false;
                    cameraCurrentID--;
                }
            }

            //主要劇情(暫停操作+下方大面板)
            if (interactPoints[currentInteractID].interactType == InteractType.InfoText && interactPoints[currentInteractID].colliderPoint != null) {
                Vector3 point = interactPoints[currentInteractID].colliderPoint.transform.position;
                BoxCollider collider = interactPoints[currentInteractID].colliderPoint;
                if ((playerPos.x <= point.x + collider.size.x * 0.5f) && (playerPos.x >= point.x - collider.size.x * 0.5f) &&
                    (playerPos.y <= point.y + collider.size.y * 0.5f) && (playerPos.y >= point.y - collider.size.y * 0.5f) &&
                     (playerPos.z <= point.z + collider.size.z * 0.5f) && (playerPos.z >= point.z - collider.size.z * 0.5f)
                    )
                {
                    //if(interactPoints[currentInteractID].interactType == InfoText)//關玩家操作
                    _dialoguemanager.StartDialogue();
                    collider.enabled = false;
                    currentInteractID++;
                }
            }

            //主要事件(可以操作+左側小面板)
            if (interactPoints[currentInteractID].interactType == InteractType.MainText && interactPoints[currentInteractID].colliderPoint != null){

                Vector3 point = interactPoints[currentInteractID].colliderPoint.transform.position;
                BoxCollider collider = interactPoints[currentInteractID].colliderPoint;
                if ((playerPos.x <= point.x + collider.size.x * 0.5f) && (playerPos.x >= point.x - collider.size.x * 0.5f) &&
                    (playerPos.y <= point.y + collider.size.y * 0.5f) && (playerPos.y >= point.y - collider.size.y * 0.5f) &&
                     (playerPos.z <= point.z + collider.size.z * 0.5f) && (playerPos.z >= point.z - collider.size.z * 0.5f)
                    )
                {
                    _maineventio.TriggerMainEvent();
                    collider.enabled = false;
                    currentInteractID++;
                }
            }

        }

        bool CrossCameraLine() {
            return (VCameraPoints[cameraCurrentID].reverse != cameraPointLines[cameraCurrentID].HasCrossedLine(playerPosV2));
        }
        bool CrossCameraLastLine()
        {
            return (VCameraPoints[cameraCurrentID-1].reverse != cameraPointLines[cameraCurrentID-1].HasCrossedLine(playerPosV2));
        }

    }

    public enum InteractType
    {
        CameraMove, InfoText, MainText, RandomText
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

            if (scr.ProgressPoints != null && scr.ProgressPoints.Length > 0)
            {
                for (int i = 0; i < scr.ProgressPoints.Length; i++)
                {
                    if (scr.ProgressPoints[i].Point!= null)
                    {
                        Gizmos.color = Color.red;
                        Transform point = scr.ProgressPoints[i].Point;
                        Gizmos.DrawLine(point.position + 0.5f * 10.0f * point.right, point.position - 0.5f * 10.0f * point.right);
                    }
                }
            }
        }
    }
#endif
}

