using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCastTest : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Vector3.Angle(new Vector3(100,100,100), new Vector3(100,0,-100)));
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red, 5.0f);
        if (Physics.Raycast(transform.position, transform.forward, 5.0f)) {
            Debug.Log("ray cast");
        }
    }
}
