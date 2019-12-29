using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BloodBond {
    public class ActorState
    {
        int stateStep;
        float time;
        float maxTime;

        public virtual void Update() {
            Debug.Log("actor state");
        }
    }


}

