using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class EnemyBase
    {
        float deltaTime = .0f;

        float routeEndTime;

        Vector3 selfPos;
        Vector3 moveFwdDir = new Vector3(0,0,0);

        EnemyManager enemyManager;

        Animator animator;

        PathFinder.Path curPath;
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

        public EnemyBase(Transform t, EnemyManager manager) {
            transform = t;
            enemyManager = manager;
            animator = t.GetComponent<Animator>();
        }


        public void SetPatrolArea(PatrolRoute _patrolRoute, PathFinder.PathFinding _pathFinding) {
            pathFinding = _pathFinding;
            patrolRoute = _patrolRoute;

            idleState = new EnemyIdleState(this);
            patrolState = new EnemyPatrolState(this);
            chaseState = new EnemyChaseState(this);
            attackState = new EnemyAttackState(this);
            hurtState = new EnemyHurtState(this);
            yellState = new EnemyYellState(this);
            dieState = new EnemyDieState(this);

            if (patrolRoute.routeType == PatrolRoute.RouteType.Rotate) curState = idleState;
            else curState = patrolState;
        }


        public virtual void Update(float dtTime) {
            deltaTime = dtTime;
            selfPos = transform.position;
            curState.Update(dtTime);
        }

        public void Patroling()
        {
            Vector2 pos2D = new Vector2(selfPos.x, selfPos.z);
            if (patrolRoute.path.turnBoundaries[patrolRoute.CurPointID].HasCrossedLine(pos2D))
            {
                if (patrolRoute.routeType == PatrolRoute.RouteType.Cycle) {
                    animator.SetBool("Look", true);
                    return;
                }

                if (patrolRoute.CurPointID == patrolRoute.path.finishLineIndex) //|| pathIndex >= path.canAttckIndex
                {
                    patrolRoute.CurPointID = 1;
                    if (patrolRoute.routeType == PatrolRoute.RouteType.Pingpong)
                    {
                        animator.SetBool("Look", true);
                        if (!patrolRoute.Reverse)
                        {
                            patrolRoute.Reverse = true;
                            curPath = patrolRoute.reversePath;
                        }
                        else {
                            patrolRoute.Reverse = false;
                            curPath = patrolRoute.path;
                        }
                    } 
                }
                else
                {
                    patrolRoute.CurPointID++;
                    moveFwdDir = new Vector3(curPath.lookPoints[patrolRoute.CurPointID].x - selfPos.x, 0, curPath.lookPoints[patrolRoute.CurPointID].z - selfPos.z).normalized;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.HunterValue.RotateSpeed);
                    
                }
            }
            transform.position += deltaTime * enemyManager.HunterValue.MoveSpeed * moveFwdDir;
        }

    }


}


