using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class EnemyNightmare : EnemyBase
    {
        float findPathBlankTime = .0f;
        public EnemyNightmare(Transform t, EnemyManager manager) : base(t, manager)
        {

            
        }

        public override void SetPatrolArea(PatrolRoute _patrolRoute, PathFinder.PathFinding _pathFinding)
        {
            pathFinding = _pathFinding;
            patrolRoute = _patrolRoute;

            idleState = new EnemyIdleState(this);
            patrolState = new EnemyPatrolState(this);
            chaseState = new EnemyChaseState(this);
            comboAttackState = new EnemyComboAttackState(this, 1, new float[1] { 0.31f}, new float[1] { 0.62f});
            Collider[] atkC = new Collider[1] { transform.Find("ATKCollider").GetComponent<Collider>()};
            comboAttackState.ATKColliders = atkC;
            hurtState = new EnemyHurtState(this);
            dieState = new EnemyDieState(this);
            suspectIdleState = new EnemySuspectIdleState(this);
            giveUpState = new EnemyGiveUpState(this);

            routeType = _patrolRoute.routeType;
            curPatrolPath = patrolRoute.path;

            if (patrolRoute.routeType == PatrolRoute.RouteType.Rotate)
            {
                transform.rotation = Quaternion.LookRotation(patrolRoute.LastLookForward);
                curState = idleState;
            }
            else
            {
                moveFwdDir = new Vector3(curPatrolPath.lookPoints[patrolRoute.CurPointID].x - selfPos.x, 0, curPatrolPath.lookPoints[patrolRoute.CurPointID].z - selfPos.z).normalized;
                if (patrolRoute.LastLookAround)
                {
                    curState = idleState;
                    idleTime = 1.0f;
                    transform.rotation = Quaternion.LookRotation(patrolRoute.LastLookForward);
                }
                else
                {
                    curState = patrolState;
                    animator.SetBool("Patrol", true);
                    transform.rotation = Quaternion.LookRotation(moveFwdDir);
                }

            }
        }
        public bool PlayerInSight(float distance)
        {
            Vector2 distV2 = new Vector2(enemyManager.Player.SelfTransform.position.x - transform.position.x, enemyManager.Player.SelfTransform.position.z - transform.position.z);
            if (Vector2.SqrMagnitude(distV2) <= distance * distance && pathFinding.CheckPlayerIsWalbable(enemyManager.Player.SelfTransform.position))
            {
                Debug.Log("nightmare 一般狀態 距離近");
                return true;
            }
            return false;

        }
        public override bool FindPlayer()
        {
            //return false;

            if (PlayerInSight(enemyManager.NightmareValue.SightDistance)) //Physics.Raycast(lookPos, lookDir, 5.0f, 1 << LayerMask.NameToLayer("Player"))
            {
                Debug.Log("懷疑時間 " + seeDelayTime);
                seeDelayTime += deltaTime * 1.5f;
                if (seeDelayTime > enemyManager.NightmareValue.SeeConfirmTime)
                {
                    seeDelayTime = .0f;
                    animator.SetTrigger("Alarm");
                    targetPos = enemyManager.Player.SelfTransform.position;
                    targetDir = new Vector3(targetPos.x - selfPos.x, 0, targetPos.z - selfPos.z);
                    if (!findingPath) {
                        findingPath = false;
                        curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, targetPos, OnPathFound);
                    }

                    Debug.Log("進懷疑");
                    ChangeState(suspectIdleState);  //先進"懷疑idle"以免尋路過久
                    return true;
                }
                return false;
            }
            else
            {
                if (seeDelayTime > .0f) seeDelayTime -= deltaTime;
                return false;
            }
        }
        public override void Idle()
        {
            Debug.Log("idle");
            if (idleTime > .0f) {
                stateTime += deltaTime;
                if (stateTime >= idleTime) {
                    Debug.Log("to patrol");
                    idleTime = -1.0f;
                    animator.SetBool("Patrol", true);
                    ChangeState(patrolState);
                }           
            }
        }
        public override void Patroling()
        {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateStep == 0)
            {
                if (aniInfo.IsName("Patrol"))
                {
                    stateStep++;
                    moveFwdDir = new Vector3(curPatrolPath.lookPoints[patrolRoute.CurPointID].x - selfPos.x, 0, curPatrolPath.lookPoints[patrolRoute.CurPointID].z - selfPos.z).normalized;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.NightmareValue.RotateSpeed);
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
                        if (!patrolRoute.LastLookAround && patrolRoute.routeType == PatrolRoute.RouteType.Pingpong)
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
                            return;
                        }
                    }
                    else
                    {
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
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.NightmareValue.RotateSpeed);
                //transform.position += deltaTime * enemyManager.HunterValue.MoveSpeed * transform.forward;

            }
            else if (stateStep == 2)
            {   //旋轉巡視

                if (Vector3.Angle(patrolRoute.LastLookForward, transform.forward) > 5.0f)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(patrolRoute.LastLookForward), deltaTime * enemyManager.NightmareValue.RotateSpeed);
                }
                else
                {
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
                    animator.SetBool("Patrol", false);
                    idleTime = 2.0f;
                    ChangeState(idleState);
                }
            }
        }
        public override bool FindPlayerInSuspect()
        {
            //return false;
            if (PlayerInDistance(enemyManager.NightmareValue.SightDistance*1.5f))
            {
                Debug.Log("馴鹿空檔idle");
                if (distanceCase == 2)
                {
                    animator.SetBool("Chase", false);
                    ChangeState(comboAttackState);
                    animator.SetBool("Attack", true);
                    return true;
                }
                else if (distanceCase == 0)
                {
                    animator.applyRootMotion = true;
                    ChangeState(giveUpState);//走回巡邏
                    return true;
                }
                else {
                    //判斷還是追逐，不過要等尋路完
                    return false;
                }
            }
            else {
                animator.applyRootMotion = true;
                ChangeState(giveUpState);//走回巡邏
                return true;
            }
        }
        public override void SuspectIdle()
        {
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDir), deltaTime * enemyManager.NightmareValue.RotateSpeed);
            if (findingPath) {
                Debug.Log("尋到路");
                animator.applyRootMotion = true;
                animator.SetBool("Chase",true);
                ChangeState(chaseState);
                
            }
            else {
                
                stateTime += deltaTime;
                if (stateStep == 0 && stateTime > 0.5f) //等一陣子先回idle動畫
                {
                    animator.SetBool("Chase", false);
                    stateStep++;
                }
                else if (stateTime > 2.0f) //等太久馴鹿未回傳，放棄
                {
                    ChangeState(giveUpState);
                }
            }
        }
        public bool PlayerInDistance(float distance)
        {
            Vector2 distV2 = new Vector2(enemyManager.Player.SelfTransform.position.x - transform.position.x, enemyManager.Player.SelfTransform.position.z - transform.position.z);
            float dist = Vector2.SqrMagnitude(distV2);
            if (dist <= distance * distance)
            {
                //進攻擊狀態
                if (dist <= enemyManager.NightmareValue.AttackDist * enemyManager.NightmareValue.AttackDist)
                {
                    Debug.Log("in distance  進攻擊狀態");
                    distanceCase = 2;
                    targetPos = enemyManager.Player.SelfTransform.position;
                    return true;

                }
                //進追逐狀態
                else
                {
                    Debug.Log("in distance  進追逐狀態");
                    distanceCase = 1;
                    targetPos = enemyManager.Player.SelfTransform.position;
                    return true;
                }
            }
            return false;

        }
        public override void Chasing()
        {
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
                if (!pathFinding.CheckInGrid(enemyManager.Player.transform.position))
                {
                    Debug.Log("超出範圍  不追");
                    animator.SetBool("Chase", false);
                    ChangeState(giveUpState);
                    return;
                }
                //先判斷距離和看不看的到
                if (PlayerInDistance(enemyManager.NightmareValue.SightDistance*1.5f))
                {
                    Debug.Log("追逐");
                    //追逐
                    if (distanceCase == 1)
                    {
                        Debug.Log("還感覺的到，持續追逐");
                        if (Physics.Linecast(targetPos, selfPos + new Vector3(0, 1.0f, 0), 1 << LayerMask.NameToLayer("Barrier") | LayerMask.NameToLayer("Wall")))
                        {
                            findPathBlankTime += deltaTime;
                            if (findPathBlankTime > 0.3f) {
                                curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, targetPos, OnPathFound);
                                findPathBlankTime = .0f;
                            }
                            if (findingPath) {
                                Vector2 pos2D = new Vector2(selfPos.x, selfPos.z);
                                if (playerPath.turnBoundaries[playerPathIndex].HasCrossedLine(pos2D))
                                {
                                    if (playerPathIndex == playerPath.finishLineIndex)
                                    {
                                        Debug.Log("到尋玩家終點");
                                        findingPath = false;
                                        //animator.SetBool("Chase", false);
                                        ChangeState(suspectIdleState);
                                        curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, targetPos, OnPathFound);
                                        return;
                                    }
                                    else
                                    {
                                        playerPathIndex++;
                                    }
                                }
                            }
                           
                            moveFwdDir = new Vector3(playerPath.lookPoints[playerPathIndex].x - selfPos.x, 0, playerPath.lookPoints[playerPathIndex].z - selfPos.z).normalized;
                            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.NightmareValue.RotateSpeed);
                        }
                        else {
                            //findPathBlankTime += deltaTime;
                            //if (findPathBlankTime > 0.3f)
                            //{
                            //    curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, targetPos, OnPathFound);
                            //    findPathBlankTime = .0f;
                            //}
                            //if (findingPath)
                            //{
                            //    Vector2 pos2D = new Vector2(selfPos.x, selfPos.z);
                            //    if (playerPath.turnBoundaries[playerPathIndex].HasCrossedLine(pos2D))
                            //    {
                            //        if (playerPathIndex == playerPath.finishLineIndex)
                            //        {
                            //            Debug.Log("到尋玩家終點");
                            //            //animator.SetBool("Chase", false);
                            //            ChangeState(suspectIdleState);
                            //            findingPath = false;
                            //            curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, targetPos, OnPathFound);
                            //            return;
                            //        }
                            //        else
                            //        {
                            //            playerPathIndex++;
                            //        }
                            //    }
                            //}
                            //moveFwdDir = new Vector3(playerPath.lookPoints[playerPathIndex].x - selfPos.x, 0, playerPath.lookPoints[playerPathIndex].z - selfPos.z).normalized;
                            moveFwdDir = new Vector3(targetPos.x - selfPos.x, 0, targetPos.z - selfPos.z).normalized;
                            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.NightmareValue.RotateSpeed);
                        }
                       
                       // transform.position += deltaTime * enemyManager.HunterValue.MoveSpeed * 2.0f * transform.forward;
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
                    else
                    {
                        animator.SetBool("Chase", false);
                        ChangeState(giveUpState);
                    }
                }
                //看不到
                else
                {
                    animator.SetBool("Chase", false);
                    ChangeState(giveUpState);
                }
            }
        }

        public override void GiveUp()
        {
            if (stateStep == 0)
            {
                Debug.Log("放棄追逐，尋路回本來位置請求");
                findingPath = false;
                Vector3 goal = (patrolRoute.routeType == PatrolRoute.RouteType.Rotate) ? patrolRoute.StartPosition : curPatrolPath.lookPoints[patrolRoute.CurPointID - 1];
                curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, goal, OnPathFound);
                stateStep++;
            }
            else if (stateStep == 1)
            {
                if (findingPath)
                {
                    Debug.Log("放棄追逐，找到回本來位置  ");
                    animator.SetBool("Patrol", true);
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
                        if (patrolRoute.routeType != PatrolRoute.RouteType.Rotate)
                        {
                            Debug.Log("走回到原本路線點");
                            ChangeState(patrolState);
                            return;
                        }
                        else
                        {
                            animator.SetBool("Patrol", false);
                            ChangeState(idleState);
                        }
                    }
                    else
                    {
                        playerPathIndex++;
                    }
                }
                moveFwdDir = new Vector3(playerPath.lookPoints[playerPathIndex].x - selfPos.x, 0, playerPath.lookPoints[playerPathIndex].z - selfPos.z).normalized;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveFwdDir), deltaTime * enemyManager.NightmareValue.RotateSpeed);
            }
        }

        public override void ComboAttack(ref int comboCount, int maxCombo)
        {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            
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
                    if ((Physics.OverlapBox(selfPos + 0.3f * selfFwd, new Vector3(0.5f, 0.5f, 0.5f), transform.rotation, (1 << LayerMask.NameToLayer("Barrier") | 1 << LayerMask.NameToLayer("Player"))).Length == 0))
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
                if (animator.applyRootMotion) animator.applyRootMotion = (Physics.OverlapBox(selfPos + 0.3f * selfFwd, new Vector3(0.5f, 0.5f, 0.5f), transform.rotation, (1 << LayerMask.NameToLayer("Barrier") | 1 << LayerMask.NameToLayer("Player"))).Length == 0);

                if (aniInfo.normalizedTime >= comboAttackState.currentColliderTime)
                {
                    Debug.Log("開啟碰撞器");
                    comboAttackState.hasEnableCollider = true;
                    comboAttackState.curATKCollider.enabled = true;
                    stateStep++;
                }
            }
            else if (stateStep == 2)
            {
                if (animator.applyRootMotion) animator.applyRootMotion = (Physics.OverlapBox(selfPos + 0.3f * selfFwd, new Vector3(0.5f, 0.5f, 0.5f), transform.rotation, (1 << LayerMask.NameToLayer("Barrier") | 1 << LayerMask.NameToLayer("Player"))).Length == 0);
                if (aniInfo.normalizedTime >= comboAttackState.currentDisColliderTime)
                {
                    Debug.Log("關閉碰撞器");
                    comboAttackState.hasEnableCollider = false;
                    comboAttackState.curATKCollider.enabled = false;
                    stateStep++;
                }
            }
            else
            {
                if (animator.applyRootMotion) animator.applyRootMotion = (Physics.OverlapBox(selfPos + 0.3f * selfFwd, new Vector3(0.5f, 0.5f, 0.5f), transform.rotation, (1 << LayerMask.NameToLayer("Barrier") | 1 << LayerMask.NameToLayer("Player"))).Length == 0);
                if (aniInfo.normalizedTime >= 0.98f)
                {
                    Debug.Log("攻擊結束");
                    if (PlayerInDistance(enemyManager.NightmareValue.SightDistance*1.5f))
                    {
                        animator.applyRootMotion = true;
                        if (distanceCase == 1)
                        {
                            comboCount = 0;
                            animator.SetBool("Attack", false);
                            Vector3 dir = new Vector3(targetPos.x - selfPos.x, 0, targetPos.z - selfPos.z);
                            transform.rotation = Quaternion.LookRotation(dir);
                            ChangeState(suspectIdleState);
                            findingPath = false;
                            curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, targetPos, OnPathFound);
                        }
                        else if (distanceCase == 2)
                        {
                            comboCount++;
                            if (comboCount >= maxCombo) comboCount = 0;
                            stateStep = 0;
                            animator.SetTrigger("NextCombo");
                            Vector3 dir = new Vector3(targetPos.x - selfPos.x, 0, targetPos.z - selfPos.z);
                            transform.rotation = Quaternion.LookRotation(dir);
                            Debug.Log("繼續攻擊");
                        }
                    }
                    else
                    {
                        comboCount = 0;
                        animator.SetBool("Attack", false);
                        findingPath = false;
                        ChangeState(suspectIdleState);
                    }
                }
            }
        }

        public override void InHurt()
        {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateStep == 0)
            {
                if (aniInfo.IsName("Hurt")) stateStep++;
            }
            else
            {
                if (CheckGetHurtInHurt())
                {
                    stateStep = 0;
                    return;
                }
                if (aniInfo.normalizedTime > (0.85f))
                {
                    animator.SetBool("Hurt", false);
                    if (PlayerInDistance(enemyManager.NightmareValue.SightDistance*1.5f))
                    {
                        if (distanceCase == 1)
                        {
                            findingPath = false;
                            ChangeState(suspectIdleState);
                            curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, targetPos, OnPathFound);
                            
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
                            findingPath = false;
                            ChangeState(suspectIdleState);
                            curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, targetPos, OnPathFound);
                            
                        }
                    }
                    else
                    {
                        findingPath = false;
                        ChangeState(suspectIdleState);
                        curPathRequest = PathFinder.PathRequestManager.RequestPath(pathFinding, curPathRequest, selfPos, targetPos, OnPathFound);
                    }
                    canHurt = true;
                    lastHurtHash = 999;
                    stateStep = 0;
                }
            }
        }
    }
}


