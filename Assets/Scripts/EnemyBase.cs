using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class EnemyBase
    {

        float routeEndTime;
        PatrolRoute.RouteType routeType;
        PathFinder.PathFinding pathFinding;

        public Transform transform;


        public EnemyBase(Transform t) {
            transform = t;
        }

    }




}


