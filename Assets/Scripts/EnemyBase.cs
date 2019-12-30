﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class EnemyBase
    {
        int hp;

        bool findingPath = false;

        int stateStep = 0;
        float stateTime = .0f, deltaTime = .0f;

        float idleTime = .0f;

        int lookARoundNum = 0;

        Vector3 selfPos;
        Vector3 moveFwdDir = new Vector3(0, 0, 0);
        Vector3 lookDir, lookPos;

        EnemyManager enemyManager;

        CapsuleCollider hurtAreaCollider;

        Transform head;
        Animator animator;

        int playerPathIndex = 0;
        PathFinder.Path curPath, playerPath;
        PatrolRoute patrolRoute;
        PathFinder.PathRequestManager.PathRequest curPathRequest;
        PathFinder.PathFinding pathFinding;
        public PatrolRoute PatrolRoute {
            get { return patrolRoute; }
        }

        EnemyBaseState curState;
        EnemyIdleState idleState;
        EnemyPatrolState patrolState;
        EnemyLookAroundState lookAroundState;
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
            hurtAreaCollider = t.GetComponent<CapsuleCollider>();
            hp = enemyManager.HunterValue.Health;
        }


        public void SetPatrolArea(PatrolRoute _patrolRoute, PathFinder.PathFinding _pathFinding) {
            pathFinding = _pathFinding;
            patrolRoute = _patrolRoute;

            idleState = new EnemyIdleState(this);
            patrolState = new EnemyPatrolState(this);
            lookAroundState = new EnemyLookAroundState(this);
            chaseState = new EnemyChaseState(this);
            attackState = new EnemyAttackState(this);
            hurtState = new EnemyHurtState(this);
            yellState = new EnemyYellState(this);
            dieState = new EnemyDieState(this);

            curPath = patrolRoute.path;
            if (patrolRoute.routeType == PatrolRoute.RouteType.Rotate) curState = idleState;
            else {
                curState = patrolState;
                animator.SetBool("Patrol", true);
                moveFwdDir = new Vector3(curPath.lookPoints[patrolRoute.CurPointID].x - selfPos.x, 0, curPath.lookPoints[patrolRoute.CurPointID].z - selfPos.z).normalized;
                transform.rotation = Quaternion.LookRotation(moveFwdDir);
            }

            head = transform.Find("mixamorig:Hips").GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0);
            lookDir = head.forward;
            lookPos = transform.position + new Vector3(0, 1.3f, 0);
        }


        public virtual void Update(float dtTime) {
            deltaTime = dtTime;
            selfPos = transform.position;
            lookDir = head.forward;
            lookPos = selfPos + new Vector3(0, 1.3f, 0);
            curState.Update();
        }

        void ChangeState(EnemyBaseState state) {
            stateStep = 0;
            stateTime = .0f;
            curState = state;
        }

        public bool FindPlayer()
        {
            if (!findingPath && Physics.Raycast(lookPos, lookDir, 5.0f, 1 << LayerMask.NameToLayer("Player")))
            {
                findingPath = true;
                curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, enemyManager.Player.transform.position, OnPathFound);
                //animator.SetBool("Chase", true);
                ChangeState(chaseState);
                return true;
            }
            else return false;
        }

        public virtual void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
        {
            curPathRequest = null;
            if (pathSuccessful)
            {
                if (curState != chaseState) {
                    ChangeState(chaseState);
                }
                animator.SetBool("Chase", true);
                playerPath = new PathFinder.Path(waypoints, selfPos, 0.5f);
                playerPathIndex = 0;
                moveFwdDir = new Vector3(playerPath.lookPoints[playerPathIndex].x - selfPos.x, 0, playerPath.lookPoints[playerPathIndex].z - selfPos.z).normalized;
                stateStep = 1;

                //StopCoroutine("FollowPath");
                //StartCoroutine("FollowPath");
            }
            else
            {
                ChangeState(idleState);
            }
        }

        public void SetAniBool(string name, bool value) {
            animator.SetBool(name, value);
        }

        public void Idle() {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateStep == 0)
            {
                if (aniInfo.IsName("Idle")) stateStep++;
                idleTime = Random.Range(1.5f, 2.5f);
            }
            else
            {
                stateTime += deltaTime;
                if (stateTime > idleTime) {
                    animator.SetBool("Look", true);
                    ChangeState(lookAroundState);
                }
            }
        }

        public void LookAround() {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateStep == 0)
            {
                if (aniInfo.IsName("LookRound")) stateStep++;
            }
            else {
                if (aniInfo.normalizedTime > patrolRoute.LookRoundNum * 0.9f) {
                    animator.SetBool("Look", false);

                    if (patrolRoute.routeType == PatrolRoute.RouteType.Rotate) ChangeState(idleState);
                    else {
                        ChangeState(patrolState);
                        animator.SetBool("Patrol", true);
                    }
                }
            }
        }

        public void Patroling()
        {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateStep == 0)
            {
                if (aniInfo.IsName("Patrol")) {
                    stateStep++;
                    moveFwdDir = new Vector3(curPath.lookPoints[patrolRoute.CurPointID].x - selfPos.x, 0, curPath.lookPoints[patrolRoute.CurPointID].z - selfPos.z).normalized;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.HunterValue.RotateSpeed);
                }
            }
            else
            {
                Vector2 pos2D = new Vector2(selfPos.x, selfPos.z);
                if (curPath.turnBoundaries[patrolRoute.CurPointID].HasCrossedLine(pos2D))
                {

                    if (patrolRoute.CurPointID == patrolRoute.path.finishLineIndex) //|| pathIndex >= path.canAttckIndex
                    {
                        patrolRoute.CurPointID = 1;
                        animator.SetBool("Look", true);
                        ChangeState(lookAroundState);
                        if (patrolRoute.routeType == PatrolRoute.RouteType.Pingpong)
                        {


                            if (!patrolRoute.Reverse)
                            {
                                patrolRoute.Reverse = true;
                                curPath = patrolRoute.reversePath;
                            }
                            else
                            {
                                patrolRoute.Reverse = false;
                                curPath = patrolRoute.path;
                            }
                        }
                    }
                    else
                    {
                        patrolRoute.CurPointID++;
                        moveFwdDir = new Vector3(curPath.lookPoints[patrolRoute.CurPointID].x - selfPos.x, 0, curPath.lookPoints[patrolRoute.CurPointID].z - selfPos.z).normalized;
                        if (patrolRoute.routeType == PatrolRoute.RouteType.Cycle)
                        {
                            animator.SetBool("Look", true);
                            ChangeState(lookAroundState);
                        }
                    }
                }
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.HunterValue.RotateSpeed);
                transform.position += deltaTime * enemyManager.HunterValue.MoveSpeed * transform.forward;

            }
        }

        public void Chasing() {
            if (stateStep == 1)
            {
                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (aniInfo.IsName("Chase")) stateStep++;
                Debug.Log("chase  1");
            }
            else if (stateStep == 2)
            {
                Debug.Log("chase  2");
                stateTime += deltaTime;
                if (stateTime > 0.5f)
                {
                    curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, enemyManager.Player.transform.position, OnPathFound);
                }
                if (playerPathIndex == playerPath.finishLineIndex) //|| pathIndex >= path.canAttckIndex
                {
                    stateStep = 3;
                    return;
                }
                else
                {
                    playerPathIndex++;
                    moveFwdDir = new Vector3(playerPath.lookPoints[playerPathIndex].x - selfPos.x, 0, playerPath.lookPoints[playerPathIndex].z - selfPos.z).normalized;
                }
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.HunterValue.RotateSpeed);
                transform.position += deltaTime * enemyManager.HunterValue.MoveSpeed * 2.0f * transform.forward;
            }
            else {
                stateTime += deltaTime;
                if (stateTime > 0.5f)
                {
                    curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, enemyManager.Player.transform.position, OnPathFound);
                }
            }
        }

        public bool CheckGetHurt() {

            Vector3 center = hurtAreaCollider.center;
            Vector3 point2 = transform.position + center.x * transform.right + center.z * transform.forward;
            Vector3 point1 = point2 + new Vector3(0, hurtAreaCollider.height, 0);
            Debug.DrawLine(point1, point2, Color.red);
            Collider[] cols = Physics.OverlapCapsule(point1, point2, hurtAreaCollider.radius, enemyManager.HunterValue.HurtAreaLayer);
            if (cols != null && cols.Length > 0)
            {
                hp -= 10;
                ChangeState(hurtState);
                animator.SetBool("Hurt", true);
                return true;
            }

            return false;
        }

        public void InHurt() {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateStep == 0)
            {
                if (aniInfo.IsName("Hurt")) stateStep++;
            }
            else
            {
                if (aniInfo.normalizedTime > 0.7f)
                {
                    animator.SetBool("Hurt", false);
                    
                }
            }
        }
    }


}


