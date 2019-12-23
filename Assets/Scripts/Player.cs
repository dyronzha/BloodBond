using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BloodBond {
    public class Player : MonoBehaviour
    {
        float deltaTime;

        Transform selfTransform;
        Animator animator;

        [SerializeField]
        PlayerValue infoValue;
        PlayerState curState;
        IdleState idleState;
        MoveState moveState;
        DashState dashState;
        NormalComboATKState normalComboAtkState;
        HurtState hurtState;
        InputSystem input;

        // Start is called before the first frame update
        void Awake()
        {
            animator = GetComponent<Animator>();
            input = new InputSystem();

            idleState = new IdleState(this);
            moveState = new MoveState(this);
            dashState = new DashState(this);
            normalComboAtkState = new NormalComboATKState(this);
            hurtState = new HurtState(this);
            curState = idleState;
        }

        // Update is called once per frame
        void Update()
        {
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("Karol@Atack1")) 
                Debug.Log("Karol@Atack1   " +  animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Karol@Atack2"))
                Debug.Log("Karol@Atack2   " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Karol@Atack3"))
                Debug.Log("Karol@Atack3   " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

            deltaTime = Time.deltaTime;
            curState.Update();
            
        }

        void CheckState() { 
            
        }

        public void Movement()
        {
            Vector3 _dir = new Vector3(input.GetHMoveAxis(), .0f, input.GetVMoveAxis());
            Vector3 nextPos = selfTransform.position + infoValue.MoveSpeed * deltaTime * _dir;
            if (!Physics.Linecast(selfTransform.position, nextPos, 1 << LayerMask.NameToLayer("Obstacle")))
            {
                selfTransform.position = nextPos;
            }
        }
        public void Dash() { 
            
        }
        public void NormalComboAttack() { 
        
        }
    }
}

