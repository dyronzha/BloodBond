using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class HunterManager : MonoBehaviour
    {
        List<EnemyBase> freeHunterList, usedHunterList;



        // Start is called before the first frame update
        private void Awake()
        {
            Transform freelist = transform.Find("FreeList");
            for (int i = 0; i < freelist.childCount; i++) {
                EnemyBase enemy = new EnemyBase(freelist.GetChild(i));
                freeHunterList.Add(enemy);
            }
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public EnemyBase SpawnEnemyAtLoc(Vector3 loc) {


            return freeHunterList[0];
        }


    }
}


