using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinder;

namespace BloodBond {
    public class PatrolManager : MonoBehaviour
    {

        //List<PatrolRoute> patrolRoutes = new List<PatrolRoute>();

        public struct AreaPatrol {
            public string name;
            public PathFinding pathFinding;
            public List<PatrolRoute> patrolRoutes;
            public List<Transform> archerLocs;
            public float HeightY;
            public AreaPatrol(PathFinding finding, float height, string n) {
                pathFinding = finding;
                patrolRoutes = new List<PatrolRoute>();
                archerLocs = new List<Transform>();
                HeightY = height;
                name = n;
            }
        }

        List<AreaPatrol> areaPatrolRoutes;

        EnemyManager enemyManager;
        AreaPatrol curArea;

        // Start is called before the first frame update
        private void Awake()
        {
            //subArea數量
            areaPatrolRoutes = new List<AreaPatrol>();
            for (int i = 0; i < transform.childCount; i++)
            {
                //subArea
                Transform area = transform.GetChild(i);
                if (!area.gameObject.activeSelf) continue;
                areaPatrolRoutes.Add(new AreaPatrol(area.GetComponent<PathFinding>(), area.position.y, area.name));
                //areaPatrolRoutes[i] = new AreaPatrol(area.GetComponent<PathFinding>());

                //subArea內的路線或定點
                for (int j = 0; j < area.childCount; j++)
                {
                    Transform c = area.GetChild(j);
                    if (!c.gameObject.activeSelf) continue;

                    if (c.name.Contains("PatrolRoute")) {
                        PatrolRoute route = c.GetComponent<PatrolRoute>();
                        route.Init();
                        areaPatrolRoutes[areaPatrolRoutes.Count-1].patrolRoutes.Add(route);
                        //route.gameObject.SetActive(false);
                    }
                    else if (c.name.Contains("ArcherLoc")) {
                        areaPatrolRoutes[areaPatrolRoutes.Count - 1].archerLocs.Add(c);
                    }

                }
            }
            enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        }

        void Start()
        {
            if (areaPatrolRoutes.Count > 0) {

                List<EnemyBase> enemyArea = new List<EnemyBase>();
                List<AreaPatrol> patrolArea = new List<AreaPatrol>();
                for (int i = 0; i < areaPatrolRoutes.Count; i++)
                {
                    patrolArea.Add(areaPatrolRoutes[i]);   
                    for (int j = 0; j < areaPatrolRoutes[i].patrolRoutes.Count; j++)
                    {
                        EnemyBase enemy = enemyManager.SpawnEnemyWithRoute(areaPatrolRoutes[i].patrolRoutes[j].enemyType, areaPatrolRoutes[i].patrolRoutes[j].StartPosition, areaPatrolRoutes[i].patrolRoutes[j], areaPatrolRoutes[i].pathFinding, areaPatrolRoutes[i].HeightY);
                        enemy.AreaNmae = areaPatrolRoutes[i].name;
                        enemyArea.Add(enemy);
                    }
                    for (int j = 0; j < areaPatrolRoutes[i].archerLocs.Count; j++)
                    {
                        EnemyBase enemy = enemyManager.SpawnAcherInLoc(areaPatrolRoutes[i].archerLocs[j].position, areaPatrolRoutes[i].archerLocs[j].forward, areaPatrolRoutes[i].HeightY);
                        enemy.AreaNmae = areaPatrolRoutes[i].name;
                        enemyArea.Add(enemy);
                    }

                    if (i == 2 || i == 3)   
                    {
                        enemyManager.CreateArea(enemyArea, patrolArea);
                        enemyArea = new List<EnemyBase>();
                        patrolArea = new List<AreaPatrol>();
                    } 
                }
                //將manager目前的區域變回0
                enemyManager.SetActiveArea(0);
            }
           
        }

        // Update is called once per frame
        void Update()
        {

        }

        

    }
}


