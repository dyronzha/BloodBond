using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class GameManager : MonoBehaviour
    {
        int Step = 0;
        int nightmareDeadCount = 0;
        float gameOverTime = .0f;
        EnemyManager enemymanager;
        MapInteract mapInteract;

        public Transform door;

        public Animator blackScene;

        private static GameManager singletonInScene;
        public static GameManager SingletonInScene
        {
            get
            {
                return singletonInScene;
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            singletonInScene = this;

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Step == 1)
            {
                door.position += new Vector3(0, -1, 0);
                if (door.position.y < -8.0f)
                {
                    Step++;
                }
            }
            else if (Step == 3)
            {
                gameOverTime += Time.deltaTime;
                if (gameOverTime > 9.0f)
                {
                    Step++;
                    gameOverTime = .0f;
                }
            }
            else if (Step == 4) {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                Player.canControl = true;
            }
        }

        public void CountNightmareDead() {
            nightmareDeadCount++;
            if(nightmareDeadCount > 3)Step = 1;
        }

        public void GameOver() {
            Step = 3;
            blackScene.Play("GameOver");
        }

        public void ReStart() { 
            
        }
    }
}

