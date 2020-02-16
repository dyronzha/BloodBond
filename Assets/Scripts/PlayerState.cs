using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodBond {
    public class PlayerState : ActorState
    {
        protected Player player;
        public PlayerState(Player p)
        {
            player = p;
        }
        public override void Update()
        {
            Debug.Log("player state");
        }
    }

    public class PlayerIdleState : PlayerState
    {
        public PlayerIdleState(Player p) : base(p)
        {

        }
        public override void Update()
        {
            if (player.CheckGetHurt()) return;
            if (player.CheckNormalComboAttackInput() || player.CheckDashInput()) return;
            player.IdleCheckMove();
        }
    }

    public class PlayerMoveState : PlayerState
    {

        public PlayerMoveState(Player p) : base(p)
        {

        }
        public override void Update()
        {
            if (player.CheckGetHurt() || player.MoveCheckDodge() || player.CheckNormalComboAttackInput() || player.CheckDashInput())
            {
                player.SetAnimatorBool("Run", false);
                return;
            }
            player.Movement();

        }
    }

    public class PlayerDodgeState : PlayerState
    {
        public PlayerDodgeState(Player p) : base(p)
        {
            
        }
        public override void Update()
        {
            Debug.Log("dodge");
            player.Dodge();
        }
        
    }

    public class PlayerDashState : PlayerState
    {
        public PlayerDashState(Player p) : base(p)
        {
            
        }
        public override void Update()
        {
            player.Dash();
        }
    }

    public class PlayerNormalComboATKState : PlayerState
    {
        int _curCombo = 0;
        int _maxCombo;
        public PlayerNormalComboATKState(Player p, int maxCombo) : base(p)
        {
            _maxCombo = maxCombo;
        }
        public override void Update()
        {
            if (player.CheckGetHurt() || player.AttackCheckDodge()) {
                _curCombo = 0;
                player.SetAnimatorBool("NormalComboATK", false);
                return;
            }
            player.NormalComboAttack(ref _curCombo, _maxCombo);

        }
    }

    public class PlayerHurtState : PlayerState
    {
        public PlayerHurtState(Player p) : base(p)
        {

        }
        public override void Update() {
            player.InHurt();
        }
    }

    public class PlayerDieState : PlayerState
    {
        public PlayerDieState(Player p) : base(p)
        {

        }

    }
}

