using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class EnemyArcher : EnemyBase
    {
        bool aim = false;
        float attackBlankTime = .0f;
        EnemyDistantAttackState enemyDistantATKState;
        Transform spine1;
        Quaternion lastAimRot;

        public EnemyArcher(Transform t, EnemyManager manager): base(t, manager) {
            
            spine1 = transform.Find("mixamorig:Hips").Find("mixamorig:Spine").Find("mixamorig:Spine1");
        }

        public override void LateUpdate(float dtTime)
        {
            if(aim)Aming();
        }

        public override void Init()
        {
            idleState = new EnemyIdleState(this);
            lookAroundState = new EnemyLookAroundState(this);

            hurtState = new EnemyHurtState(this);

            dieState = new EnemyDieState(this);

            enemyDistantATKState = new EnemyDistantAttackState(this);
            ChangeState(idleState);
        }

        public override bool FindPlayer()
        {
            Vector2 distV2 = new Vector2(enemyManager.Player.SelfTransform.position.x - transform.position.x, enemyManager.Player.SelfTransform.position.z - transform.position.z);
            moveFwdDir = (new Vector3(distV2.x, 0, distV2.y));
            if (Vector2.SqrMagnitude(distV2) <= enemyManager.ArcherValue.SightDistance * enemyManager.ArcherValue.SightDistance) {
                ChangeState(enemyDistantATKState);
                animator.SetBool("Aim",true);
                transform.rotation = Quaternion.LookRotation(moveFwdDir) * Quaternion.Euler(0, -60, 0);
                aim = true;
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
            

            
            //if (stateStep == 0) {
            //    attackBlankTime += deltaTime;

            //    if (attackBlankTime > 1.0f) { 

            //    }
            //}
        }
        void Aming() {
            Vector2 distV2 = new Vector2(enemyManager.Player.SelfTransform.position.x - transform.position.x, enemyManager.Player.SelfTransform.position.z - transform.position.z);
            moveFwdDir = (new Vector3(distV2.x, 0, distV2.y));
            //transform.rotation = Quaternion.LookRotation(moveFwdDir) * Quaternion.Euler(0, -60, 0);
            lastAimRot = Quaternion.LookRotation(moveFwdDir) * Quaternion.Euler(0, -60, 0);
            spine1.rotation = lastAimRot;
        }
    }
}

