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

            // ÿһ֡��ȥ��ȡ����
            inputHandler.TickInput(delta);

            // �ƶ����� ���� x+y �����������Ӽ��ɵõ�
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;
            moveDirection *= speed;

            // ����ͶӰ��������ٶȡ������Ҹо�û��Ҫ����Ϊv��hֻ����һ����άƽ��
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;
            //rigidbody.velocity = moveDirection;


            // ����������ֵ�ı�animator�еĲ������Դ������Ŷ���
            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);

            // ��ת
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
            // ��Vector3�������ת������
            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            // ΪʲôҪ��camera��transform������target�ķ��򣿣���Ϊ�˺�������camera����target���߼���
            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;
            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = myTransform.forward;
 

            float rs = rotationSpeed;

            // ��Vector3����ת��Ϊ��Ԫ��
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta); // �����ֵ

            myTransform.rotation = targetRotation;
        }

        #endregion

    }
}
