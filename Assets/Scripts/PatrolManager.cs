using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class PatrolManager : MonoBehaviour
    {

        List<PatrolRoute> patrolRoutes = new List<PatrolRoute>();
        EnemyManager enemyManager;


        // Start is called before the first frame update
        private void Awake()
        {
            
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform area = transform.GetChild(i);
                for (int j = 0; j < area.childCount; j++)
                {
                    PatrolRoute route = area.GetChild(j).GetComponent<PatrolRoute>();
                    patrolRoutes.Add(route);
                }
            }

            enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        }
        void Start()
        {
            for (int i = 0; i < patrolRoutes.Count; i++) {
                enemyManager.SpawnEnemyWithRoute(patrolRoutes[i].StartPosition, patrolRoutes[i]);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        

    }
}


