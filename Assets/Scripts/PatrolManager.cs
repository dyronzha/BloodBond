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
            public AreaPatrol(PathFinding finding ) {
                pathFinding = finding;
                patrolRoutes = new List<PatrolRoute>();
            }
        }

        AreaPatrol[] areaPatrolRoutes;

        EnemyManager enemyManager;


        // Start is called before the first frame update
        private void Awake()
        {
            areaPatrolRoutes = new AreaPatrol[transform.childCount];
            for (int i = 0; i < areaPatrolRoutes.Length; i++)
            {
                Transform area = transform.GetChild(i);
                areaPatrolRoutes[i] = new AreaPatrol(area.GetComponent<PathFinding>());
                for (int j = 0; j < area.childCount; j++)
                {
                    areaPatrolRoutes[i].patrolRoutes.Add(area.GetChild(j).GetComponent<PatrolRoute>());
                }
            }
            enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        }

        void Start()
        {
            for (int i = 0; i < areaPatrolRoutes.Length; i++) {
                for (int j = 0; j < areaPatrolRoutes[i].patrolRoutes.Count; j++) {
                    enemyManager.SpawnEnemyWithRoute(areaPatrolRoutes[i].patrolRoutes[i].StartPosition, areaPatrolRoutes[i].patrolRoutes[j], areaPatrolRoutes[i].pathFinding);
                }
                
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        

    }
}


