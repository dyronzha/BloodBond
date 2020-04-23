using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class EnemyBaseState : ActorState
    {
        protected bool returnOnce = false;
        protected EnemyBase enemyBase;
        public EnemyBaseState(EnemyBase enemy)
        {
            enemyBase = enemy;
        }
        public override void Update()
        {
            
        }
        public virtual void GoAlarm()
        {
            returnOnce = true;
        }
    }

    public class EnemyIdleState : EnemyBaseState{
        public EnemyIdleState(EnemyBase enemy) : base(enemy) { 
            
        }
        public override void Update()
        {
            //if (!enemyBase.FindPlayer())enemyBase.Idle();
            if (returnOnce)
            {
                returnOnce = false;
                return;
            }
            if (enemyBase.CheckGetHurt() || enemyBase.FindPlayer()) return;
            
            enemyBase.Idle();
        }
        public override void GoAlarm()
        {
            returnOnce = true;
        }
    }
    public class EnemyRambleState : EnemyBaseState
    {
        public EnemyRambleState(EnemyBase enemy) : base(enemy)
        {

        }
        public override void Update()
        {

        }
    }
    public class EnemyPatrolState : EnemyBaseState
    {
        public EnemyPatrolState(EnemyBase enemy) : base(enemy)
        {

        }
        public override void Update()
        {
            if (returnOnce)
            {
                returnOnce = false;
                return;
            }
            if (enemyBase.CheckGetHurt() || enemyBase.FindPlayer()) {
                enemyBase.SetAniBool("Patrol", false);
                return;
            }
            enemyBase.Patroling();
            //if (enemyBase.FindPlayer()) enemyBase.SetAniBool("Patrol", false);
            //else enemyBase.Patroling();
        }
        public override void GoAlarm() {
            enemyBase.SetAniBool("Patrol", false);
            returnOnce = true;
        }
    }
    public class EnemyLookAroundState : EnemyBaseState
    {
        public EnemyLookAroundState(EnemyBase enemy) : base(enemy)
        {

        }
        public override void Update()
        {
            if (returnOnce)
            {
                returnOnce = false;
                return;
            }
            if (enemyBase.CheckGetHurt() || enemyBase.FindPlayer()) {
                enemyBase.SetAniBool("Look", false);
                return;
            }

            enemyBase.LookAround();

            //if (enemyBase.FindPlayer()) enemyBase.SetAniBool("Look", false);
            //else enemyBase.LookAround();
        }
        public override void GoAlarm()
        {
            enemyBase.SetAniBool("Look", false);
            returnOnce = true;
        }

    }
    public class EnemySuspectIdleState : EnemyBaseState
    {
        public EnemySuspectIdleState(EnemyBase enemy) : base(enemy)
        {

        }
        public override void Update()
        {
            if (returnOnce)
            {
                returnOnce = false;
                return;
            }
            if (enemyBase.CheckGetHurt() || enemyBase.FindPlayerInSuspect())
            {
                return;
            }

            enemyBase.SuspectIdle();
        }
        public override void GoAlarm()
        {
            returnOnce = true;
        }
    }
    public class EnemySuspectMoveState : EnemyBaseState
    {
        public EnemySuspectMoveState(EnemyBase enemy) : base(enemy)
        {

        }
        public override void Update()
        {
            if (returnOnce)
            {
                returnOnce = false;
                return;
            }
            if (enemyBase.CheckGetHurt() || enemyBase.FindPlayerInSuspect())
            {
                enemyBase.SetAniBool("Patrol", false);
                return;
            }

            enemyBase.SuspectMove();
        }
        public override void GoAlarm()
        {
            enemyBase.SetAniBool("Patrol", false);
            returnOnce = true;
        }
    }
    public class EnemySuspectLookAroundState : EnemyBaseState
    {
        public EnemySuspectLookAroundState(EnemyBase enemy) : base(enemy)
        {

        }
        public override void Update()
        {
            if (returnOnce)
            {
                returnOnce = false;
                return;
            }
            if (enemyBase.CheckGetHurt() || enemyBase.FindPlayerInSuspect())
            {
                enemyBase.SetAniBool("Look", false);
                return;
            }

            enemyBase.SuspectLookAround();
        }
        public override void GoAlarm()
        {
            enemyBase.SetAniBool("Look", false);
            returnOnce = true;
        }
    }
    public class EnemyChaseState : EnemyBaseState
    {
        public EnemyChaseState(EnemyBase enemy) : base(enemy)
        {

        }
        public override void Update()
        {
            //是否看到玩家在chasing判斷
            if (enemyBase.CheckGetHurt()) {
                enemyBase.SetAniBool("Chase", false);
                return;
            }
            enemyBase.Chasing();   
        }
    }
    public class EnemyComboAttackState : EnemyBaseState
    {
        public bool hasEnableCollider = false;
        int curCombo = 0;
        public int CurComboCount { get { return curCombo; } }
        int maxCombo;
        float[] colliderEnableTimes;
        public float currentColliderTime { get { return colliderEnableTimes[curCombo]; } }
        float[] colliderDisableTimes;
        public float currentDisColliderTime { get { return colliderDisableTimes[curCombo]; } }
        Collider[] AtkColliders;
        public Collider[] ATKColliders { set { AtkColliders = value; } }
        public Collider curATKCollider { get { return AtkColliders[curCombo]; } }
        public Collider lastATKCollider { get { return AtkColliders[Mathf.Clamp(curCombo - 1,0, AtkColliders.Length-1)]; } }

        public EnemyComboAttackState(EnemyBase enemy, int _maxCombo, float[] _collEnabTimes, float[] _collDisableTimes) : base(enemy)
        {
            maxCombo = _maxCombo;
            colliderEnableTimes = _collEnabTimes;
            colliderDisableTimes = _collDisableTimes;
        }
        public override void Update()
        {
            //攻擊完再判斷與玩家距離
            if (enemyBase.CheckGetHurt()) {
                enemyBase.SetAniBool("Attack", false);
                AtkColliders[0].enabled = false;
                curCombo = 0;
                return;
            }
            enemyBase.ComboAttack(ref curCombo, maxCombo);
        }
    }
    public class EnemyDistantAttackState : EnemyBaseState
    {
        public EnemyDistantAttackState(EnemyBase enemy) : base(enemy)
        {
            
        }
        public override void Update()
        {
            if (enemyBase.CheckGetHurt())
            {
                enemyBase.SetAniBool("Aim", false);
                enemyBase.SetAniBool("Attack", false);
                return;
            }
            enemyBase.DistantAttack();
        }
    }
    public class EnemyGiveUpState : EnemyBaseState
    {
        public EnemyGiveUpState(EnemyBase enemy) : base(enemy)
        {

        }
        public override void Update()
        {
            if (returnOnce)
            {
                returnOnce = false;
                return;
            }
            Debug.Log("in give up");
            if (enemyBase.CheckGetHurt() || enemyBase.FindPlayer())
            {
                enemyBase.SetAniBool("Patrol", false);
                return;
            }
            Debug.Log("give up update");

            enemyBase.GiveUp();
        }
        public override void GoAlarm()
        {
            enemyBase.SetAniBool("Patrol", false);
            returnOnce = true;
        }
    }

    public class EnemyHurtState : EnemyBaseState
    {
        public EnemyHurtState(EnemyBase enemy) : base(enemy)
        {

        }
        public override void Update()
        {
            enemyBase.InHurt();
        }
    }
    public class EnemyYellState : EnemyBaseState
    {
        public EnemyYellState(EnemyBase enemy) : base(enemy)
        {

        }
        public override void Update()
        {

        }
    }


    public class EnemyDieState : EnemyBaseState
    {
        public EnemyDieState(EnemyBase enemy) : base(enemy)
        {

        }
        public override void Update()
        {
            enemyBase.Dead();
        }
    }
}


