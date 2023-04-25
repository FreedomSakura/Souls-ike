using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerLocomotion : MonoBehaviour
    {
        Transform cameraObject;
        InputHandler inputHandler;
        Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Status")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float rotationSpeed = 10;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            
            animatorHandler.Initialize();
            cameraObject = Camera.main.transform;
            myTransform = transform;
        }

        public void Update()
        {
            float delta = Time.deltaTime;

            // 每一帧都去获取输入
            inputHandler.TickInput(delta);

            // 移动方向 —— x+y 方向的向量相加即可得到
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;
            moveDirection *= speed;

            // 计算投影到地面的速度——但我感觉没必要，因为v和h只构成一个二维平面
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;
            //rigidbody.velocity = moveDirection;


            // 动画——插值改变animator中的参数，以此来播放动画
            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);

            // 旋转
            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }

        }

        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;

        private void HandleRotation(float delta)
        {
            // 用Vector3计算出旋转的向量
            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            // 为什么要用camera的transform来计算target的方向？？？为了后续制作camera跟随target的逻辑吗？
            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;
            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = myTransform.forward;
 

            float rs = rotationSpeed;

            // 将Vector3向量转换为四元数
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta); // 球面插值

            myTransform.rotation = targetRotation;
        }

        #endregion

    }
}
