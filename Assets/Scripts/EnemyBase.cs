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
        float suspectTime = .0f;

        float idleTime = .0f;
        float seeDelayTime = .0f;
        int lookARoundNum = 0;

        bool canHurt = true;
        int lastHurtHash = 999;

        Vector3 selfPos;
        Vector3 moveFwdDir = new Vector3(0, 0, 0);
        Vector3 lookDir, lookPos;
        Vector3 targetPos;

        EnemyManager enemyManager;

        CapsuleCollider hurtAreaCollider;

        Transform head;
        Animator animator;

        int sightStep = 0;

        int playerPathIndex = 0;
        PathFinder.Path curPatrolPath, playerPath;
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
        EnemySuspectIdleState suspectIdleState;
        EnemySuspectMoveState suspectMoveState;
        EnemySuspectLookAroundState suspectLookAround;
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
            curPatrolPath = patrolRoute.path;

            if (patrolRoute.routeType == PatrolRoute.RouteType.Rotate) {
                curState = idleState;
            } 
            else {
                moveFwdDir = new Vector3(curPatrolPath.lookPoints[patrolRoute.CurPointID].x - selfPos.x, 0, curPatrolPath.lookPoints[patrolRoute.CurPointID].z - selfPos.z).normalized;
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
            return false;

            if (PlayerInSight(lookDir, enemyManager.HunterValue.SightDistance, enemyManager.HunterValue.SightAngle)) //Physics.Raycast(lookPos, lookDir, 5.0f, 1 << LayerMask.NameToLayer("Player"))
            {
                seeDelayTime += deltaTime*1.5f;
                if (seeDelayTime > enemyManager.HunterValue.SeeConfirmTime)
                {
                    seeDelayTime = .0f;
                    animator.SetTrigger("Alarm");
                }
                findingPath = false;
                curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, enemyManager.Player.SelfTransform.position, OnPathFound);
                ChangeState(suspectIdleState);  //先進"懷疑idle"以免尋路過久

                //animator.SetBool("Chase", true);
                //ChangeState(chaseState);
                return true;
            }
            else {
                if (seeDelayTime > .0f) seeDelayTime -= deltaTime;
                return false;
            } 
        }
        public bool FindPlayerInSuspect() {
            return false;
            if (PlayerInSight(lookDir, enemyManager.HunterValue.SightDistance, enemyManager.HunterValue.SightAngle)) {
                findingPath = false;
                curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, enemyManager.Player.SelfTransform.position, OnPathFoundInSuspect);
                moveFwdDir = new Vector3(targetPos.x - selfPos.x, 0, targetPos.z - selfPos.z).normalized;
                return true;
            }
            return false;
        }

        bool PlayerInSight(Vector3 dir, float distance, float angle) {
            if (sightStep == 0)
            {
                Vector2 distV2 = new Vector2(enemyManager.Player.SelfTransform.position.x - transform.position.x, enemyManager.Player.SelfTransform.position.z - transform.position.z);
                Vector2 dirV2 = new Vector2(dir.x, dir.z);
                if (Vector2.SqrMagnitude(distV2) <= distance * distance && Vector2.Angle(dirV2, distV2) < angle)
                {
                    
                    sightStep++;
                }
                return false;
            }
            else {
                sightStep = 0;
                if (Physics.Linecast(lookPos, enemyManager.Player.SelfTransform.position, 1 << LayerMask.NameToLayer("Barrier")))
                {
                    return false;
                }
                else {
                    targetPos = enemyManager.Player.SelfTransform.position;
                    return true;
                }
                    
            }
           
        }

        public virtual void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
        {
            curPathRequest = null;  //找到之後把尋路請求清除
            if (pathSuccessful)
            {
                //ChangeState(suspectMoveState);
                playerPath = new PathFinder.Path(waypoints, selfPos, 0.5f);
                playerPathIndex = 0;
                moveFwdDir = new Vector3(playerPath.lookPoints[playerPathIndex].x - selfPos.x, 0, playerPath.lookPoints[playerPathIndex].z - selfPos.z).normalized;
                findingPath = true;

                //StopCoroutine("FollowPath");
                //StartCoroutine("FollowPath");
            }
            else
            {
                //沒找到路徑回巡邏
            }
        }
        public virtual void OnPathFoundInSuspect(Vector3[] waypoints, bool pathSuccessful)
        {
            curPathRequest = null;  //找到之後把尋路請求清除
            if (pathSuccessful)
            {
                if (curState != chaseState)
                {
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
                ChangeState(suspectIdleState);  //沒找到路徑回idle
            }
        }

        public void SetAniBool(string name, bool value) {
            animator.SetBool(name, value);
        }

        public void Idle() {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateStep == 0)
            {
                if (aniInfo.IsName("Idle")) {
                    stateStep++;
                    idleTime = Random.Range(1.5f, 2.5f);
                } 
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
                    moveFwdDir = new Vector3(curPatrolPath.lookPoints[patrolRoute.CurPointID].x - selfPos.x, 0, curPatrolPath.lookPoints[patrolRoute.CurPointID].z - selfPos.z).normalized;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.HunterValue.RotateSpeed);
                }
            }
            else if (stateStep == 1)
            {
                Vector2 pos2D = new Vector2(selfPos.x, selfPos.z);
                if (curPatrolPath.turnBoundaries[patrolRoute.CurPointID].HasCrossedLine(pos2D))
                {
                    if (patrolRoute.CurPointID == patrolRoute.path.finishLineIndex)
                    {
                        patrolRoute.CurPointID = 1;
                        pathOver = true;
                        if (!patrolRoute.LastLookAround && patrolRoute.routeType == PatrolRoute.RouteType.Pingpong) {
                            if (patrolRoute.SetReversePath())
                            {
                                curPatrolPath = patrolRoute.reversePath;
                            }
                            else
                            {
                                curPatrolPath = patrolRoute.path;
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
                moveFwdDir = new Vector3(curPatrolPath.lookPoints[patrolRoute.CurPointID].x - selfPos.x, 0, curPatrolPath.lookPoints[patrolRoute.CurPointID].z - selfPos.z).normalized;
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
                            curPatrolPath = patrolRoute.reversePath;
                        }
                        else
                        {
                            curPatrolPath = patrolRoute.path;
                        }
                        pathOver = false;
                    }
                    animator.speed = 1.0f;
                    animator.SetBool("Look", true);
                    ChangeState(lookAroundState);
                }
            }
        }

        public void SuspectIdle() {
            if (stateStep == 0) {
                suspectTime = Random.Range(0.3f, 2.0f);
                stateStep++;
            }
            else {
                    stateTime += deltaTime;
                    if (stateTime > suspectTime && findingPath)
                    {
                        animator.SetBool("Patrol", true);
                        ChangeState(suspectMoveState);
                    }
            }
        }
        public void SuspectMove()
        {
            if (stateStep == 0)
            {
                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (aniInfo.IsName("Patrol"))
                {
                    stateStep++;
                }
            }
            else {
                Vector2 diff = new Vector2(selfPos.x - targetPos.x, selfPos.z - targetPos.z);
                if (diff.sqrMagnitude < 1.0f) {
                    animator.SetBool("Patrol", false);
                    ChangeState(lookAroundState);//到定點查看
                    return;
                }

                Vector2 pos2D = new Vector2(selfPos.x, selfPos.z);
                if (curPatrolPath.turnBoundaries[patrolRoute.CurPointID].HasCrossedLine(pos2D))
                {
                    if (patrolRoute.CurPointID == patrolRoute.path.finishLineIndex)
                    {

                    }
                    else
                    {
                        patrolRoute.CurPointID++;
                    }
                }
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.HunterValue.RotateSpeed);
                transform.position += deltaTime * enemyManager.HunterValue.MoveSpeed * moveFwdDir;
                
            }
        }

        public void Chasing() {
            if (stateStep == 0)
            {
                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (aniInfo.IsName("Chase")) stateStep++;
                Vector2 pos2D = new Vector2(selfPos.x, selfPos.z);
                if (curPatrolPath.turnBoundaries[patrolRoute.CurPointID].HasCrossedLine(pos2D))
                {
                    if (patrolRoute.CurPointID == patrolRoute.path.finishLineIndex)
                    {
                        
                    }
                    else
                    {
                        patrolRoute.CurPointID++;
                    }
                }
                moveFwdDir = new Vector3(curPatrolPath.lookPoints[patrolRoute.CurPointID].x - selfPos.x, 0, curPatrolPath.lookPoints[patrolRoute.CurPointID].z - selfPos.z).normalized;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.HunterValue.RotateSpeed);
                transform.position += deltaTime * enemyManager.HunterValue.MoveSpeed * moveFwdDir;
                Debug.Log("chase  0");
            }
            else if (stateStep == 1)
            {
                Debug.Log("chase  2");
                stateTime += deltaTime;
                if (stateTime > 0.5f)
                {
                    curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, enemyManager.Player.SelfTransform.position, OnPathFound);
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


                Vector3 diff = new Vector3(enemyManager.Player.SelfTransform.position.x - selfPos.x, 0, enemyManager.Player.SelfTransform.position.z - selfPos.z);
                float dist = diff.sqrMagnitude;

                if (!Physics.Linecast(lookPos, enemyManager.Player.SelfTransform.position, 1 << LayerMask.NameToLayer("Barrier"))) //看得到玩家
                {
                    if (dist > 2.0f)  //不夠近繼續追，夠近攻擊
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.HunterValue.RotateSpeed);
                        transform.position += deltaTime * enemyManager.HunterValue.MoveSpeed * 2.0f * moveFwdDir;
                    }
                    else
                    {
                        animator.SetBool("Patrol", false);
                        animator.SetBool("Attack", true);
                        ChangeState(attackState);
                    }
                }
                else
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.HunterValue.RotateSpeed);
                    transform.position += deltaTime * enemyManager.HunterValue.MoveSpeed * 2.0f * moveFwdDir;
                }
            }
            else {
                stateTime += deltaTime;
                if (stateTime > 0.5f)
                {
                    curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, enemyManager.Player.SelfTransform.position, OnPathFound);
                }
            }
        }

        public bool CheckGetHurt() {

            Vector3 center = hurtAreaCollider.center;
            Vector3 point2 = transform.position + center.x * transform.right + center.z * transform.forward;
            Vector3 point1 = point2 + new Vector3(0, hurtAreaCollider.height, 0);
            Debug.DrawLine(point1, point2, Color.red);
            Collider[] cols = Physics.OverlapCapsule(point1, point2, hurtAreaCollider.radius, enemyManager.HunterValue.HurtAreaLayer);
            int curCount = enemyManager.Player.GetAttackComboCount();
            if (cols != null && cols.Length > 0 && lastHurtHash != curCount)
            {
                Debug.Log("get hurt  last" + lastHurtHash + "  cur" + curCount + "  hp:" + hp);
                hp -= 10;
                lastHurtHash = curCount;
                
                if (hp > 0)
                {
                    canHurt = false;
                    animator.SetBool("Hurt", true);
                    ChangeState(hurtState);
                }
                else {
                    animator.SetBool("Hurt", false);
                    animator.SetBool("Dead", true);
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
            int curCount = enemyManager.Player.GetAttackComboCount();
            if (cols != null && cols.Length > 0 && lastHurtHash != curCount)
            {
                Debug.Log("get hurt  last" + lastHurtHash + "  cur" + curCount + "  hp:" + hp);
                hp -= 10;
                lastHurtHash = curCount;
                
                if (hp > 0)
                {
                    canHurt = false;
                    animator.SetTrigger("HurtAgain");
                    //ChangeState(hurtState);
                    Debug.Log("hurt time   " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                }
                else
                {
                    animator.SetBool("Dead", true);
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
                if (CheckGetHurtDie()) {
                    stateStep = 0;
                    return;
                }
                if (aniInfo.normalizedTime > (0.85f))
                {
                    animator.SetBool("Hurt", false);
                    if (patrolRoute.routeType != PatrolRoute.RouteType.Rotate) {
                        animator.SetBool("Patrol", true);
                        ChangeState(patrolState);
                    }
                    else ChangeState(idleState);
                    canHurt = true;
                    lastHurtHash = 999;
                    stateStep = 0;
                }
            }
        }
        public void Dead() {
            if (stateStep == 0)
            {
                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (aniInfo.IsName("Dead")) {
                    animator.SetBool("Dead", false);
                    stateStep++;
                } 
            }
        }
    }


}


