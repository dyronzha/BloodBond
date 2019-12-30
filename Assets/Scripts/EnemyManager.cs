using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class EnemyManager : MonoBehaviour
    {
        Player player;

        List<EnemyBase> freeHunterList = new List<EnemyBase>();
        List<EnemyBase> usedHunterList = new List<EnemyBase>();

        PatrolManager patrolManager;

        // Start is called before the first frame update
        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                EnemyBase enemy = new EnemyBase(transform.GetChild(i));
                freeHunterList.Add(enemy);
                enemy.transform.gameObject.SetActive(false);
            }

            patrolManager = GameObject.Find("PatrolManager").GetComponent<PatrolManager>();

        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public EnemyBase SpawnEnemyWithRoute(Vector3 loc, PatrolRoute route, PathFinder.PathFinding finding)
        {
            EnemyBase enemy = freeHunterList[0];
            enemy.transform.position = loc;
            enemy.transform.gameObject.SetActive(true);
            usedHunterList.Add(enemy);
            enemy.SetPatrolArea(route, finding);
            freeHunterList.RemoveAt(0);
            return enemy;
        }


    }
}


