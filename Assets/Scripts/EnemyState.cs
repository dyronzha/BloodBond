﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class EnemyBaseState : ActorState
    {
        protected EnemyBase enemyBase;
        public EnemyBaseState(EnemyBase enemy)
        {
            enemyBase = enemy;
        }
        public override void Update()
        {
            
        }

    }

    public class EnemyIdleState : EnemyBaseState{
        public EnemyIdleState(EnemyBase enemy) : base(enemy) { 
            
        }
        public override void Update()
        {
            //if (!enemyBase.FindPlayer())enemyBase.Idle();
            if (enemyBase.CheckGetHurt() || enemyBase.FindPlayer()) return;
            enemyBase.Idle();
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
            if (enemyBase.CheckGetHurt() || enemyBase.FindPlayer()) {
                enemyBase.SetAniBool("Patrol", false);
                return;
            }
            enemyBase.Patroling();
            //if (enemyBase.FindPlayer()) enemyBase.SetAniBool("Patrol", false);
            //else enemyBase.Patroling();
        }

    }
    public class EnemyLookAroundState : EnemyBaseState
    {
        public EnemyLookAroundState(EnemyBase enemy) : base(enemy)
        {

        }
        public override void Update()
        {
            if (enemyBase.CheckGetHurt() || enemyBase.FindPlayer()) {
                enemyBase.SetAniBool("Look", false);
                return;
            }
            enemyBase.LookAround();
            //if (enemyBase.FindPlayer()) enemyBase.SetAniBool("Look", false);
            //else enemyBase.LookAround();
        }

    }
    public class EnemyChaseState : EnemyBaseState
    {
        public EnemyChaseState(EnemyBase enemy) : base(enemy)
        {

        }
        public override void Update()
        {
            enemyBase.Chasing();   
        }
    }
    public class EnemyAttackState : EnemyBaseState
    {
        public EnemyAttackState(EnemyBase enemy) : base(enemy)
        {

        }
        public override void Update()
        {

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
    public class EnemySuspectIdleState : EnemyBaseState
    {
        public EnemySuspectIdleState(EnemyBase enemy) : base(enemy)
        {

        }
        public override void Update()
        {
            
        }
    }
    public class EnemySuspectMoveState : EnemyBaseState
    {
        public EnemySuspectMoveState(EnemyBase enemy) : base(enemy)
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


