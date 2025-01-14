
using UnityEngine;
using System.Collections.Generic;
using GoSystem.Control;
using UnityEngine.UI;
using System.Collections;
using System;

namespace GoSystem
{
    [GBehaviourAttributeAttribute("Character Controller", true)]
    public class GoCharacterController : GoSystemsBehaviour
    {
        public GoSystems Gs = new GoSystems();
        [HideInInspector] public int Index;

        [Header("Idel Settings")]
        public float MoveSpeed = 7f;
        public float SprintRunSpeed = 10f;
        [Space(5)]
        [Header("Crouch Settings")]
        [Space(5)]
        public float CrouchSpeed = 3;
        [HideInInspector] public bool Crouch = true;
        public float CrouchClliderSize = 2;
        public Vector3 CrouchClliderOffSet = new Vector3(0, 1, 0);
        [Space(5)]
        [Header("Jump Settings")]
        [Space(5)]
        public LayerMask layerJump;
        public float jumpForce = 5f;
        public float JumpHigh = 5;
        public float TimerJump;
        public float GrawndDistens = 0.6f;
        public LayerMask LayerWall;
        [Space(5)]
        [Header("Input Buttons")]
        public GoInput InputJump = new GoInput();
        public GoInput InputCrouch = new GoInput();
        public GoInput InputSprintRun = new GoInput();
        public MoveInput MoveAxis = new MoveInput();
        [Space(5)]
        [Header("Ik Foot Settings")]
        [Space(5)]
        [Range(0, 1)]
        public float IKfootWeight;
        public Vector3 offsetFoodPosition;
        public LayerMask IKlayer;
        [Space(5)]
        private float _speed;
        [Space(5)]
        [HideInInspector] public bool IsGround;
        [HideInInspector] public bool lockcode;
        [HideInInspector] public bool lockjump;
        [HideInInspector] public bool LockIKFoots;
        public Vector3 offsetLLeg;
        public Vector3 offsetRLeg;
        [Space(5)]
        [Header("UI Settings")]
        [Space(5)]
        public Slider SprintBar;
        public float TimeSprintRun = 3f;

        public override void OnStartLocalPlayer()
        {
            if (!isLocalPlayer) return;

            GoSystems.Player = this;
            GoSystemsController.Getbone();
            _speed = MoveSpeed;
            //Cursor.lockState = CursorLockMode.Locked;
            if (SprintBar == null) return;
                SprintBar.maxValue = TimeSprintRun;
                SprintBar.value = TimeSprintRun;

        }

        void Update()
        {
         
            if(!isLocalPlayer) return;
            GoTrigger.uplay();
            GoSystemsController.Getbone();
            Gs.IsGraunded(layerJump);
            if (!lockcode)
            {
                Gs.MovimentBehaviour(MoveSpeed, SprintRunSpeed, InputSprintRun,MoveAxis);
                if (!lockjump)
                {
                    IsGround = Gs.IsGraunded(layerJump);
                    //When entering the Jump state in the Animator, output the message in the console
                    Jumping();
                    JumpAvoidance();
                    if (!IsGround)
                    {
                        GoSystems.IsFoods = false;
                        GoSystems.IsLags = false;

                        GoSystemsController.stop();
                    }
                    else
                    {
                        GoSystemsController.Move();
                        Gs.IsJumping = false;
                    }

                }
                if (Crouch)
                {
                    Gs.CrouchPosAction(InputCrouch, CrouchSpeed);
                    Gs.CrouchColiderSizeOffset = CrouchClliderSize;
                    Gs.CrouchOffset = CrouchClliderOffSet;
                }
                Gs.GrawndDistens = GrawndDistens;
            }
        }


        void Jumping()
        {
            Gs.JumpBehaviour(TimerJump, JumpHigh, jumpForce, layerJump, InputJump);
        }
        private bool IsCollidingWithObstacles()
        {
            var pos = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z);
            Collider[] colliders = Physics.OverlapSphere(pos, 0.5f, LayerWall);
            return colliders.Length > 0;
        }
        private void JumpAvoidance()
        {
            if (Gs.IsLockJump) return;
            
                if (IsCollidingWithObstacles() && IsGround == false && Gs.Go_Velocity > 0)
                {
                    GoSystemsController.rigidbody.velocity = new Vector3(GoSystemsController.rigidbody.velocity.x, -8, GoSystemsController.rigidbody.velocity.z);
                    GoSystemsController.rigidbody.useGravity = false;
                }
                else
                {
                    GoSystemsController.rigidbody.useGravity = true;
                }
            
        }
        private void FixedUpdate()
        {
            GoTrigger.GoEnter();
            GoTrigger.GoExit();
        }
        private void OnAnimatorIK(int layerIndex)
        {
            if (!LockIKFoots)
            {
                Gs.FixPosFood(IKfootWeight, offsetFoodPosition, ref IKlayer);
                Gs.FixPosLegs(offsetLLeg, offsetRLeg);
            }
        }
        #region Evints Methods
        public void LockMove()
        {
            Gs.IsLockMove = true;
        }
        public void Unlockcode()
        {
            Gs.IsLockMove = false;
        }
        public void LockJump()
        {
            Gs.IsLockJump = true;
            lockjump = true;
        }
        public void UnlockJump()
        {
            lockjump = false;
            Gs.IsLockJump = false;
        }
        #endregion
        private void OnDrawGizmos()
        {
            Color color;
            var pos = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z);
            ColorUtility.TryParseHtmlString("#FF8E28",out color);
            Gizmos.color = color;
            Gizmos.DrawWireSphere(pos, 0.8f);
        }

        public void OnMobileCrouchClick()
        {
            StartCoroutine( InputCrouch.MobileInput(SetCrouchBool));
            // Gs.MobileInputCrouch = true;
        }

        public void OnMobileJumpClick()
        {
            StartCoroutine(InputJump.MobileInput(SetJumpBool));
        }
        public void OnMobileSprintClick()
        {
            Gs.MobileInputMove = true;
        }
        private void SetCrouchBool(bool status)
        {
            Gs.MobileInputCrouch = status;
        }
        private void SetJumpBool(bool status)
        {
           Gs.MobileInputJump = status;
        }
        public void OnMobileSprintNotClick()
        {
            Gs.MobileInputMove = false;
        }

    }
    
}

