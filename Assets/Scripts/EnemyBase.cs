using System.Collections;
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

        bool canHurt = true;
        int lastHurtHash;

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
        int curPointID = 1;
        PatrolRoute.RouteType routeType;
        bool pathOver = false;

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

            routeType = _patrolRoute.routeType;
            curPath = patrolRoute.path;

            if (patrolRoute.routeType == PatrolRoute.RouteType.Rotate) {
                curState = idleState;
            } 
            else {
                moveFwdDir = new Vector3(curPath.lookPoints[patrolRoute.CurPointID].x - selfPos.x, 0, curPath.lookPoints[patrolRoute.CurPointID].z - selfPos.z).normalized;
                if (patrolRoute.LastLookAround)
                {
                    curState = lookAroundState;
                    animator.SetBool("Look", true);
                    transform.rotation = Quaternion.LookRotation(patrolRoute.LastLookForward);
                    
                }
                else {
                    curState = patrolState;
                    animator.SetBool("Patrol", true);
                    transform.rotation = Quaternion.LookRotation(moveFwdDir);
                }
                
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
                if (aniInfo.IsName("LookRound")) {
                    stateStep++;
                    lookARoundNum = 1;
                    
                } 
            }
            else {
                if (aniInfo.normalizedTime > lookARoundNum * 0.9f) {
                    lookARoundNum++;
                    if (lookARoundNum > patrolRoute.LastLookNum) {  //巡邏點已經提前+1，所以是看上一個點的旋轉次數
                        lookARoundNum = 0;
                        animator.SetBool("Look", false);
                        if (patrolRoute.routeType == PatrolRoute.RouteType.Rotate) ChangeState(idleState);
                        else
                        {
                            ChangeState(patrolState);
                            animator.SetBool("Patrol", true);
                        }
                    }
                }
            }
        }

        public void Patroling()
        {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateStep == 0)
            {
                if (aniInfo.IsName("Patrol"))
                {
                    stateStep++;
                    moveFwdDir = new Vector3(curPath.lookPoints[patrolRoute.CurPointID].x - selfPos.x, 0, curPath.lookPoints[patrolRoute.CurPointID].z - selfPos.z).normalized;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.HunterValue.RotateSpeed);
                }
            }
            else if (stateStep == 1)
            {
                Vector2 pos2D = new Vector2(selfPos.x, selfPos.z);
                if (curPath.turnBoundaries[patrolRoute.CurPointID].HasCrossedLine(pos2D))
                {
                    if (patrolRoute.CurPointID == patrolRoute.path.finishLineIndex)
                    {
                        patrolRoute.CurPointID = 1;
                        pathOver = true;
                        if (!patrolRoute.LastLookAround && patrolRoute.routeType == PatrolRoute.RouteType.Pingpong) {
                            if (patrolRoute.SetReversePath())
                            {
                                curPath = patrolRoute.reversePath;
                            }
                            else
                            {
                                curPath = patrolRoute.path;
                            }
                            pathOver = false;
                            return;
                        }
                    }
                    else {
                        patrolRoute.CurPointID++;
                    }

                    if (patrolRoute.LastLookAround)
                    {
                        stateStep++;
                        animator.speed = 0.3f;
                        return;
                    }
                }
                moveFwdDir = new Vector3(curPath.lookPoints[patrolRoute.CurPointID].x - selfPos.x, 0, curPath.lookPoints[patrolRoute.CurPointID].z - selfPos.z).normalized;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.HunterValue.RotateSpeed);
                transform.position += deltaTime * enemyManager.HunterValue.MoveSpeed * transform.forward;

            }
            else if (stateStep == 2){   //旋轉巡視
                
                if (Vector3.Angle(patrolRoute.LastLookForward, transform.forward) > 5.0f)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(patrolRoute.LastLookForward), deltaTime * enemyManager.HunterValue.RotateSpeed);
                }
                else {
                    if (pathOver && patrolRoute.routeType == PatrolRoute.RouteType.Pingpong)
                    {
                        if (patrolRoute.SetReversePath())
                        {
                            curPath = patrolRoute.reversePath;
                        }
                        else
                        {
                            curPath = patrolRoute.path;
                        }
                        pathOver = false;
                    }
                    animator.speed = 1.0f;
                    animator.SetBool("Look", true);
                    ChangeState(lookAroundState);
                }
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
            if (cols != null && cols.Length > 0 && lastHurtHash != enemyManager.Player.GetAttackHash())
            {
                hp -= 10;
                lastHurtHash = enemyManager.Player.GetAttackHash();
                if (hp > 0)
                {
                    canHurt = false;
                    animator.SetBool("Hurt", true);
                    ChangeState(hurtState);
                }
                else {
                    animator.SetBool("Dead", true);
                    lastHurtHash = 0;
                    ChangeState(dieState);
                }
                return true;
            }

            return false;
        }
        public bool CheckGetHurtDie()
        {

            Vector3 center = hurtAreaCollider.center;
            Vector3 point2 = transform.position + center.x * transform.right + center.z * transform.forward;
            Vector3 point1 = point2 + new Vector3(0, hurtAreaCollider.height, 0);
            Debug.DrawLine(point1, point2, Color.red);
            Collider[] cols = Physics.OverlapCapsule(point1, point2, hurtAreaCollider.radius, enemyManager.HunterValue.HurtAreaLayer);
            if (cols != null && cols.Length > 0 && lastHurtHash != enemyManager.Player.GetAttackHash())
            {
                hp -= 10;
                lastHurtHash = enemyManager.Player.GetAttackHash();
                if (hp > 0)
                {
                    canHurt = false;
                    animator.SetBool("Hurt", true);
                    ChangeState(hurtState);
                }
                else
                {
                    animator.SetBool("Dead", true);
                    lastHurtHash = 0;
                    ChangeState(dieState);
                    return true;
                }  
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

                if(CheckGetHurtDie())return;
                if (aniInfo.normalizedTime > 0.85f)
                {
                    animator.SetBool("Hurt", false);
                    if (patrolRoute.routeType != PatrolRoute.RouteType.Rotate) {
                        animator.SetBool("Patrol", true);
                        ChangeState(patrolState);
                    }
                    else ChangeState(idleState);
                    canHurt = true;
                    lastHurtHash = 0;
                }
            }
        }
        public void Dead() {
            Debug.Log("dddddddddddddiiiiiiiiiiiiiiiiie");
            if (stateStep == 0)
            {
                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (aniInfo.IsName("Dead")) {
                    Debug.Log("dddddddddddddiiiiiiiiiiiiiiiiie   confirm");
                    animator.SetBool("Dead", false);
                    stateStep++;
                } 
            }
        }
    }


}


