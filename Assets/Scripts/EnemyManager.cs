using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class EnemyManager : MonoBehaviour
    {
        float deltaTime;
        Player player;
        public Player Player { get { return player; } }

        List<EnemyBase> freeBaseHunterList = new List<EnemyBase>();
        List<EnemyBase> usedBaseHunterList = new List<EnemyBase>();
        List<EnemyBase> freeArcherHunterList = new List<EnemyBase>();

        PatrolManager patrolManager;

        [SerializeField]
        EnemyValue hunterInfo;
        public EnemyValue HunterValue {
            get { return hunterInfo; }
        }

        // Start is called before the first frame update
        private void Awake()
        {
            patrolManager = GameObject.Find("PatrolManager").GetComponent<PatrolManager>();
            for (int i = 0; i < transform.childCount; i++)
            {
                EnemyBase enemy = new EnemyBase(transform.Find("PatrolEnemy").GetChild(i), this);
                freeBaseHunterList.Add(enemy);
                enemy.transform.gameObject.SetActive(false);
            }

            

            player = GameObject.Find("Karol").GetComponent<Player>();
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            deltaTime = Time.deltaTime;
            for (int i = usedBaseHunterList.Count-1; i >= 0; i--) {
                usedBaseHunterList[i].Update(deltaTime);
            }
        }

        public EnemyBase SpawnEnemyWithRoute(Vector3 loc, PatrolRoute route, PathFinder.PathFinding finding)
        {
            EnemyBase enemy = freeBaseHunterList[0];
            enemy.transform.position = new Vector3(loc.x, 0, loc.z);
            enemy.transform.gameObject.SetActive(true);
            usedBaseHunterList.Add(enemy);
            enemy.SetPatrolArea(route, finding);
            freeBaseHunterList.RemoveAt(0);
            return enemy;
        }


    }
}


