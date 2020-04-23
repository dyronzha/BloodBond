using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class EnemyArcher : EnemyBase
    {
        bool isAim = false;
        float attackBlankTime = .0f;
        EnemyDistantAttackState enemyDistantATKState;
        Transform spine1, hand, crossBow;
        Vector3 crossBowPos, oringinLoc, oringinDir; 
        Quaternion idleRot, lastAimRot, crossBowRot;



        public EnemyArcher(Transform t, EnemyManager manager): base(t, manager) {
            
            spine1 = transform.Find("mixamorig:Hips").Find("mixamorig:Spine").Find("mixamorig:Spine1");
            hand = spine1.Find("mixamorig:Spine2").Find("mixamorig:RightShoulder").Find("mixamorig:RightArm").Find("mixamorig:RightForeArm").Find("mixamorig:RightHand");
            crossBow = hand.Find("Crossbow");
        }

        public override void LateUpdate(float dtTime)
        {
        }

        public override void Init()
        {
            idleState = new EnemyIdleState(this);
            lookAroundState = new EnemyLookAroundState(this);

            hurtState = new EnemyHurtState(this);

            dieState = new EnemyDieState(this);

            enemyDistantATKState = new EnemyDistantAttackState(this);
            idleRot = transform.rotation;
            crossBowPos = crossBow.localPosition;
            crossBowRot = crossBow.localRotation;
            ChangeState(idleState);
            oringinLoc = transform.position;
            oringinDir = transform.forward;
        }

        public override bool FindPlayer()
        {
            //Vector2 distV2 = new Vector2(enemyManager.Player.SelfTransform.position.x - transform.position.x, enemyManager.Player.SelfTransform.position.z - transform.position.z);
            //moveFwdDir = (new Vector3(distV2.x, 0, distV2.y));
            //if (Vector2.SqrMagnitude(distV2) <= enemyManager.ArcherValue.SightDistance * enemyManager.ArcherValue.SightDistance) {
            //    ChangeState(enemyDistantATKState);
            //    animator.SetBool("Aim",true);
            //    lastAimRot = Quaternion.LookRotation(moveFwdDir) * Quaternion.Euler(0, -60, 0);
            //    transform.rotation = lastAimRot;
            //    enemyManager.SubLateAction(Aming);
            //    crossBow.gameObject.SetActive(true);
            //    return true;
            //}
            //return false;
                
            if (PlayerInSight(lookDir, enemyManager.ArcherValue.SightDistance, enemyManager.ArcherValue.SightAngle)) //Physics.Raycast(lookPos, lookDir, 5.0f, 1 << LayerMask.NameToLayer("Player"))
            {
                Debug.Log("懷疑時間 " + seeDelayTime);
                seeDelayTime += deltaTime;
                if (curState == lookAroundState && animator.speed > 0.7f) animator.speed = .0f;
                if (seeDelayTime > enemyManager.ArcherValue.SeeConfirmTime)
                {
                    animator.speed = 1.0f;
                    seeDelayTime = .0f;
                    targetPos = enemyManager.Player.SelfTransform.position;
                    targetDir = new Vector3(targetPos.x - selfPos.x, 0, targetPos.z - selfPos.z);

                    Debug.Log("進遠程攻擊");
                    isAlarm = true;
                    enemyManager.SetAllEnemyAlarm(this);
                    isAim = true;
                    ChangeState(enemyDistantATKState);
                    animator.SetBool("Aim", true);
                    lastAimRot = Quaternion.LookRotation(targetDir) * Quaternion.Euler(0, -60, 0);
                    transform.rotation = lastAimRot;
                    enemyManager.SubLateAction(transform.name, Aming);
                    crossBow.gameObject.SetActive(true);
                    return true;
                }
                return false;
            }
            else
            {
                Debug.Log("沒看到減少懷疑時間");
                seeDelayTime -= deltaTime*.5f;
                if (seeDelayTime < .0f) seeDelayTime = .0f;
                animator.speed = 1.0f;
                return false;
            }
        }

        public override void LookAround()
        {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateStep == 0)
            {
                if (aniInfo.IsName("LookRound"))
                {
                    stateStep++;
                }
            }
            else
            {
                if (aniInfo.normalizedTime >  0.9f)
                {
                    animator.SetBool("Look", false);
                    isAlarm = false;
                    ChangeState(idleState);
                }
            }
        }

        public override void DistantAttack()
        {

            if (stateStep == 0)
            {
                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (aniInfo.IsName("HoldUp"))
                {
                    crossBow.gameObject.SetActive(true);
                    stateStep++;
                }
            }
            else if (stateStep == 1)
            {
                if (!PlayerInDistanceAfterClose()) //檢查還看得到玩家
                {
                    ChangeState(idleState);
                    isAlarm = false;
                    attackBlankTime = .0f;
                    animator.SetBool("Aim", false);
                    animator.SetBool("Attack", false);
                    enemyManager.UnSubLateAction(transform.name);
                    isAim = false;
                    transform.rotation = idleRot;
                    crossBow.gameObject.SetActive(false);
                    return;
                }
                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (aniInfo.IsName("Aim"))
                {
                    stateStep++;
                }
            }
            else if (stateStep == 2)
            {
                if (!PlayerInDistanceAfterClose())//檢查還看得到玩家
                {
                    ChangeState(idleState);
                    isAlarm = false;
                    attackBlankTime = .0f;
                    animator.SetBool("Aim", false);
                    animator.SetBool("Attack", false);
                    enemyManager.UnSubLateAction(transform.name);
                    isAim = false;
                    transform.rotation = idleRot;
                    crossBow.gameObject.SetActive(false);
                    return;
                }
                attackBlankTime += deltaTime;
                if (attackBlankTime > 1.0f && enemyManager.GetArrowNum() > 0)
                {
                    attackBlankTime = .0f;
                    animator.SetBool("Attack", true);
                    
                    stateStep++;
                }
            }
            else if (stateStep == 3) {
                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (aniInfo.IsName("Attack"))
                {
                    AudioManager.SingletonInScene.PlaySound2D("BowHunter_Shoot", 0.3f);
                    crossBow.gameObject.SetActive(false);
                    targetPos = enemyManager.Player.SelfTransform.position;
                    enemyManager.SpawnArrow(crossBow.position, new Vector3(targetPos.x - crossBow.position.x, targetPos.y + 2.0f - crossBow.position.y, targetPos.z - crossBow.position.z).normalized);
                    stateStep++;
                }
            }
            else
            {
                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (aniInfo.normalizedTime > 0.95f)
                {
                    animator.SetBool("Attack", false);
                    attackBlankTime = .0f;
                    stateStep = 1;
                    if (!PlayerInDistanceAfterClose())
                    {
                        ChangeState(idleState);
                        isAlarm = false;
                        isAim = false;
                        enemyManager.UnSubLateAction(transform.name);
                        animator.SetBool("Aim", false);
                        transform.rotation = idleRot;
                    }
                    else
                    {
                        crossBow.gameObject.SetActive(true);
                    }
                }
            }
        }
        public void ClearAttackBlankTime() {
            attackBlankTime = .0f;
        }
        void Aming() {
            Vector2 distV2 = new Vector2(enemyManager.Player.SelfTransform.position.x - transform.position.x, enemyManager.Player.SelfTransform.position.z - transform.position.z);
            moveFwdDir = (new Vector3(distV2.x, spine1.forward.y, distV2.y));
            Quaternion rot = Quaternion.LookRotation(moveFwdDir) * Quaternion.Euler(0, -65, 0);
            lastAimRot = Quaternion.Lerp(lastAimRot, Quaternion.LookRotation(new Vector3(distV2.x,0,distV2.y)) * Quaternion.Euler(0, -40, 0), deltaTime*2.0f);
            transform.rotation = lastAimRot;
            spine1.rotation = rot;
            
        }

        public override bool DashGetHurt()
        {
            if (curState == dieState) return false;
            if (!isAlarm)
            {
                AudioManager.SingletonInScene.PlaySound2D("Hunter_Death", 0.3f);
                hp = 0;
                animator.SetBool("Hurt", false);
                animator.SetBool("Dead", true);
                ChangeState(dieState);
                return true;
            }
            else
            {
                hp -= 10;
                BloodSplash.Play();
                if (hp > 0)
                {
                    AudioManager.SingletonInScene.PlaySound2D("Enemy_Hurt", 0.3f);
                    canHurt = false;
                    animator.SetBool("Hurt", true);
                    ChangeState(hurtState);

                }
                else
                {
                    AudioManager.SingletonInScene.PlaySound2D("Hunter_Death", 0.3f);
                    animator.SetBool("Hurt", false);
                    animator.SetBool("Dead", true);
                    ChangeState(dieState);
                }
            }
            if (isAim)
            {
                isAim = false;
                //transform.rotation = Quaternion.LookRotation(new Vector3(HurtDir.x, 0, HurtDir.z));
                enemyManager.UnSubLateAction(transform.name);
                crossBow.gameObject.SetActive(false);
                crossBow.parent = hand;
                crossBow.localPosition = crossBowPos;
                crossBow.localRotation = crossBowRot;
            }
            return false;
        }

        public override bool CheckGetHurt()
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
                AudioManager.SingletonInScene.PlaySound2D("Enemy_Hurt", 0.3f);
                hp -= 10;
                lastHurtHash = curCount;
                targetPos = enemyManager.Player.SelfTransform.position;
                HurtDir = new Vector3(targetPos.x - transform.position.x, 0, targetPos.z - transform.position.z);
                transform.rotation = Quaternion.LookRotation(HurtDir);
                Debug.Log("hurtttt face  " + HurtDir);
                BloodSplash.transform.rotation = Quaternion.LookRotation(HurtDir);
                BloodSplash.Play();
                Debug.Log("gettttttttttttttttt   hurt  " + isAim);
                if (hp > 0)
                {
                    canHurt = false;
                    animator.SetBool("Hurt", true);
                    ChangeState(hurtState);
                }
                else
                {
                    AudioManager.SingletonInScene.PlaySound2D("Hunter_Death", 0.3f);
                    animator.SetBool("Hurt", false);
                    animator.SetBool("Dead", true);
                    ChangeState(dieState);
                }
                if (isAim) {
                    isAim = false;
                    //transform.rotation = Quaternion.LookRotation(new Vector3(HurtDir.x, 0, HurtDir.z));
                    enemyManager.UnSubLateAction(transform.name);
                    crossBow.gameObject.SetActive(false);
                    crossBow.parent = hand;
                    crossBow.localPosition = crossBowPos;
                    crossBow.localRotation = crossBowRot;
                }
                return true;
            }

            return false;
        }
        public override bool CheckGetHurtInHurt()
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
                AudioManager.SingletonInScene.PlaySound2D("Enemy_Hurt", 0.3f);
                hp -= 10;
                lastHurtHash = curCount;
                HurtDir = new Vector3(targetPos.x - transform.position.x, 0, targetPos.z - transform.position.z);
                transform.rotation = Quaternion.LookRotation(new Vector3(HurtDir.x, 0, HurtDir.z));
                if (isAim)
                {
                    isAim = false;
                    enemyManager.UnSubLateAction(transform.name);
                    crossBow.gameObject.SetActive(false);
                    crossBow.parent = hand;
                    crossBow.localPosition = crossBowPos;
                    crossBow.localRotation = crossBowRot;
                }
                if (hp > 0)
                {
                    canHurt = false;
                    animator.SetTrigger("HurtAgain");
                    //ChangeState(hurtState);
                    Debug.Log("hurt time   " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                }
                else
                {
                    AudioManager.SingletonInScene.PlaySound2D("Hunter_Death", 0.3f);
                    animator.SetBool("Dead", true);
                    ChangeState(dieState);
                    return true;
                }
            }
            return false;
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
                    if (!PlayerInDistanceAfterClose())
                    {
                        ChangeState(idleState);
                    }
                    else {
                        isAim = true;
                        ChangeState(enemyDistantATKState);
                        animator.SetBool("Aim", true);
                        lastAimRot = Quaternion.LookRotation(moveFwdDir) * Quaternion.Euler(0, -60, 0);
                        transform.rotation = lastAimRot;
                        enemyManager.SubLateAction(transform.name, Aming);
                    }
                    canHurt = true;
                    stateStep = 0;
                }
            }
        }
        public override bool PlayerInDistanceAfterClose()   //在近距離接觸過後的判斷，少角度確認比較寬鬆
        {
            float distance = enemyManager.ArcherValue.SightDistance;
            //float angle = enemyManager.HunterValue.SightAngle;

            if (!Physics.Linecast(lookPos, enemyManager.Player.SelfTransform.position, 1 << LayerMask.NameToLayer("Barrier")))
            {
                Vector2 distV2 = new Vector2(enemyManager.Player.SelfTransform.position.x - transform.position.x, enemyManager.Player.SelfTransform.position.z - transform.position.z);
                float dist = Vector2.SqrMagnitude(distV2);
                if (dist <= distance * distance)
                {
                    targetPos = enemyManager.Player.SelfTransform.position;
                    return true;
                }
                Debug.Log("in distance 太遠看不到");
            }
            Debug.Log("in distance 有障礙物");
            return false;
        }

        public override void AllAlarm()
        {
            //if (!isAlarm && hp > 0)
            //{
            //    isAlarm = true;
            //    isAim = true;
            //    ChangeState(enemyDistantATKState);
            //    animator.SetBool("Aim", true);
            //    lastAimRot = Quaternion.LookRotation(targetDir) * Quaternion.Euler(0, -60, 0);
            //    transform.rotation = lastAimRot;
            //    enemyManager.SubLateAction(transform.name, Aming);
            //    crossBow.gameObject.SetActive(true);
            //}

        }

        public override void Reset()
        {
            hp = enemyManager.ArcherValue.Health;
            transform.position = oringinLoc;
            transform.rotation = Quaternion.LookRotation(oringinDir);
            animator.Play("Idle");
            animator.SetBool("Aim",false);
            animator.SetBool("Chase", false);
            animator.SetBool("Hurt", false);
            animator.SetBool("Look", false);
            animator.SetBool("Attack", false);
            animator.SetBool("Dead", false);
            patrolRoute.CurPointID = 0;
            playerPathIndex = 0;
            isAlarm = false;
            stateStep = 0;
            stateTime = .0f;
            deltaTime = .0f;
            suspectTime = .0f;
            idleTime = .0f;
            seeDelayTime = .0f;
            lookARoundNum = 0;
            sightStep = 0;
            distanceCase = 0; // 1:警覺  2:攻擊
            findingPath = false;
            pathOver = false;
            ChangeState(idleState);

        }
    }
}

