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
            if (player.CheckHurt()) return;
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
            Debug.Log("move state");
            if (player.MoveCheckDodge()) return;
            if (player.CheckHurt()) return;
            player.Movement();
        }
    }

    public class DodgeState : PlayerState
    {
        public DodgeState(Player p) : base(p)
        {
            player.Dodge();
        }
    }

    public class DashState : PlayerState
    {
        public DashState(Player p) : base(p)
        {
            player.Dash();
        }
    }

    public class NormalComboATKState : PlayerState
    {
        public NormalComboATKState(Player p) : base(p)
        {

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

