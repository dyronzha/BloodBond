using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour{

    int index;
    //public Animator BlackPanel;

    void Start(){
        index = SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadNextScene() {
        index++;
        if (index > 2) index = 0;
        SceneManager.LoadScene(index);
    }
}
