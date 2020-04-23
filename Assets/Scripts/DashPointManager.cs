using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BloodBond {
    public class DashPointManager : MonoBehaviour
    {
        int currentID = -1;
        Player player;
        public struct PointSet {
            public Transform startPoint;
            public Transform goalPoint;
            public Animator[] animator;
        }
        PointSet[] pointSet;
        // Start is called before the first frame update
        private void Awake()
        {
            player = GameObject.Find("Karol").GetComponent<Player>();
            pointSet = new PointSet[transform.childCount];
            for (int i = 0; i < pointSet.Length; i++) {
                Transform t = transform.GetChild(i);
                pointSet[i] = new PointSet();
                pointSet[i].startPoint = t.GetChild(0);
                pointSet[i].goalPoint= t.GetChild(1);
                pointSet[i].animator = new Animator[2] { 
                pointSet[i].startPoint.GetComponent<Animator>(),pointSet[i].goalPoint.GetComponent<Animator>()};
            }
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (currentID < 0)
            {
                for (int i = 0; i < pointSet.Length; i++)
                {
                    Vector2 distV2 = new Vector2(pointSet[i].startPoint.position.x - player.transform.position.x, pointSet[i].startPoint.position.z - player.transform.position.z);
                    if (distV2.sqrMagnitude <= 2.0f && (Mathf.Abs(pointSet[i].startPoint.position.y - player.transform.position.y) < 0.5f))
                    {
                        pointSet[i].animator[0].Play("Showup");
                        pointSet[i].animator[1].Play("Showup");
                        currentID = i;
                        player.SpecificDash(pointSet[i].goalPoint.position);
                        break;
                    }
                }
            }
            else {
                Vector2 distV2 = new Vector2(pointSet[currentID].startPoint.position.x - player.transform.position.x, pointSet[currentID].startPoint.position.z - player.transform.position.z);
                if (distV2.sqrMagnitude > 2.0f || (Mathf.Abs(pointSet[currentID].startPoint.position.y - player.transform.position.y) >= 0.5f))
                {
                    pointSet[currentID].animator[0].SetTrigger("End");
                    pointSet[currentID].animator[1].SetTrigger("End");
                    currentID = -1;
                    player.CancleSpecificDash();
                }
            }
        }
    }
}

