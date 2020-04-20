using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinder;

namespace BloodBond {
    public class PatrolManager : MonoBehaviour
    {

        //List<PatrolRoute> patrolRoutes = new List<PatrolRoute>();

        public struct AreaPatrol {
            public PathFinding pathFinding;
            public List<PatrolRoute> patrolRoutes;
            public List<Transform> archerLocs;
            public AreaPatrol(PathFinding finding ) {
                pathFinding = finding;
                patrolRoutes = new List<PatrolRoute>();
                archerLocs = new List<Transform>();
            }
        }

        AreaPatrol[] areaPatrolRoutes;

        EnemyManager enemyManager;


        // Start is called before the first frame update
        private void Awake()
        {
            //subArea數量
            areaPatrolRoutes = new AreaPatrol[transform.childCount];
            for (int i = 0; i < areaPatrolRoutes.Length; i++)
            {
                //subArea
                Transform area = transform.GetChild(i);
                if (!area.gameObject.activeSelf) continue;
                areaPatrolRoutes[i] = new AreaPatrol(area.GetComponent<PathFinding>());

                //subArea內的路線或定點
                for (int j = 0; j < area.childCount; j++)
                {
                    Transform c = area.GetChild(j);
                    if (!c.gameObject.activeSelf) continue;

                    if (c.name.Contains("PatrolRoute")) {
                        PatrolRoute route = c.GetComponent<PatrolRoute>();
                        route.Init();
                        areaPatrolRoutes[i].patrolRoutes.Add(route);
                        //route.gameObject.SetActive(false);
                    }
                    else if (c.name.Contains("ArcherLoc")) {
                        areaPatrolRoutes[i].archerLocs.Add(c);
                    }

                }
            }
            enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        }

        void Start()
        {
            for (int i = 0; i < areaPatrolRoutes.Length; i++) {
                if (!transform.GetChild(i).gameObject.activeSelf) continue;
                for (int j = 0; j < areaPatrolRoutes[i].patrolRoutes.Count; j++) {
                    enemyManager.SpawnEnemyWithRoute(areaPatrolRoutes[i].patrolRoutes[j].StartPosition, areaPatrolRoutes[i].patrolRoutes[j], areaPatrolRoutes[i].pathFinding);
                }
                for (int j = 0; j < areaPatrolRoutes[i].archerLocs.Count; j++)
                {
                    enemyManager.SpawnAcherInLoc(areaPatrolRoutes[i].archerLocs[j].position, areaPatrolRoutes[i].archerLocs[j].forward);
                }

            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        

    }
}


