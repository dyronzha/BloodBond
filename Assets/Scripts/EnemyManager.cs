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
        List<EnemyArrow> freeEnemyArrowList = new List<EnemyArrow>();
        List<EnemyArrow> usedEnemyArrowList = new List<EnemyArrow>();
        Dictionary<string, EnemyArrow> arrowDic = new Dictionary<string, EnemyArrow>();
        List<EnemyNightmare> freeNightmarerList = new List<EnemyNightmare>();
        List<EnemyNightmare> usedNightmareList = new List<EnemyNightmare>();
        Dictionary<string, EnemyBase> enemyDic = new Dictionary<string, EnemyBase>();

        int areaCount = 0;
        List<EnemyBase>[] enemyArea;
        List<EnemyBase> currentAreaEnemy;
        PatrolManager.AreaPatrol curArea;

        PatrolManager patrolManager;

        bool allAlarm = false;
        int enemyDeadNum = 0;

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
        [SerializeField]
        EnemyValue nightmareInfo;
        public EnemyValue NightmareValue
        {
            get { return nightmareInfo; }
        }

        [SerializeField]
        ObjectValue enemyArrowInfo;
        public ObjectValue EnemyArrowValue
        {
            get { return enemyArrowInfo; }
        }

        Dictionary<string, System.Action> actionDIcs = new Dictionary<string, System.Action>();
        public void SubLateAction(string name, System.Action action) {
            if (!actionDIcs.ContainsKey(name)) {
                actionDIcs.Add(name, action);
            }
        }
        public void UnSubLateAction(string name)
        {
            Debug.Log("remove action  " + name);
            if (actionDIcs.ContainsKey(name)) {
                actionDIcs.Remove(name);
                Debug.Log("scusssssssssssss  ");
            }
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
                enemyDic.Add(enemy.transform.name, enemy);
            }
            t = transform.Find("ArcherEnemys");
            for (int i = 0; i < t.childCount; i++)
            {
                EnemyArcher enemy = new EnemyArcher(t.GetChild(i), this);
                freeArcherHunterList.Add(enemy);
                enemy.transform.gameObject.SetActive(false);
                enemyDic.Add(enemy.transform.name, enemy);
            }
            t = transform.Find("Arrows");
            for (int i = 0; i < t.childCount; i++) {
                EnemyArrow arrow = new EnemyArrow(t.GetChild(i), this);
                freeEnemyArrowList.Add(arrow);
                arrow.transform.gameObject.SetActive(false);
                arrowDic.Add(arrow.transform.name, arrow);
            }
            t = transform.Find("NightmareEnemys");
            for (int i = 0; i < t.childCount; i++)
            {
                EnemyNightmare enemy = new EnemyNightmare(t.GetChild(i), this);
                freeNightmarerList.Add(enemy);
                enemy.transform.gameObject.SetActive(false);
                enemyDic.Add(enemy.transform.name, enemy);
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
            //for (int i = usedBaseHunterList.Count-1; i >= 0; i--) {
            //    usedBaseHunterList[i].Update(deltaTime);
            //}
            //for (int i = usedNightmareList.Count - 1; i >= 0; i--)
            //{
            //    usedNightmareList[i].Update(deltaTime);
            //}
            //for (int i = usedArcherHunterList.Count - 1; i >= 0; i--) {
            //    usedArcherHunterList[i].Update(deltaTime);
            //}
            if (currentAreaEnemy != null) {
                for (int i = 0; i < currentAreaEnemy.Count; i++)
                {
                    currentAreaEnemy[i].Update(deltaTime);
                }
                for (int i = usedEnemyArrowList.Count - 1; i >= 0; i--)
                {
                    usedEnemyArrowList[i].Update(deltaTime);
                }
                //清除搜尋路後的權重
                curArea.pathFinding.ClearGridExtendPenalty();
            }

            if (Input.GetKeyDown(KeyCode.Space)) ResetEnemy();
        }
        private void LateUpdate()
        {
            foreach (KeyValuePair<string, System.Action> item in actionDIcs)
            {
                item.Value();
            }

        }

        public void CreateArea(int length) {
            enemyArea = new List<EnemyBase>[length];
        }
        public void AddNewArea(int id) {
            areaCount = id;
            enemyArea[id] = new List<EnemyBase>();
            currentAreaEnemy = enemyArea[id];
        }
        public void SetActiveArea(int id, PatrolManager.AreaPatrol area) {
            currentAreaEnemy = enemyArea[id];
            curArea = area;
        }
        public void EnemyDead(EnemyBase enemy) {
            if (currentAreaEnemy.Contains(enemy))
            {
                enemyDeadNum++;
                if (enemyDeadNum >= currentAreaEnemy.Count - 1) { 
                    //區域結束
                }
            }
        }

        public EnemyBase SpawnEnemyWithRoute(PatrolRoute.EnemyType type, Vector3 loc, PatrolRoute route, PathFinder.PathFinding finding, float height)
        {
            if (type == PatrolRoute.EnemyType.Hunter)
            {
                EnemyBase enemy;
                enemy = freeBaseHunterList[0];
                enemy.transform.position = new Vector3(loc.x, height, loc.z);
                enemy.transform.gameObject.SetActive(true);
                enemy.HeightY = height;
                //usedBaseHunterList.Add(enemy);
                currentAreaEnemy.Add(enemy);
                Debug.Log("current enemy list " + currentAreaEnemy.Count);
                enemy.SetPatrolArea(route, finding);
                freeBaseHunterList.RemoveAt(0);
                return enemy;
            }
            else {
                EnemyNightmare enemy;
                enemy = freeNightmarerList[0];
                enemy.transform.position = new Vector3(loc.x, height, loc.z);
                enemy.transform.gameObject.SetActive(true);
                enemy.HeightY = height;
                //usedNightmareList.Add(enemy);
                currentAreaEnemy.Add(enemy);
                Debug.Log("current enemy list " + currentAreaEnemy.Count);
                enemy.SetPatrolArea(route, finding);
                freeNightmarerList.RemoveAt(0);
                return enemy;
            } 
            
        }
        public void ResetEnemy() {
            for (int i = 0; i < currentAreaEnemy.Count; i++)
            {
                currentAreaEnemy[i].Reset();
            }
        }
        public EnemyArcher SpawnAcherInLoc(Vector3 loc, Vector3 dir, float height) {
            EnemyArcher enemy = freeArcherHunterList[0];
            enemy.transform.position = new Vector3(loc.x, height, loc.z);
            enemy.transform.rotation = Quaternion.LookRotation(dir);
            enemy.transform.gameObject.SetActive(true);
            enemy.HeightY = height;
            //usedArcherHunterList.Add(enemy);
            currentAreaEnemy.Add(enemy);
            Debug.Log("current enemy list " + currentAreaEnemy.Count);
            freeArcherHunterList.RemoveAt(0);
            enemy.Init();
            return enemy;
        }

        public void SetAllEnemyAlarm(EnemyBase enemy) {
            
            if (currentAreaEnemy.Contains(enemy)) {
                for (int i = 0; i < currentAreaEnemy.Count; i++)
                {
                    if (enemy.transform.name.CompareTo(currentAreaEnemy[i].transform.name) != 0)
                    {
                        currentAreaEnemy[i].AllAlarm();
                    }
                }
            }

        }

        public int GetArrowNum() {
            return freeEnemyArrowList.Count;
        }
        public EnemyArrow SpawnArrow(Vector3 pos, Vector3 dir) {
            EnemyArrow arrow = freeEnemyArrowList[0];
            arrow.transform.position = pos;
            arrow.SetFly(dir);
            arrow.transform.gameObject.SetActive(true);
            usedEnemyArrowList.Add(arrow);
            freeEnemyArrowList.RemoveAt(0);
            return arrow;
        }
        public EnemyBase FindEnemyInDic(string name) {
            if (enemyDic.ContainsKey(name))
            {
                return enemyDic[name];
            }
            else return null;
        }
        public EnemyArrow FindArrowInDic(string name) {
            if (arrowDic.ContainsKey(name))
            {
                return arrowDic[name];
            }
            else return null;
        }
        public void RecycleArrow(EnemyArrow arrow) {
            if (usedEnemyArrowList.Contains(arrow)) {
                arrow.transform.gameObject.SetActive(false);
                freeEnemyArrowList.Add(arrow);
                usedEnemyArrowList.Remove(arrow);
            }

        }
    }
}


