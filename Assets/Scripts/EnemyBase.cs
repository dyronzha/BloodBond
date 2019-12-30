using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class EnemyBase
    {

        float routeEndTime;

        EnemyManager enemyManager;

        PatrolRoute patrolRoute;
        PathFinder.PathFinding pathFinding;
        public PatrolRoute PatrolRoute {
            get { return patrolRoute; }
        }

        EnemyBaseState curState;
        EnemyIdleState idleState;
        EnemyPatrolState patrolState;
        EnemyChaseState chaseState;
        EnemyAttackState attackState;
        EnemyHurtState hurtState;
        EnemyYellState yellState;
        EnemyDieState dieState;

        public Transform transform;

        public EnemyBase(Transform t) {
            transform = t;
        }


        public void SetPatrolArea(PatrolRoute _patrolRoute, PathFinder.PathFinding _pathFinding) {
            pathFinding = _pathFinding;
            patrolRoute = _patrolRoute;
            if (patrolRoute.routeType == PatrolRoute.RouteType.Rotate) curState = idleState;
            else curState = patrolState;
        }


        public virtual void Update() {
            curState.Update();
        }

        public void Patroling()
        {
            
        }

    }


}


