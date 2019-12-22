using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BloodBond {
    public class ActorState
    {
        int stateStep;
        float time;
        float maxTime;

        public void Update() { 
        
        }
    }

    public class PlayerState : ActorState { 
    
    }

    public class EnemyState : ActorState { 
        
    }

}

