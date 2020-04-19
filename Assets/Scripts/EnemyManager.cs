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
        List<EnemyArcher> freeArcherHunterList = new List<EnemyArcher>();
        List<EnemyArcher> usedArcherHunterList = new List<EnemyArcher>();

        PatrolManager patrolManager;

        [SerializeField]
        EnemyValue hunterInfo;
        public EnemyValue HunterValue {
            get { return hunterInfo; }
        }
        [SerializeField]
        EnemyValue archerInfo;
        public EnemyValue ArcherValue
        {
            get { return archerInfo; }
        }

        System.Action LateUpdateAction;
        public void SubLateAction(System.Action action) {
            if (LateUpdateAction != null) LateUpdateAction = action;
            else LateUpdateAction += action;
        }
        public void UnSubLateAction(System.Action action)
        {
            if(LateUpdateAction != null) LateUpdateAction -= action;
        }

        // Start is called before the first frame update
        private void Awake()
        {
            patrolManager = GameObject.Find("PatrolManager").GetComponent<PatrolManager>();

            Transform t = transform.Find("PatrolEnemys");
            for (int i = 0; i < t.childCount; i++) {
                EnemyBase enemy = new EnemyBase(t.GetChild(i), this);
                freeBaseHunterList.Add(enemy);
                enemy.transform.gameObject.SetActive(false);
            }
            t = transform.Find("ArcherEnemys");
            for (int i = 0; i < t.childCount; i++)
            {
                EnemyArcher enemy = new EnemyArcher(t.GetChild(i), this);
                freeArcherHunterList.Add(enemy);
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
            for (int i = usedArcherHunterList.Count - 1; i >= 0; i--) {
                usedArcherHunterList[i].Update(deltaTime);
            }
        }
        private void LateUpdate()
        {
            if(LateUpdateAction != null) LateUpdateAction();
        }

        public EnemyBase SpawnEnemyWithRoute(Vector3 loc, PatrolRoute route, PathFinder.PathFinding finding)
        {
            EnemyBase enemy = freeBaseHunterList[0];
            enemy.transform.position = new Vector3(loc.x, loc.y, loc.z);
            enemy.transform.gameObject.SetActive(true);
            usedBaseHunterList.Add(enemy);
            enemy.SetPatrolArea(route, finding);
            freeBaseHunterList.RemoveAt(0);
            return enemy;
        }
        public EnemyArcher SpawnAcherInLoc(Vector3 loc) {
            EnemyArcher enemy = freeArcherHunterList[0];
            enemy.transform.position = new Vector3(loc.x, loc.y, loc.z);
            enemy.transform.gameObject.SetActive(true);
            usedArcherHunterList.Add(enemy);
            freeArcherHunterList.RemoveAt(0);
            enemy.Init();
            return enemy;
        }


    }
}


