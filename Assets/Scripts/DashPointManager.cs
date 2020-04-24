using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BloodBond
{
    public class DashPointManager : MonoBehaviour
    {
        int currentID = 0;
        Player player;
        public struct PointSet
        {
            public bool isNear;
            public Transform startPoint;
            public Transform goalPoint;
            public Animator[] animator;
            public GameObject[] parti;
            public ParticleSystem[] startParticleSystem;
            public ParticleSystem[] goalParticleSystem;
        }
        PointSet[] pointSet;
        // Start is called before the first frame update
        private void Awake()
        {
            player = GameObject.Find("Karol").GetComponent<Player>();
            pointSet = new PointSet[transform.childCount];
            for (int i = 0; i < pointSet.Length; i++)
            {
                Transform t = transform.GetChild(i);
                pointSet[i] = new PointSet();
                pointSet[i].startPoint = t.GetChild(0);
                pointSet[i].goalPoint = t.GetChild(1);
                pointSet[i].animator = new Animator[2] {
                pointSet[i].startPoint.GetComponent<Animator>(),pointSet[i].goalPoint.GetComponent<Animator>()};

                pointSet[i].parti = new GameObject[2];

                pointSet[i].parti[0] = pointSet[i].startPoint.Find("TeleportSpot").gameObject;
                pointSet[i].startParticleSystem = new ParticleSystem[4];
                pointSet[i].startParticleSystem[0] = pointSet[i].parti[0].transform.GetChild(0).GetComponent<ParticleSystem>();
                pointSet[i].startParticleSystem[1] = pointSet[i].parti[0].transform.GetChild(1).GetComponent<ParticleSystem>();
                pointSet[i].startParticleSystem[2] = pointSet[i].parti[0].transform.GetChild(2).GetComponent<ParticleSystem>();
                pointSet[i].startParticleSystem[3] = pointSet[i].parti[0].transform.GetChild(3).GetComponent<ParticleSystem>();

                pointSet[i].parti[1] = pointSet[i].goalPoint.Find("TeleportSpot").gameObject;
                pointSet[i].goalParticleSystem = new ParticleSystem[4];
                pointSet[i].goalParticleSystem[0] = pointSet[i].parti[1].transform.GetChild(0).GetComponent<ParticleSystem>();
                pointSet[i].goalParticleSystem[1] = pointSet[i].parti[1].transform.GetChild(1).GetComponent<ParticleSystem>();
                pointSet[i].goalParticleSystem[2] = pointSet[i].parti[1].transform.GetChild(2).GetComponent<ParticleSystem>();
                pointSet[i].goalParticleSystem[3] = pointSet[i].parti[1].transform.GetChild(3).GetComponent<ParticleSystem>();

            }
        }
        void Start()
        {

        }


        public void NextTransPort() {
            Debug.Log("neeeeeeeeeeeeeeeeext  trans");
            pointSet[currentID].isNear = false;

            pointSet[currentID].startParticleSystem[0].Stop();
            pointSet[currentID].startParticleSystem[1].Stop();
            pointSet[currentID].startParticleSystem[2].Stop();
            pointSet[currentID].startParticleSystem[3].Stop();

            pointSet[currentID].animator[1].SetTrigger("End");

            currentID++;
        }
        public void LastTransPort()
        {
            Debug.Log("lllllllllllllllllllllllllast  trans");
            pointSet[currentID - 1].isNear = false;

            pointSet[currentID - 1].goalParticleSystem[0].Stop();
            pointSet[currentID - 1].goalParticleSystem[1].Stop();
            pointSet[currentID - 1].goalParticleSystem[2].Stop();
            pointSet[currentID - 1].goalParticleSystem[3].Stop();

            pointSet[currentID - 1].animator[0].SetTrigger("End");

            currentID--;
        }
        // Update is called once per frame
        void Update()
        {
            if (currentID < pointSet.Length)
            {
                Vector2 distV2 = new Vector2(pointSet[currentID].startPoint.position.x - player.transform.position.x, pointSet[currentID].startPoint.position.z - player.transform.position.z);
                if (!pointSet[currentID].isNear)
                {
                    if (distV2.sqrMagnitude <= 2.0f && (Mathf.Abs(pointSet[currentID].startPoint.position.y - player.transform.position.y) < 3.0f))
                    {
                        Debug.Log("close next ");
                        pointSet[currentID].isNear = true;
                        pointSet[currentID].animator[1].Play("Showup");
                        pointSet[currentID].startParticleSystem[0].Play();
                        pointSet[currentID].startParticleSystem[1].Play();
                        pointSet[currentID].startParticleSystem[2].Play();
                        pointSet[currentID].startParticleSystem[3].Play();
                        player.SpecificDash(pointSet[currentID].goalPoint.position, NextTransPort);
                    }
                }
                else {
                    if (distV2.sqrMagnitude > 2.5f || (Mathf.Abs(pointSet[currentID].startPoint.position.y - player.transform.position.y) > 3.0f))
                    {
                        Debug.Log("leave next ");
                        pointSet[currentID].isNear = false;
                        pointSet[currentID].animator[1].SetTrigger("End");

                        pointSet[currentID].startParticleSystem[0].Stop();
                        pointSet[currentID].startParticleSystem[1].Stop();
                        pointSet[currentID].startParticleSystem[2].Stop();
                        pointSet[currentID].startParticleSystem[3].Stop();
                        player.CancleSpecificDash();
                    }
                }
            }
            if (currentID > 0)
            {
                Vector2 distV2 = new Vector2(pointSet[currentID -1 ].goalPoint.position.x - player.transform.position.x, pointSet[currentID - 1].goalPoint.position.z - player.transform.position.z);
                if (!pointSet[currentID - 1].isNear)
                {
                    if (distV2.sqrMagnitude <= 2.0f && (Mathf.Abs(pointSet[currentID - 1].goalPoint.position.y - player.transform.position.y) < 3.0f))
                    {
                        Debug.Log("close last ");
                        pointSet[currentID - 1].isNear = true;
                        pointSet[currentID - 1].animator[0].Play("Showup");
                        pointSet[currentID - 1].goalParticleSystem[0].Play();
                        pointSet[currentID - 1].goalParticleSystem[1].Play();
                        pointSet[currentID - 1].goalParticleSystem[2].Play();
                        pointSet[currentID - 1].goalParticleSystem[3].Play();
                        player.SpecificDash(pointSet[currentID - 1].startPoint.position, LastTransPort);
                    }
                }
                else {
                    if(distV2.sqrMagnitude > 2.5f || (Mathf.Abs(pointSet[currentID - 1].goalPoint.position.y - player.transform.position.y) > 3.0f))
                    {
                        Debug.Log("leave last ");
                        pointSet[currentID - 1].isNear = false;
                        pointSet[currentID - 1].animator[0].SetTrigger("End");

                        pointSet[currentID - 1].goalParticleSystem[0].Stop();
                        pointSet[currentID - 1].goalParticleSystem[1].Stop();
                        pointSet[currentID - 1].goalParticleSystem[2].Stop();
                        pointSet[currentID - 1].goalParticleSystem[3].Stop();
                        player.CancleSpecificDash();
                    }
                }
            }


            //if (currentID < 0)
            //{
            //    for (int i = 0; i < pointSet.Length; i++)
            //    {
            //        Vector2 distV2 = new Vector2(pointSet[i].startPoint.position.x - player.transform.position.x, pointSet[i].startPoint.position.z - player.transform.position.z);
            //        if (distV2.sqrMagnitude <= 2.0f && (Mathf.Abs(pointSet[i].startPoint.position.y - player.transform.position.y) < 0.5f))
            //        {
            //            //pointSet[i].animator[0].Play("Showup");
            //            pointSet[i].animator[1].Play("Showup");
            //            pointSet[i].particleSystem[0].Play();
            //            pointSet[i].particleSystem[1].Play();
            //            currentID = i;
            //            player.SpecificDash(pointSet[i].goalPoint.position);
            //            break;
            //        }
            //    }
            //}
            //else
            //{
            //    Vector2 distV2 = new Vector2(pointSet[currentID].startPoint.position.x - player.transform.position.x, pointSet[currentID].startPoint.position.z - player.transform.position.z);
            //    if (distV2.sqrMagnitude > 2.0f || (Mathf.Abs(pointSet[currentID].startPoint.position.y - player.transform.position.y) >= 0.5f))
            //    {
            //        //pointSet[currentID].animator[0].SetTrigger("End");
            //        pointSet[currentID].animator[1].SetTrigger("End");
            //        pointSet[currentID].particleSystem[0].Stop();
            //        pointSet[currentID].particleSystem[1].Stop();
            //        currentID = -1;
            //        player.CancleSpecificDash();
            //    }
            //}
        }

    }
}
