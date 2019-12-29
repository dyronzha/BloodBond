using System.Collections;
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
            Debug.Log("player state");
        }

    }

    public class EnemyIdleState : EnemyBaseState{
        public EnemyIdleState(EnemyBase enemy) : base(enemy) { 
            
        }

    }
    public class EnemyRambleState : EnemyBaseState
    {
        public EnemyRambleState(EnemyBase enemy) : base(enemy)
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
            
        }

    }
    public class EnemyChaseState : EnemyBaseState
    {
        public EnemyChaseState(EnemyBase enemy) : base(enemy)
        {

        }

    }
    public class EnemyAttackState : EnemyBaseState
    {
        public EnemyAttackState(EnemyBase enemy) : base(enemy)
        {

        }

    }
    public class EnemyHurtState : EnemyBaseState
    {
        public EnemyHurtState(EnemyBase enemy) : base(enemy)
        {

        }

    }
    public class EnemyYellState : EnemyBaseState
    {
        public EnemyYellState(EnemyBase enemy) : base(enemy)
        {

        }

    }
    public class EnemyDieState : EnemyBaseState
    {
        public EnemyDieState(EnemyBase enemy) : base(enemy)
        {

        }

    }
}


