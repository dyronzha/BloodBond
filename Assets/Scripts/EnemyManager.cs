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
            Transform freelist = transform.Find("FreeList");
            for (int i = 0; i < freelist.childCount; i++)
            {
                EnemyBase enemy = new EnemyBase(freelist.GetChild(i));
                freeHunterList.Add(enemy);
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

            return freeHunterList[0];
        }


    }
}


