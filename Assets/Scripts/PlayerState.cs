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
        public bool hasEnableCollider = false;
        int _curCombo = 0;
        public int CurComboCount { get { return _curCombo; } }
        int _maxCombo;
        float[] colliderEnableTimes;
        public float currentColliderTime { get { return colliderEnableTimes[_curCombo]; } }
        Collider[] AtkColliders;
        public Collider[] ATKColliders { set { AtkColliders = value; } }
        public Collider curATKCollider { get { return AtkColliders[_curCombo]; } }
        public Collider lastATKCollider { get { return AtkColliders[_curCombo-1]; } }
        public struct AttackAreaInfo {
            public Vector3 pos;
            public Vector3 size;
        }

        //用於在玩家攻擊時判斷有沒有打到敵人的overlap
        AttackAreaInfo[] attackAreaInfo;
        public AttackAreaInfo curATKAreaInfo { get { return attackAreaInfo[_curCombo]; } }

        public PlayerNormalComboATKState(Player p, int maxCombo) : base(p)
        {
            _maxCombo = maxCombo;
        }
        public PlayerNormalComboATKState(Player p, int maxCombo, float[] collEnabTimes) : base(p)
        {
            _maxCombo = maxCombo;
            colliderEnableTimes = collEnabTimes;
        }
        public override void Update()
        {
            if (player.CheckGetHurt() || player.AttackCheckDodge()) {
                _curCombo = 0;
                player.SetAnimatorBool("NormalComboATK", false);
                curATKCollider.enabled = false;
                hasEnableCollider = false;
                return;
            }
            player.NormalComboAttack(ref _curCombo, _maxCombo);

        }
        
        //用於在玩家攻擊時判斷有沒有打到敵人的overlap
        public void SetCollider(Collider[] colliders) {
            attackAreaInfo = new AttackAreaInfo[colliders.Length];
            for (int i = 0; i < attackAreaInfo.Length; i++) {
                attackAreaInfo[i] = new AttackAreaInfo();
                attackAreaInfo[i].pos = colliders[i].bounds.center;
                attackAreaInfo[i].size = colliders[i].bounds.extents;
            }
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

