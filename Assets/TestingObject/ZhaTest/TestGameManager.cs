using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BloodBond {
    public class TestGameManager : MonoBehaviour
    {
        InputSystem input;

        public Player player;
        public Transform goal;


        // Start is called before the first frame update
        void Start()
        {
            input = new InputSystem();
        }

        // Update is called once per frame
        void Update()
        {
            if (SceneManager.sceneCountInBuildSettings == 0)
            {
                if (input.GetDodgeInput()) SceneManager.LoadScene(1);
            }
            else if (SceneManager.sceneCountInBuildSettings == 1)
            {
                Vector3 goalPoint = new Vector3(goal.position.x, 0, goal.position.z);
                Vector3 dif = goalPoint - player.transform.position;
                if (Vector3.SqrMagnitude(dif) < 2.0f) {
                    SceneManager.LoadScene(2);
                }
            }
            else if (SceneManager.sceneCountInBuildSettings == 2)
            {
                if (input.GetDodgeInput()) SceneManager.LoadScene(0);
            }
        }
    }
}


