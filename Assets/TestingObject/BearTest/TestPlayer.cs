using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour{
    public bool hasParent;
    float inputMoveX, inputMoveY;
    Transform model;
    Animator animator;

    bool freeMode = false;

    Collider collider;
    Rigidbody rigidBody;

    public float speed;
    public float rotateAngle;
    public float rotateSpeed;
    public Camera mainCamera;

    bool During_Dialogue = false;



    // Start is called before the first frame update
    private void Awake()
    {
        collider = GetComponent<Collider>();
        if (!hasParent)
        {
            animator = GetComponent<Animator>();
            rigidBody = GetComponent<Rigidbody>();
        }
        else {
            model = transform.GetChild(0);
            animator = model.GetComponent<Animator>();
            rigidBody = GetComponent<Rigidbody>();
        }


        //playerInput = ReInput.players.GetPlayer(0);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (During_Dialogue == false) {
            Move();

            if (freeMode) {
                if (Input.GetButtonDown("FreeMode")) {
                    freeMode = false;
                    rigidBody.useGravity = true;
                }
                if (Input.GetButton("Down"))
                {
                    rigidBody.velocity = new Vector3(0, -8, 0);
                }
                else if (Input.GetButton("Up"))
                {
                    rigidBody.velocity = new Vector3(0, 8, 0);

                }
                else {
                    rigidBody.velocity = new Vector3(0, 0, 0);
                }
            }
            else
            {
                if (Input.GetButtonDown("FreeMode"))
                {
                    freeMode = true;
                    rigidBody.useGravity = false;
                }
            }
        }

    }

    private void FixedUpdate()
    {
        //float inputV = Mathf.Clamp01(inputMoveX * inputMoveX + inputMoveY * inputMoveY);

        //if (inputV > 0.15f)
        //{
        //    animator.SetFloat("moveSpeed", inputV);
        //    Vector3 moveForward = (new Vector3(inputMoveX * mainCamera.transform.right.x, 0, inputMoveX * mainCamera.transform.right.z)
        //                                      + new Vector3(inputMoveY * mainCamera.transform.forward.x, 0, inputMoveY * mainCamera.transform.forward.z)).normalized;

        //    float difAngle = Vector3.Angle(transform.forward, moveForward);

        //    //transform.rotation = Quaternion.LookRotation(moveForward);
        //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveForward), Time.deltaTime * rotateSpeed);
        //    if (difAngle < rotateAngle) transform.position += Time.deltaTime * inputV * speed * transform.forward;
        //}
        //else animator.SetFloat("moveSpeed", 0);

    }

    private void Move()
    {
        inputMoveX = Input.GetAxis("Move Horizontal");
        inputMoveY = Input.GetAxis("Move Vertical");
        Vector3 baseFWD, baseRight;
        baseFWD = mainCamera.transform.forward;
        baseRight = mainCamera.transform.right;

        float inputV = Mathf.Clamp01(inputMoveX * inputMoveX + inputMoveY * inputMoveY);
        if (inputV > 0.15f)
        {
            animator.SetFloat("moveSpeed", inputV);
            Vector3 moveForward = (new Vector3(inputMoveX * baseRight.x, 0, inputMoveX * baseRight.z)
                                              + new Vector3(inputMoveY * baseFWD.x, 0, inputMoveY * baseFWD.z)).normalized;
            float difAngle;
            if (!hasParent) difAngle = Vector3.Angle(transform.forward, moveForward);
            else difAngle = Vector3.Angle(model.transform.forward, moveForward);
            //transform.rotation = Quaternion.LookRotation(moveForward);
            if (!hasParent)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveForward), Time.deltaTime * rotateSpeed);
                if (difAngle < rotateAngle) transform.position += Time.deltaTime * inputV * speed * transform.forward;
            }
            else {
                model.transform.rotation = Quaternion.Lerp(model.transform.rotation, Quaternion.LookRotation(moveForward), Time.deltaTime * rotateSpeed);
                if (difAngle < rotateAngle) transform.position += Time.deltaTime * inputV * speed * model.transform.forward;
            } 
            


        }
        else
        {
            animator.SetFloat("moveSpeed", 0);
        }
    }

    public void SwitchDialogue(bool State) {
        During_Dialogue = State;
    }


}
