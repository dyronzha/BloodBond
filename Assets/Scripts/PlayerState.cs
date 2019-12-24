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

    public class IdleState : PlayerState
    {
        public IdleState(Player p) : base(p)
        {

        }
        public override void Update()
        {
            Debug.Log("idle");
            if (player.CheckHurt()) return;
            if (player.CheckNormalComboAttack()) return;
            player.IdleCheckMove();
        }
    }

    public class MoveState : PlayerState
    {

        public MoveState(Player p) : base(p)
        {

        }
        public override void Update()
        {
            Debug.Log("run");
            if (player.MoveCheckDodge()) return;
            if (player.CheckHurt()) return;
            if (player.CheckNormalComboAttack()) return;
            player.Movement();
        }
    }

    public class DodgeState : PlayerState
    {
        public DodgeState(Player p) : base(p)
        {
            
        }
        public override void Update()
        {
            Debug.Log("dodge");
            player.Dodge();
        }
        
    }

    public class DashState : PlayerState
    {
        public DashState(Player p) : base(p)
        {
            
        }
        public override void Update()
        {
            player.Dash();
        }
    }

    public class NormalComboATKState : PlayerState
    {
        int _curCombo = 0;
        int _maxCombo;
        public NormalComboATKState(Player p, int maxCombo) : base(p)
        {
            _maxCombo = maxCombo;
        }
        public override void Update()
        {
            if (player.CheckHurt() || player.MoveCheckDodge()) {
                _curCombo = 0;
            }
            if (player.CheckNextCombo() && player.NormalComboAttack(_curCombo)) _curCombo++;

        }
    }

    public class HurtState : PlayerState
    {
        public HurtState(Player p) : base(p)
        {

        }
    }

    public class DieState : PlayerState
    {
        public DieState(Player p) : base(p)
        {

        }
    }
}

