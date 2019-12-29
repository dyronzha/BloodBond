using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BloodBond {
    public class PatrolRoute : MonoBehaviour
    {
        public enum RouteType
        {
            Pingpong, Cycle, Rotate
        }

        [SerializeField]
        private RouteType routeType;

        [SerializeField]
        private int lookRoundNum;
    }
}


