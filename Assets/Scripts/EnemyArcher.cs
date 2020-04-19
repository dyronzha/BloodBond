﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class EnemyArcher : EnemyBase
    {
        bool isAim = false;
        float attackBlankTime = .0f;
        EnemyDistantAttackState enemyDistantATKState;
        Transform spine1, hand, crossBow;
        Vector3 crossBowPos; 
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
        }

        public override bool FindPlayer()
        {
            Vector2 distV2 = new Vector2(enemyManager.Player.SelfTransform.position.x - transform.position.x, enemyManager.Player.SelfTransform.position.z - transform.position.z);
            moveFwdDir = (new Vector3(distV2.x, 0, distV2.y));
            if (Vector2.SqrMagnitude(distV2) <= enemyManager.ArcherValue.SightDistance * enemyManager.ArcherValue.SightDistance) {
                ChangeState(enemyDistantATKState);
                animator.SetBool("Aim",true);
                lastAimRot = Quaternion.LookRotation(moveFwdDir) * Quaternion.Euler(0, -60, 0);
                transform.rotation = lastAimRot;
                enemyManager.SubLateAction(Aming);
                crossBow.gameObject.SetActive(true);
                return true;
            }
            return false;
                
            if (PlayerInSight(lookDir, enemyManager.ArcherValue.SightDistance, enemyManager.ArcherValue.SightAngle)) //Physics.Raycast(lookPos, lookDir, 5.0f, 1 << LayerMask.NameToLayer("Player"))
            {
                Debug.Log("懷疑時間 " + seeDelayTime);
                seeDelayTime += deltaTime;
                if (seeDelayTime > enemyManager.HunterValue.SeeConfirmTime)
                {
                    seeDelayTime = .0f;
                    animator.SetTrigger("Alarm");
                    targetPos = enemyManager.Player.SelfTransform.position;
                    targetDir = new Vector3(targetPos.x - selfPos.x, 0, targetPos.z - selfPos.z);

                    Debug.Log("進遠程攻擊");
                    ChangeState(enemyDistantATKState);  
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
                    ChangeState(idleState);
                }
            }
        }

        public override void DistantAttack()
        {
            if (stateStep == 0)
            {
                attackBlankTime += deltaTime;

                if (attackBlankTime > 1.0f)
                {
                    attackBlankTime = .0f;
                    animator.SetBool("Attack", true);
                    crossBow.parent = null;
                    stateStep++;
                }
            }
            else if (stateStep == 1)
            {
                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (aniInfo.IsName("Attack"))
                {
                    crossBow.localScale = new Vector3(0.543f, 0.543f, 0.543f);
                    stateStep++;
                }
            }
            else if (stateStep == 2) {
                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (aniInfo.normalizedTime > 0.3f) {
                    crossBow.gameObject.SetActive(false);
                    crossBow.parent = hand;
                    crossBow.localPosition = crossBowPos;
                    crossBow.localRotation = crossBowRot;
                    crossBow.localScale = new Vector3(0.4f, 0.4f, 0.4f);
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
                    stateStep = 0;
                    if (!PlayerInDistanceAfterClose())
                    {
                        ChangeState(idleState);
                        isAim = false;
                        enemyManager.UnSubLateAction(Aming);
                        animator.SetBool("Aim", false);
                        transform.rotation = idleRot;
                    }
                    else crossBow.gameObject.SetActive(true);
                }
            }
        }
        public void ClearAttackBlankTime() {
            attackBlankTime = .0f;
        }
        void Aming() {
            Vector2 distV2 = new Vector2(enemyManager.Player.SelfTransform.position.x - transform.position.x, enemyManager.Player.SelfTransform.position.z - transform.position.z);
            moveFwdDir = (new Vector3(distV2.x, 0, distV2.y));
            Quaternion rot = Quaternion.LookRotation(moveFwdDir) * Quaternion.Euler(0, -60, 0);
            lastAimRot = Quaternion.Lerp(lastAimRot, rot * Quaternion.Euler(0, -10, 0), deltaTime);
            transform.rotation = lastAimRot;
            spine1.rotation = rot;
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
                hp -= 0;
                lastHurtHash = curCount;

                HurtDir = new Vector3(targetDir.x - transform.position.x, targetDir.y - transform.position.y, targetDir.z - transform.position.z);
                BloodSplash.transform.rotation = Quaternion.LookRotation(HurtDir);
                BloodSplash.Play();

                if (hp > 0)
                {
                    canHurt = false;
                    animator.SetBool("Hurt", true);
                    ChangeState(hurtState);
                }
                else
                {
                    animator.SetBool("Hurt", false);
                    animator.SetBool("Dead", true);
                    ChangeState(dieState);
                }
                if (isAim) {
                    isAim = false;
                    transform.rotation = Quaternion.LookRotation(new Vector3(HurtDir.x, 0, HurtDir.z));
                    enemyManager.UnSubLateAction(Aming);
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
                hp -= 0;
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

                bool canSee = PlayerInDistanceAfterClose();
                if (aniInfo.normalizedTime > (0.85f))
                {
                    animator.SetBool("Hurt", false);
                    if (!PlayerInDistanceAfterClose())
                    {
                        ChangeState(idleState);
                    }
                    else {
                        ChangeState(enemyDistantATKState);
                        animator.SetBool("Aim", true);
                        lastAimRot = Quaternion.LookRotation(moveFwdDir) * Quaternion.Euler(0, -60, 0);
                        transform.rotation = lastAimRot;
                        enemyManager.SubLateAction(Aming);
                    }
                    canHurt = true;
                    lastHurtHash = 999;
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
    }
}

