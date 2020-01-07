using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class EnemyManager : MonoBehaviour
    {
        float deltaTime;
        Player player;
        public Player Player { get { return player; } }

        List<EnemyBase> freeHunterList = new List<EnemyBase>();
        List<EnemyBase> usedHunterList = new List<EnemyBase>();

        PatrolManager patrolManager;

        [SerializeField]
        EnemyValue hunterInfo;
        public EnemyValue HunterValue {
            get { return hunterInfo; }
        }

        // Start is called before the first frame update
        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                EnemyBase enemy = new EnemyBase(transform.GetChild(i), this);
                freeHunterList.Add(enemy);
                enemy.transform.gameObject.SetActive(false);
            }

            patrolManager = GameObject.Find("PatrolManager").GetComponent<PatrolManager>();

            player = GameObject.Find("Karol").GetComponent<Player>();
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            deltaTime = Time.deltaTime;
            for (int i = usedHunterList.Count-1; i >= 0; i--) {
                usedHunterList[i].Update(deltaTime);
            }
        }

        public EnemyBase SpawnEnemyWithRoute(Vector3 loc, PatrolRoute route, PathFinder.PathFinding finding)
        {
            EnemyBase enemy = freeHunterList[0];
            enemy.transform.position = new Vector3(loc.x, 0, loc.z);
            enemy.transform.gameObject.SetActive(true);
            usedHunterList.Add(enemy);
            enemy.SetPatrolArea(route, finding);
            freeHunterList.RemoveAt(0);
            return enemy;
        }


    }
}


