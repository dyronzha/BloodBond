using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class EnemyBase
    {
        protected int hp;

        bool findingPath = false;

        int stateStep = 0;
        float stateTime = .0f, deltaTime = .0f;
        float suspectTime = .0f;

        float idleTime = .0f;
        float seeDelayTime = .0f;
        int lookARoundNum = 0;

        bool canHurt = true;
        int lastHurtHash = 999;

        Vector3 selfPos, selfFwd;
        Vector3 moveFwdDir = new Vector3(0, 0, 0);
        Vector3 lookDir, lookPos;
        Vector3 targetPos, targetDir;

        EnemyManager enemyManager;

        CapsuleCollider hurtAreaCollider;

        Transform head;
        Animator animator;

        int sightStep = 0;
        int distanceCase = 0; // 1:警覺  2:攻擊

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

        protected EnemyBaseState curState;
        protected EnemyIdleState idleState;
        protected EnemyPatrolState patrolState;
        protected EnemyLookAroundState lookAroundState;
        protected EnemyChaseState chaseState;
        protected EnemyComboAttackState comboAttackState;
        protected EnemyHurtState hurtState;
        protected EnemyYellState yellState;
        protected EnemySuspectIdleState suspectIdleState;
        protected EnemySuspectMoveState suspectMoveState;
        protected EnemySuspectLookAroundState suspectLookAroundState;
        protected EnemyGiveUpState giveUpState;
        protected EnemyDieState dieState;

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
            comboAttackState = new EnemyComboAttackState(this,3,new float[3] {0.75f,0.6f,0.3f}, new float[3] {0.92f,0.73f,0.4f});
            Collider[] atkC = new Collider[1] { transform.Find("mixamorig:Hips").GetChild(2).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetComponent<Collider>() };
            comboAttackState.ATKColliders = atkC;
            hurtState = new EnemyHurtState(this);
            yellState = new EnemyYellState(this);
            dieState = new EnemyDieState(this);
            suspectIdleState = new EnemySuspectIdleState(this);
            suspectMoveState = new EnemySuspectMoveState(this);
            suspectLookAroundState = new EnemySuspectLookAroundState(this);
            giveUpState = new EnemyGiveUpState(this);

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
            selfFwd = transform.forward;
            lookDir = head.forward;
            lookPos = selfPos + new Vector3(0, 1.3f, 0);
            curState.Update();
        }

        public void ChangeState(EnemyBaseState state) {
            stateStep = 0;
            stateTime = .0f;
            curState = state;
            animator.speed = 1.0f;
        }

        public virtual bool FindPlayer()
        {
            //return false;

            if (PlayerInSight(lookDir, enemyManager.HunterValue.SightDistance, enemyManager.HunterValue.SightAngle)) //Physics.Raycast(lookPos, lookDir, 5.0f, 1 << LayerMask.NameToLayer("Player"))
            {
                Debug.Log("懷疑時間 " + seeDelayTime);
                seeDelayTime += deltaTime*1.5f;
                if (seeDelayTime > enemyManager.HunterValue.SeeConfirmTime)
                {
                    seeDelayTime = .0f;
                    animator.SetTrigger("Alarm");
                    targetPos = enemyManager.Player.SelfTransform.position;
                    targetDir = new Vector3(targetPos.x - selfPos.x, 0, targetPos.z - selfPos.z);
                    findingPath = false;
                    curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, targetPos, OnPathFound);
                    Debug.Log("進懷疑");
                    ChangeState(suspectIdleState);  //先進"懷疑idle"以免尋路過久
                    //animator.SetBool("Chase", true);
                    //ChangeState(chaseState);
                    return true;
                }
                return false;
            }
            else {
                if (seeDelayTime > .0f) seeDelayTime -= deltaTime;
                return false;
            } 
        }
        public bool FindPlayerInSuspect() {
            //return false;
            if (PlayerInDistance(lookDir, enemyManager.HunterValue.SightDistance, enemyManager.HunterValue.SightAngle)) {
                if (distanceCase == 1)
                {
                    findingPath = false;
                    //curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, enemyManager.Player.SelfTransform.position, OnPathFoundInSuspect);
                    ChangeState(chaseState);
                    animator.SetBool("Chase", true);
                    return true;
                }
                else if (distanceCase == 2)
                {
                    ChangeState(comboAttackState);
                    animator.SetBool("Attack", true);
                    return true;
                }
                else return false;
                
            }
            return false;
        }

        bool PlayerInSight(Vector3 dir, float distance, float angle) {
            Debug.DrawRay(lookPos, dir, Color.yellow, distance);
            if (sightStep == 0)
            {
                Vector2 distV2 = new Vector2(enemyManager.Player.SelfTransform.position.x - transform.position.x, enemyManager.Player.SelfTransform.position.z - transform.position.z);
                Vector2 dirV2 = new Vector2(dir.x, dir.z);
                if (Vector2.SqrMagnitude(distV2) <= distance * distance && Vector2.Angle(dirV2, distV2) < angle)
                {
                    Debug.Log("一般狀態 距離近且角度內");
                    sightStep++;
                }
                return false;
            }
            else {
                sightStep = 0;
                if (Physics.Linecast(lookPos, enemyManager.Player.SelfTransform.position, 1 << LayerMask.NameToLayer("Barrier")))
                {
                    Debug.Log("一般狀態 距離近且角度內 有障礙物");
                    return false;
                }
                else {
                    Debug.Log("一般狀態 距離近且角度內 無障礙物 看到進懷疑時間");
                    targetPos = enemyManager.Player.SelfTransform.position;
                    return true;
                }
            }
            
        }
        public virtual bool PlayerInDistanceAfterClose()   //在近距離接觸過後的判斷，少角度確認比較寬鬆
        {
            float distance = enemyManager.HunterValue.SightDistance;
            //float angle = enemyManager.HunterValue.SightAngle;
            float atkDist = enemyManager.HunterValue.AttackDist;
            Debug.DrawRay(lookPos, lookDir, Color.yellow, distance);
            if (!Physics.Linecast(lookPos, enemyManager.Player.SelfTransform.position, 1 << LayerMask.NameToLayer("Barrier")))
            {
                Vector2 distV2 = new Vector2(enemyManager.Player.SelfTransform.position.x - transform.position.x, enemyManager.Player.SelfTransform.position.z - transform.position.z);
                //Vector2 dirV2 = new Vector2(lookDir.x, lookDir.z);
                float dist = Vector2.SqrMagnitude(distV2);
                if (dist <= distance * distance)
                {
                    //進攻擊狀態
                    if (dist <= atkDist*atkDist)
                    {
                        Debug.Log("in distance  進攻擊狀態");
                        distanceCase = 2;
                        targetPos = enemyManager.Player.SelfTransform.position;
                        return true;

                    }
                    //進追逐狀態
                    else //if (Vector2.Angle(dirV2, distV2) < angle)
                    {
                        Debug.Log("in distance  進追逐狀態");
                        distanceCase = 1;
                        targetPos = enemyManager.Player.SelfTransform.position;
                        return true;
                    }
                }
                Debug.Log("in distance 太遠看不到");
            }
            Debug.Log("in distance 有障礙物");
            distanceCase = 0;
            return false;
        }
        bool PlayerInDistance(Vector3 dir, float distance, float angle)
        {
            Debug.DrawRay(lookPos, lookDir, Color.yellow, distance);
            if (!Physics.Linecast(lookPos, enemyManager.Player.SelfTransform.position, 1 << LayerMask.NameToLayer("Barrier")))
            {
                Vector2 distV2 = new Vector2(enemyManager.Player.SelfTransform.position.x - transform.position.x, enemyManager.Player.SelfTransform.position.z - transform.position.z);
                Vector2 dirV2 = new Vector2(dir.x, dir.z);
                float dist = Vector2.SqrMagnitude(distV2);
                if (dist <= distance * distance)
                {
                    //進攻擊狀態
                    if (dist <= enemyManager.HunterValue.AttackDist * enemyManager.HunterValue.AttackDist)
                    {
                        Debug.Log("in distance  進攻擊狀態");
                        distanceCase = 2;
                        targetPos = enemyManager.Player.SelfTransform.position;
                        return true;

                    }
                    //進追逐狀態
                    else if (Vector2.Angle(dirV2, distV2) < angle)
                    {
                        Debug.Log("in distance  進追逐狀態");
                        distanceCase = 1;
                        targetPos = enemyManager.Player.SelfTransform.position;
                        return true;
                    }
                    Debug.Log("in distance 太遠看不到");
                }
            }
            Debug.Log("in distance 有障礙物");
            distanceCase = 0;
            return false;
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
                animator.SetBool("Patrol", false);
                ChangeState(giveUpState);
                //沒找到路徑回巡邏
            }
        }
        public virtual void OnPathFoundInSuspect(Vector3[] waypoints, bool pathSuccessful)
        {
            curPathRequest = null;  //找到之後把尋路請求清除
            if (pathSuccessful)
            {
                findingPath = true;
                if (curState != chaseState)
                {
                    ChangeState(chaseState);
                    animator.SetBool("Chase", true);
                }
                playerPath = new PathFinder.Path(waypoints, selfPos, 0.5f);
                playerPathIndex = 0;
                moveFwdDir = new Vector3(playerPath.lookPoints[playerPathIndex].x - selfPos.x, 0, playerPath.lookPoints[playerPathIndex].z - selfPos.z).normalized;
                stateStep = 1;

                //StopCoroutine("FollowPath");
                //StartCoroutine("FollowPath");
            }
            else
            {
                animator.SetBool("Patrol", false);
                ChangeState(giveUpState);  //沒找到路徑放棄
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
                        animator.speed = 0.3f;//為了讓守衛轉到指定方向，讓動畫變慢比較自然
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
                    animator.SetBool("Patrol", false);
                    ChangeState(lookAroundState);
                }
            }
        }

        public void SuspectIdle() {
            if (stateStep == 0) {
                suspectTime = Random.Range(0.3f, 2.0f);
                transform.rotation = Quaternion.LookRotation(targetDir);
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
                Vector2 pos2D = new Vector2(selfPos.x, selfPos.z);
                if (playerPath.turnBoundaries[playerPathIndex].HasCrossedLine(pos2D))
                {
                    if (playerPathIndex == playerPath.finishLineIndex)
                    {
                        animator.SetBool("Patrol", false);
                        animator.SetBool("Look", true);
                        ChangeState(lookAroundState);//到定點查看
                        return;
                    }
                    else
                    {
                        playerPathIndex++;
                    }
                }
                moveFwdDir = new Vector3(curPatrolPath.lookPoints[patrolRoute.CurPointID].x - selfPos.x, 0, curPatrolPath.lookPoints[patrolRoute.CurPointID].z - selfPos.z).normalized;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.HunterValue.RotateSpeed);
                transform.position += deltaTime * enemyManager.HunterValue.MoveSpeed * moveFwdDir;
                
            }
        }

        public void SuspectLookAround() {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateStep == 0)
            {
                if (aniInfo.IsName("LookRound"))
                {
                    stateStep++;
                    lookARoundNum = Random.Range(2, 4);
                }
            }
            else
            {
                if (aniInfo.normalizedTime > lookARoundNum * 0.9f)
                {
                    lookARoundNum++;
                    if (lookARoundNum > patrolRoute.LastLookNum)
                    {  //巡邏點已經提前+1，所以是看上一個點的旋轉次數
                        lookARoundNum = 0;
                        animator.SetBool("Patrol", false);   //避免尋路過久先回idle動畫
                        animator.SetBool("Look", false);
                        ChangeState(giveUpState);//走回巡邏

                    }
                }
            }
        }

        public void GiveUp() {
            if (stateStep == 0)
            {
                Debug.Log("放棄追逐，尋路回本來位置請求");
                findingPath = false;
                curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, curPatrolPath.lookPoints[patrolRoute.CurPointID-1], OnPathFound);
                stateStep++;
            }
            else if (stateStep == 1)
            {
                if (pathFinding)
                {
                    Debug.Log("放棄追逐，找到回本來位置  ");
                    animator.SetBool("Patrol", true);
                    animator.SetBool("Look", false);
                    stateStep++;
                }
            }
            else if (stateStep == 2)
            {
                Vector2 pos2D = new Vector2(selfPos.x, selfPos.z);
                if (playerPath.turnBoundaries[playerPathIndex].HasCrossedLine(pos2D))
                {
                    if (playerPathIndex == playerPath.finishLineIndex)
                    {
                        Debug.Log("走回到原本路線點");
                        ChangeState(patrolState); 
                        return;
                    }
                    else
                    {
                        playerPathIndex++;
                    }
                }
                moveFwdDir = new Vector3(playerPath.lookPoints[playerPathIndex].x - selfPos.x, 0, playerPath.lookPoints[playerPathIndex].z - selfPos.z).normalized;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.HunterValue.RotateSpeed);
                transform.position += deltaTime * enemyManager.HunterValue.MoveSpeed * transform.forward;
            }
        }

        public void Chasing() {
            if (stateStep == 0)
            {
                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (aniInfo.IsName("Chase")) stateStep++;
                moveFwdDir = new Vector3(targetPos.x - selfPos.x, 0, targetPos.z - selfPos.z).normalized;
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.HunterValue.RotateSpeed);
                //transform.position += deltaTime * enemyManager.HunterValue.MoveSpeed * moveFwdDir;
                Debug.Log("chase  0");
            }
            else if (stateStep == 1)
            {
                Debug.Log("chase  1");
                
                //確認玩家有沒有超過grid，超過放棄追逐
                if (pathFinding.CheckInGrid(targetPos)) {
                    ChangeState(giveUpState);
                    return;
                }
                //先判斷距離和看不看的到
                if (PlayerInDistance(lookDir, enemyManager.HunterValue.SightDistance, enemyManager.HunterValue.SightAngle)) {

                    //追逐
                    if (distanceCase == 1)
                    {
                        Debug.Log("還看的到，持續追逐");
                        moveFwdDir = new Vector3(targetPos.x - selfPos.x, 0, targetPos.z - selfPos.z).normalized;
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.HunterValue.RotateSpeed);
                        transform.position += deltaTime * enemyManager.HunterValue.MoveSpeed * 2.0f * transform.forward;
                    }
                    //攻擊
                    else if (distanceCase == 2)
                    {
                        Debug.Log("夠近改攻擊");
                        ChangeState(comboAttackState);
                        animator.SetBool("Attack", true);
                        animator.SetBool("Chase", false);
                    }
                    //看不到
                    else {
                        moveFwdDir = new Vector3(targetPos.x - selfPos.x, 0, targetPos.z - selfPos.z).normalized;
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.HunterValue.RotateSpeed);
                        transform.position += deltaTime * enemyManager.HunterValue.MoveSpeed * 2.0f * moveFwdDir;
                        float diff = new Vector2(targetPos.x - selfPos.x, targetPos.z - selfPos.z).sqrMagnitude;
                        Debug.Log("看不到，到最後看到位置  距離" + diff);
                        if (diff <= 2.0f)//到達最後看到玩家的位置
                        {
                            Debug.Log("到最後位置 進觀察");
                            ChangeState(suspectLookAroundState);
                            animator.SetBool("Look", true);
                            animator.SetBool("Chase", false);
                        }
                    }
                }
                //看不到
                else
                {
                    moveFwdDir = new Vector3(targetPos.x - selfPos.x, 0, targetPos.z - selfPos.z).normalized;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.HunterValue.RotateSpeed);
                    transform.position += deltaTime * enemyManager.HunterValue.MoveSpeed * 2.0f * moveFwdDir;
                    float diff = new Vector2(targetPos.x - selfPos.x, targetPos.z - selfPos.z).sqrMagnitude;
                    Debug.Log("看不到，到最後看到位置  距離" + diff);
                    if (diff <= 2.0f)//到達最後看到玩家的位置
                    {
                        Debug.Log("到最後位置 進觀察");
                        ChangeState(suspectLookAroundState);
                        animator.SetBool("Look", true);
                        animator.SetBool("Chase", false);
                    }
                }
            }
        }
        public void ComboAttack(ref int comboCount, int maxCombo) {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            bool canSee = PlayerInDistanceAfterClose();
            Debug.Log("攻擊動畫時間" + aniInfo.normalizedTime);
            if (stateStep == 0)
            {
                Debug.Log("攻擊  combo " + comboCount);
                if (aniInfo.IsName("Combo" + comboCount.ToString()))
                {
                    Debug.Log("名字一樣   " + aniInfo.normalizedTime);
                    //如果上一段接技還沒關碰撞器，先關
                    if (comboAttackState.hasEnableCollider)
                    {
                        comboAttackState.lastATKCollider.enabled = false;
                        comboAttackState.hasEnableCollider = false;
                    }
                    if ((Physics.OverlapBox(selfPos + 0.5f * selfFwd, new Vector3(0.5f, 0.5f, 0.5f), transform.rotation, (1 << LayerMask.NameToLayer("Barrier") | 1 << LayerMask.NameToLayer("Player"))).Length == 0))
                    {
                        animator.applyRootMotion = true;
                    }
                    else
                    {
                        animator.applyRootMotion = false;
                    }
                    stateStep++;
                }
            }
            else if (stateStep == 1)
            {
                Debug.Log("攻擊 1");
                if (animator.applyRootMotion) animator.applyRootMotion = (Physics.OverlapBox(selfPos + 0.5f * selfFwd, new Vector3(0.5f, 0.5f, 0.5f), transform.rotation, (1 << LayerMask.NameToLayer("Barrier") | 1 << LayerMask.NameToLayer("Player"))).Length == 0);

                if (aniInfo.normalizedTime >= comboAttackState.currentColliderTime) {
                    Debug.Log("開啟碰撞器");
                    comboAttackState.hasEnableCollider = true;
                    comboAttackState.curATKCollider.enabled = true;
                    stateStep++;
                }
            }
            else if (stateStep == 2) {
                if (animator.applyRootMotion) animator.applyRootMotion = (Physics.OverlapBox(selfPos + 0.5f * selfFwd, new Vector3(0.5f, 0.5f, 0.5f), transform.rotation, (1 << LayerMask.NameToLayer("Barrier") | 1 << LayerMask.NameToLayer("Player"))).Length == 0);
                if (aniInfo.normalizedTime >= comboAttackState.currentDisColliderTime)
                {
                    Debug.Log("關閉碰撞器");
                    comboAttackState.hasEnableCollider = false;
                    comboAttackState.curATKCollider.enabled = false;
                    stateStep++;
                }
            }
            else {
                if (animator.applyRootMotion) animator.applyRootMotion = !Physics.Raycast(selfPos, selfFwd, 0.5f, (1 << LayerMask.NameToLayer("Barrier") | 1 << LayerMask.NameToLayer("Player")));
                if (aniInfo.normalizedTime >= 0.98f)
                {
                    Debug.Log("攻擊結束");
                    if (canSee)
                    {
                        if (distanceCase == 1)
                        {
                            comboCount = 0;
                            animator.SetBool("Attack", false);
                            animator.SetBool("Chase", true);
                            ChangeState(chaseState);
                            Debug.Log("改追逐");
                        }
                        else if (distanceCase == 2) {
                            comboCount++;
                            if (comboCount >= maxCombo) comboCount = 0;
                            stateStep = 0;
                            animator.SetTrigger("NextCombo");
                            Debug.Log("繼續攻擊");
                        } 
                    }
                    else {
                        comboCount = 0;
                        animator.SetBool("Attack", false);
                        animator.SetBool("Chase", true);
                        ChangeState(chaseState);
                        Debug.Log("改追逐");
                        //animator.SetBool("Attack", false);
                        //animator.SetBool("Look", true);
                        //Vector3 dir = new Vector3(targetPos.x - selfPos.x, 0, targetPos.z - selfPos.z);
                        //transform.rotation = Quaternion.LookRotation(dir);
                        //ChangeState(suspectLookAroundState);
                        //Debug.Log("改觀察");
                    }
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
                bool canSee = PlayerInDistanceAfterClose();
                if (aniInfo.normalizedTime > (0.85f))
                {
                    animator.SetBool("Hurt", false);
                    if (canSee)
                    {
                        if (distanceCase == 1)
                        {
                            ChangeState(chaseState);
                            animator.SetBool("Chase", true);
                        }
                        else if (distanceCase == 2)
                        {
                            Vector3 dir = new Vector3(targetPos.x - selfPos.x, 0, targetPos.z - selfPos.z);
                            transform.rotation = Quaternion.LookRotation(dir);
                            ChangeState(comboAttackState);
                            animator.SetBool("Attack", true);
                        }
                        else
                        {
                            ChangeState(chaseState);
                            animator.SetBool("Chase", true);
                            //Vector3 dir = new Vector3(targetPos.x - selfPos.x, 0, targetPos.z - selfPos.z);
                            //transform.rotation = Quaternion.LookRotation(dir);
                            //ChangeState(suspectLookAroundState);
                            //animator.SetBool("Look", true);
                        }
                    }
                    else {
                        ChangeState(chaseState);
                        animator.SetBool("Chase", true);
                        //Vector3 dir = new Vector3(targetPos.x - selfPos.x, 0, targetPos.z - selfPos.z);
                        //transform.rotation = Quaternion.LookRotation(dir);
                        //ChangeState(suspectLookAroundState);
                        //animator.SetBool("Look", true);
                    }
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


