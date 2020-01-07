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
        float dodgeOffset, dodgeCD, dodgeTime;

        bool invincible = false;
        float invincibleTime = .0f;

        bool showATKCollider = false;
        public Collider attackCollider;

        bool showDashEffect = false;
        Transform dashOrientEffect;

        KarolShader karolShader;
        public GameObject PhantomCreate;

        public UnityEngine.Experimental.VFX.VisualEffect VFX_Teleport;

        Camera mainCamera;
        Transform selfTransform;
        public Transform SelfTransform {
            get { return selfTransform; }
        }
        CapsuleCollider hurtAreaCollider;
        Animator animator;

        [SerializeField]
        PlayerValue infoValue;
        PlayerState curState;
        PlayerIdleState idleState;
        PlayerMoveState moveState;
        PlayerDodgeState dodgeState;
        PlayerDashState dashState;
        PlayerNormalComboATKState normalComboAtkState;
        PlayerHurtState hurtState;
        InputSystem input;



        // Start is called before the first frame update
        void Awake()
        {
            selfTransform = transform;
            mainCamera = Camera.main;
            hurtAreaCollider = GetComponent<CapsuleCollider>();
            animator = GetComponent<Animator>();
            input = new InputSystem();

            dashOrientEffect = transform.Find("DashOrientEffect");
            dashOrientEffect.gameObject.SetActive(false);

            idleState = new PlayerIdleState(this);
            moveState = new PlayerMoveState(this);
            dodgeState = new PlayerDodgeState(this);
            dashState = new PlayerDashState(this);
            normalComboAtkState = new PlayerNormalComboATKState(this, 2);
            hurtState = new PlayerHurtState(this);
            curState = idleState;

            karolShader = GetComponent<KarolShader>();
        }

        // Update is called once per frame
        void Update() {
            deltaTime = Time.deltaTime;

            curState.Update();
            if (inDodgeCD) CountDodgeCD();
            if (invincible) CountInvincibleTime();
        }

        public void ChangeState(PlayerState nextState) {
            curState = nextState;
            stateStep = 0;
            stateTime = .0f;
        }
        public void SetAnimatorBool(string name, bool v) {
            animator.SetBool(name, v);
        }

        //public bool CheckAniInfoState() {
        //    if (stateStep > 0) return true;
        //    else return false;
        //}

        //待機相關
        public void IdleCheckMove() {
            if (stateStep == 0)
            {
                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (aniInfo.IsName("Idle")) stateStep++;
            }
            else {
                Vector3 _dir = new Vector3(input.GetHMoveAxis(), .0f, input.GetVMoveAxis());
                if (_dir.sqrMagnitude > 0.1f)
                {
                    animator.SetBool("Run", true);
                    ChangeState(moveState);
                }
            }
        }

        //移動相關-----------------------------------------------------
        public void Movement()
        {
            if (stateStep == 0)
            {
                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (aniInfo.IsName("Run")) stateStep++;
            }
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
                if (difAngle < 45.0f && !Physics.Linecast(selfTransform.position, nextPos + 5.0f * infoValue.MoveSpeed * deltaTime * moveForward, 
                    1 << LayerMask.NameToLayer("Barrier")))
                {
                    selfTransform.position = nextPos;
                    
                }
            }
            else
            {
                animator.SetBool("Run", false);
                ChangeState(idleState);
            }
        }
        public bool MoveCheckDodge() {
            if (input.GetDodgeInput() && !inDodgeCD && stateStep > 0) {
                animator.SetBool("Dodge", true);
                ChangeState(dodgeState);
                return true;
            }
            else return false;
        }

        //閃避相關-----------------------------------------------------
        public void Dodge() {
            if (stateStep == 0)
            {
                stateStep++;
                //Vector3 baseFWD, baseRight;
                //baseFWD = mainCamera.transform.forward;
                //baseRight = mainCamera.transform.right;
                //moveForward = (new Vector3(lastDir.x * baseRight.x, 0, lastDir.x * baseRight.z)
                //                  + new Vector3(lastDir.z * baseFWD.x, 0, lastDir.z * baseFWD.z)).normalized;

                transform.rotation = Quaternion.LookRotation(moveForward);

            }
            else if (stateStep == 1)
            {

                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
                if ((aniInfo.IsName("Dodge")))
                {
                    stateStep++;
                    dodgeOffset = 1.0f;
                    karolShader.ChangeMaterial(6);
                    Instantiate(PhantomCreate, selfTransform.position + new Vector3(0.0f, 1.0f, 0.0f), transform.rotation);
                }
            }
            else {
                stateTime += deltaTime;
                if (stateTime - dodgeTime > 0.05f) {
                    dodgeTime = stateTime;
                    Instantiate(PhantomCreate, selfTransform.position + new Vector3(0.0f, 1.0f, 0.0f), transform.rotation);
                }
                
                if (stateTime < 0.3f)
                {
                    float dSpeed = infoValue.DodgeSpeed * dodgeOffset;
                    dodgeOffset = Mathf.Clamp(dodgeOffset - deltaTime * 5.0f, .0f, 1.0f);

                    

                    Debug.Log("dodge offset  " + dodgeOffset);
                    Vector3 nextPos = selfTransform.position + dSpeed * deltaTime * moveForward;
                    if (!Physics.Linecast(selfTransform.position, nextPos + dSpeed * deltaTime * moveForward, 1 << LayerMask.NameToLayer("Barrier")))
                    {
                        selfTransform.position = nextPos;
                    }
                }
                else {
                    inDodgeCD = true;
                    dodgeTime = .0f;
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

        //確認移動方向-----------------------------------------------------
        public bool GetMoveInput() {
            float x = input.GetHMoveAxis();
            float z = input.GetVMoveAxis();
            if ((x * x + z * z) > 0.1f)
            {
                Vector3 baseFWD, baseRight;
                lastDir = new Vector3(x, .0f, z);
                baseFWD = mainCamera.transform.forward;
                baseRight = mainCamera.transform.right;
                moveForward = (new Vector3(lastDir.x * baseRight.x, 0, lastDir.x * baseRight.z)
                                  + new Vector3(lastDir.z * baseFWD.x, 0, lastDir.z * baseFWD.z)).normalized;
                return true;
            }
            return false;
        }
        public bool GetMoveInput(float maxAngle)
        {
            float x = input.GetHMoveAxis();
            float z = input.GetVMoveAxis();
            if ((x * x + z * z) > 0.1f)
            {
                Vector3 baseFWD, baseRight;
                lastDir = new Vector3(x, .0f, z);
                baseFWD = mainCamera.transform.forward;
                baseRight = mainCamera.transform.right;
                moveForward = (new Vector3(lastDir.x * baseRight.x, 0, lastDir.x * baseRight.z)
                                  + new Vector3(lastDir.z * baseFWD.x, 0, lastDir.z * baseFWD.z)).normalized;



                Debug.Log(moveForward);
                float btwAngle = Vector3.SignedAngle(selfTransform.forward, moveForward, Vector3.up);

                if (Mathf.Abs(btwAngle) > maxAngle) {
                    moveForward = Quaternion.AngleAxis(maxAngle * Mathf.Sign(btwAngle), Vector3.up)* selfTransform.forward;
                    Debug.Log(moveForward + "   " + selfTransform.forward + "    " + btwAngle);
                }
                return true;
            }
            return false;
        }

        //combo相關-----------------------------------------------------
        public bool AttackCheckDodge()
        {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (input.GetDodgeInput() && GetMoveInput() && !inDodgeCD && aniInfo.IsTag("Attack") && aniInfo.normalizedTime > 0.15f)
            {
                animator.SetBool("Dodge", true);
                ChangeState(dodgeState);
                return true;
            }
            else return false;
        }
        public bool CheckNormalComboAttackInput()
        {
            if (input.GetNormalComboATK() && stateStep > 0)//animator.GetCurrentAnimatorStateInfo(0).IsName(state)
            {
                animator.SetBool("NormalComboATK", true);
                ChangeState(normalComboAtkState);
                return true;
            }
            else return false;
        }
        public bool CheckNextCombo() {
            if (input.GetNormalComboATK()) {

                return true;
            } 
            else return false;
        }
        public void NormalComboAttack(ref int comboCount, int maxCombo) {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateStep == 0) {
                if (aniInfo.IsName("Combo" + comboCount.ToString())) {
                    if (comboCount > 0 && moveForward.sqrMagnitude > 0.1f)  //接技的方向，第二下開始
                    {
                        selfTransform.rotation = Quaternion.LookRotation(moveForward);
                        
                    }
                    animator.applyRootMotion = !Physics.Raycast(selfTransform.position, selfTransform.forward, 0.5f, 1 << LayerMask.NameToLayer("Barrier"));
                    stateStep++;
                }
            }
            else if (stateStep == 1) {
                if (aniInfo.normalizedTime > 0.15f) {
                    if (aniInfo.normalizedTime < 0.55f) {
                        if (comboCount < maxCombo && input.GetNormalComboATK()) {
                            GetMoveInput(90.0f);
                            animator.SetTrigger("NextCombo");
                            comboCount++;
                            stateStep = 0;
                        }
                    }
                    else if (aniInfo.normalizedTime >= 0.7f)
                    {
                        comboCount = 0;
                        animator.SetBool("NormalComboATK", false);
                        ChangeState(idleState);
                    } 
                }
            }
        }

        public int GetAttackHash() {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            return aniInfo.fullPathHash;
        }

        public void EnableATKCollider() {
            attackCollider.enabled = true;
        }
        public void CloseATKCollider() {
            attackCollider.enabled = false;
        }

        public bool CheckDashInput() {
            if (input.GetDashInput() && stateStep >0)
            {
                ChangeState(dashState);
                animator.SetBool("Dash", true);
                return true;
            }
            else return false;
        }
        public void Dash()
        {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateStep == 0)
            {
                if (aniInfo.IsName("PreDash")) {
                    stateStep++;
                    Time.timeScale = 0.2f;
                    VFX_Teleport.playRate = 6.0f;
                    VFX_Teleport.SetInt("Number_of_Particles", 1000000);
                    VFX_Teleport.SetFloat("AttractDrag", 0.0f);
                } 
            }
            else if (stateStep == 1)
            {
                if (!GetMoveInput()) //沒有方向輸入
                {
                    if (showDashEffect)
                    {
                        dashOrientEffect.gameObject.SetActive(false);
                        showDashEffect = false;
                    }
                    if (!input.GetDashInput())
                    {
                        animator.SetBool("Dash", false);
                        ChangeState(idleState);
                        dashOrientEffect.gameObject.SetActive(false);
                        showDashEffect = false;
                        Time.timeScale = 1.0f;
                        VFX_Teleport.SetFloat("AttractDrag", 1.0f);
                        VFX_Teleport.SetInt("Number_of_Particles", 0);
                        return;
                    }
                    
                }
                else
                {
                    if (!showDashEffect)
                    {
                        dashOrientEffect.gameObject.SetActive(true);
                        showDashEffect = true;
                    }
                    if (!input.GetDashInput())
                    {
                        RaycastHit hit;
                        Vector3 pos = selfTransform.position + new Vector3(0, 1, 0);
                        Vector3 nextPos = pos + moveForward * 5.0f;

                        if (Physics.Linecast(pos, nextPos, out hit, infoValue.HurtAreaLayer | 1 << LayerMask.NameToLayer("Barrier")))
                        {
                            if (hit.transform.tag.CompareTo("Barrier") == 0)
                            {
                                Debug.Log("hiiiiiiiiiiiiit barrier   " + new Vector3(hit.point.x, 0, hit.point.z));
                                selfTransform.position = new Vector3(hit.point.x, 0, hit.point.z) - moveForward;
                            }
                            else
                            {
                                selfTransform.position = new Vector3(hit.transform.position.x, 0, hit.transform.position.z);
                            }
                        }
                        else selfTransform.position = new Vector3(nextPos.x,0, nextPos.z);

                        selfTransform.position = nextPos;
                        selfTransform.rotation = Quaternion.LookRotation(moveForward);
                        dashOrientEffect.gameObject.SetActive(false);
                        animator.SetTrigger("DashOver");
                        showDashEffect = false;
                        
                        stateStep++;

                        VFX_Teleport.SetFloat("AttractDrag", 1.0f);
                        VFX_Teleport.SetInt("Number_of_Particles", 0);
                        return;
                    }
                   
                    dashOrientEffect.rotation = Quaternion.LookRotation(moveForward);
                }
            }
            else if(stateStep == 2) {
                if (aniInfo.IsName("DashOver")) stateStep++;
            }
            else {
                if (aniInfo.normalizedTime > 0.95f) {
                    Time.timeScale = 1.0f;
                    animator.SetBool("Dash", false);
                    VFX_Teleport.SetFloat("AttractDrag", 1.0f);
                    VFX_Teleport.SetInt("Number_of_Particles", 0);
                    ChangeState(idleState);
                }
            }
        }

        public bool CheckGetHurt() {
            if (invincible) return false;
            Vector3 center = hurtAreaCollider.center;
            Vector3 point2 = selfTransform.position + center.x * selfTransform.right + center.z * selfTransform.forward;
            Vector3 point1 = point2 + new Vector3(0, hurtAreaCollider.height, 0);
            Debug.DrawLine(point1, point2, Color.red);
            Collider[] cols =  Physics.OverlapCapsule(point1, point2, hurtAreaCollider.radius, infoValue.HurtAreaLayer);
            if (cols != null && cols.Length > 0) {
                ChangeState(hurtState);
                animator.SetBool("Hurt", true);
                return true;
            }
                
            return false;
        }
        public void InHurt() {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateStep == 0)
            {
                if (aniInfo.IsName("Hurt")) stateStep++;
            }
            else {
                if (aniInfo.normalizedTime > 0.7f) {
                    animator.SetBool("Hurt", false);
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
                    invincible = true;
                }
            }
        }

        void CountInvincibleTime() {
            invincibleTime += deltaTime;
            if (invincibleTime > 0.7f) {
                invincible = false;
                invincibleTime = .0f;
            } 
        }
    }
}

