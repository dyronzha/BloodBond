using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BloodBond {
    public class Player : MonoBehaviour
    {
        int stateStep = 0;
        float stateTime, deltaTime;

        float inputMoveX, inputMoveY;
        Vector3 moveForward = new Vector3(0, 0, 0), lastDir;

        bool inDodgeCD = false;
        float dodgeOffset, dodgeCD;

        Camera mainCamera;
        Transform selfTransform;
        Animator animator;

        [SerializeField]
        PlayerValue infoValue;
        PlayerState curState;
        IdleState idleState;
        MoveState moveState;
        DodgeState dodgeState;
        DashState dashState;
        NormalComboATKState normalComboAtkState;
        HurtState hurtState;
        InputSystem input;



        // Start is called before the first frame update
        void Awake()
        {
            selfTransform = transform;
            mainCamera = Camera.main;
            animator = GetComponent<Animator>();
            input = new InputSystem();

            idleState = new IdleState(this);
            moveState = new MoveState(this);
            dodgeState = new DodgeState(this);
            dashState = new DashState(this);
            normalComboAtkState = new NormalComboATKState(this, 3);
            hurtState = new HurtState(this);
            curState = idleState;
        }

        // Update is called once per frame
        void Update()
        {
            //if(animator.GetCurrentAnimatorStateInfo(0).IsName("Karol@Atack1")) 
            //    Debug.Log("Karol@Atack1   " +  animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            //else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Karol@Atack2"))
            //    Debug.Log("Karol@Atack2   " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            //else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Karol@Atack3"))
            //    Debug.Log("Karol@Atack3   " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

            deltaTime = Time.deltaTime;
            curState.Update();
            if (inDodgeCD) CountDodgeCD();
        }

        public void ChangeState(PlayerState nextState) {
            curState = nextState;
            stateStep = 0;
            stateTime = .0f;
        }

        public void SetAnimatorBool(string name, bool v) {
            animator.SetBool(name, v);
        }

        public void IdleCheckMove() {
            Vector3 _dir = new Vector3(input.GetHMoveAxis(), .0f, input.GetVMoveAxis());
            if (_dir.sqrMagnitude > 0.1f && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
                animator.SetBool("Run", true);
                ChangeState(moveState);
            }
        }
        public void Movement()
        {
            Vector3 baseFWD, baseRight;
            baseFWD = mainCamera.transform.forward;
            baseRight = mainCamera.transform.right;

            Vector3 _dir = new Vector3(input.GetHMoveAxis(), .0f, input.GetVMoveAxis());
            if (_dir.sqrMagnitude > 0.1f)
            {
                moveForward = (new Vector3(_dir.x * baseRight.x, 0, _dir.x * baseRight.z)
                                  + new Vector3(_dir.z * baseFWD.x, 0, _dir.z * baseFWD.z)).normalized;
                Vector3 nextPos = selfTransform.position + infoValue.MoveSpeed * deltaTime * moveForward;

                float difAngle = Vector3.Angle(selfTransform.forward, moveForward);
                selfTransform.rotation = Quaternion.Lerp(selfTransform.rotation, Quaternion.LookRotation(moveForward), deltaTime * infoValue.RotateSpeed);
                if (difAngle < 45.0f && !Physics.Linecast(selfTransform.position, nextPos, 1 << LayerMask.NameToLayer("Obstacle"))) {
                    selfTransform.position = nextPos;
                }
            }
            else {
                animator.SetBool("Run", false);
                ChangeState(idleState);
            }
        }
        public bool MoveCheckDodge() {
            float x = input.GetHMoveAxis();
            float z = input.GetVMoveAxis();
            if (input.GetDodgeInput() && (x * x + z * z) > 0.1f && !inDodgeCD) {
                lastDir = new Vector3(x, .0f, z);
                animator.SetBool("Dodge", true);
                animator.SetBool("Run", false);
                ChangeState(dodgeState);
                return true;
            }
            else return false;
        }

        public void Dodge() {
            if (stateStep == 0)
            {
                stateStep++;
                Vector3 baseFWD, baseRight;
                baseFWD = mainCamera.transform.forward;
                baseRight = mainCamera.transform.right;
                moveForward = (new Vector3(lastDir.x * baseRight.x, 0, lastDir.x * baseRight.z)
                                  + new Vector3(lastDir.z * baseFWD.x, 0, lastDir.z * baseFWD.z)).normalized;

                transform.rotation = Quaternion.LookRotation(moveForward);

            }
            else if (stateStep == 1)
            {

                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
                if ((aniInfo.IsName("Dodge")))
                {
                    stateStep++;
                    dodgeOffset = 1.0f;
                }
            }
            else {
                stateTime += deltaTime;
                if (stateTime < 0.3f)
                {
                    float dSpeed = infoValue.DodgeSpeed * dodgeOffset;
                    dodgeOffset = Mathf.Clamp(dodgeOffset - deltaTime * 5.0f, .0f, 1.0f);
                    Debug.Log("dodge offset  " + dodgeOffset);
                    Vector3 nextPos = selfTransform.position + dSpeed * deltaTime * moveForward;
                    if (!Physics.Linecast(selfTransform.position, nextPos, 1 << LayerMask.NameToLayer("Obstacle")))
                    {
                        selfTransform.position = nextPos;
                    }
                }
                else {
                    inDodgeCD = true;
                    animator.SetBool("Dodge", false);
                    if (input.GetMove())
                    {
                        animator.SetBool("Run", true);
                        ChangeState(moveState);
                    }
                    else
                    {
                        animator.SetBool("Run", false);
                        ChangeState(idleState);
                    }
                }

            }

        }
        void CountDodgeCD() {
            dodgeCD += deltaTime;
            if (dodgeCD > 0.5f) {
                dodgeCD = .0f;
                inDodgeCD = false;
            }
        }

        public bool CheckNormalComboAttack()
        {
            if (input.GetNormalComboATK())
            {
                animator.SetBool("Run", false);
                animator.SetBool("NormalComboATK", true);
                ChangeState(normalComboAtkState);
                return true;
            }
            else return false;
        }
        public bool CheckNextCombo() {
            if (input.GetNormalComboATK()) return true;
            else return false;
        }
        public bool NormalComboAttack(int comboCount) {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateStep == 0) {
                if (aniInfo.IsName("Combo" + comboCount.ToString())) {
                    stateStep++;
                }
            }
            else if (stateStep == 1) {
                if (aniInfo.normalizedTime > 0.7f) {
                    if(aniInfo.normalizedTime < 0.95f) return true;
                    animator.SetBool("NormalComboATK", false);
                    ChangeState(idleState);
                }
            }
            return false;
        }


        public void Dash()
        {

        }

        public bool CheckHurt() {
            return false;
        }
    }
}

