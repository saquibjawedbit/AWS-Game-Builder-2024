
using UnityEngine;
using UnityEngine.UI;
using GoSystem.Control;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GoSystem
{
    public class GoSystems
    {
        private static readonly GoSystems gs = new GoSystems();
        private GoCharacterController player;
        public static GoSystems getSystem(GameObject script)
        {
            return script.GetComponentInParent<GoCharacterController>().Gs;
        }
        public static GoCharacterController Player
        {
            get => gs.player;  // return gs.Player;
            set => gs.player = value;  // set gs.player to value
        }
        public static bool lockGoSystemsChontorl { get; set; }
        public static UnityEngine.Events.UnityEvent OnStarted;
        public static UnityEngine.Events.UnityEvent OnUpdate;
        public static UnityEngine.Events.UnityEvent OnExit;

        public static class Axis
        {
            public static float x
            {
                get
                {
                    float X = Input.GetAxis("Horizontal");
                    return X;
                }
    }
            public static float z
            {
                get
                {
                    float Z = Input.GetAxis("Vertical");
                    return Z;
                }
            }
            public class RawAxis
            {
                public static float x
                {
                    get
                    {
                        float X = Input.GetAxisRaw("Horizontal");
                        return X;
                    }
                }
                public static float z
                {
                    get
                    {
                        float Z = Input.GetAxisRaw("Vertical");
                        return Z;
                    }
                }
                public static float x_mobile;
                public static float z_mobile;
            }
        }
        #region IK Var
        //IkFoodControler
        public static bool IsFoods = true;
        public static bool IsLags = true;
        #endregion
        #region character Var
        public static Animator animatorControler
        {
            get
            {
                Animator Anim = GoSystemsController.Charcter.GetComponent<Animator>();
                return Anim;
            }
        }
        public static Rigidbody Grigidbody
        {
            get
            {
                return GoSystemsController.rigidbody;
            }

        }
        GoInputSystem Inputsystem = new GoInputSystem();
        private string yAxismovment = "Y Movement";
        private string movment = "Movement";
        private string InputMove = "InputMovmint";
        private string action = "ActionState";
        private string crouch = "Crouch";
        private string Jump = "Jump";
        public string horizontal = "Horizontal";
        public string Vertical = "Vertical";
        private string Grawnd = "isGrawnd";
        private bool LockJump;
        private bool LockCrouch;
        public bool sprint = true;
        public float CrouchColiderSizeOffset = 2f;
        public float GrawndDistens;
        public float initialJumpVelocity;
        public float TimeFall = 3f;
        public float angle;
        public bool IsHandIK =true;
        public bool IsSprint = true;
        private float velocity;
        private Slider SprintUiBar;
        private float savespeed;
        private float Gravity = -10;
        private float TimerJump;
        private bool IntoObject;
        private bool IsRun;
        private int Crouch;
        private bool IntoTag;
        private RaycastHit hit;
        private Vector3 S_Crouch;
        public Vector3 CrouchOffset;
        #endregion
        #region Is Var
        public bool IsGround { get; set; }
        public bool IsLockMove;
        public bool AxisCtrl = true;
        public bool IsLockJump;
        public bool IsDie;
        public bool IsFoodSound;
        public bool NotIdel;
        public bool CameraPositionCrouch;
        public bool IsCrouch;
        public bool IsLockCrouch;
        public bool IsCameraInput;
        public bool IsActivte { get; set; }
        public bool IsPip { get; set; }
        public bool RagdallActivate { get; set; }
        public bool IsRagdall;
        public bool IsZipLine { get; set; }
        public bool IsMoveVilasty = true;
        public bool IsMoveCameraDiraction = true;
        public bool IsJumping { get; set; }
        public float Go_Velocity;
        #endregion
        public bool MobileInputMove;
        private static float x_anim, z_anim,x_move,z_move;
        public void MovimentBehaviour(float speed, float fastRunSpeed, GoInput input,MoveInput moveInput)
        {
            UpdateAxis(ref x_move, ref z_move, moveInput);
            float x = x_move, z = z_move;
            var Angle = x + z;
            angle = Angle;
            if (!IsLockMove)
            {
                var GetInput = Inputsystem.GetKey(input.Kaybord.ToString(), input.Joystick.ToString(), MobileInputMove);
                var animator = animatorControler;
                GoCharacterController cc = GoSystemsController.Charcter.GetComponent<GoCharacterController>();
                GoSystemsController.rigidbody.AddForce(Physics.gravity * 1, ForceMode.Acceleration);
                SprintUiBar = cc.SprintBar;
                SprintUiBar.maxValue = cc.TimeSprintRun;
                if (Velocity(x,z) != 0)
                {
                    IsFoods = false;
                    IsLags = false;
                }
                else
                {
                    IsFoods = true;
                    IsLags = true;
                }

                if (IsSprint)
                {
                    if (animator.GetFloat(action) < 1)
                    {
                        savespeed = speed;
                        sprint = false;

                    }
                    else if (GetInput && animator.GetFloat(action) > 0 && cc.SprintBar.value != 0 && IsJumping == false)
                    {
                        savespeed = fastRunSpeed;
                        cc.SprintBar.value -= Time.deltaTime;
                        IsRun = false;
                        sprint = true;
                    }
                }
                else
                {
                    if (!IsCrouch)
                    {
                        savespeed = speed;
                    }
                }
                    if (cc.SprintBar.value < cc.SprintBar.maxValue && !GetInput)
                    {
                        IsRun = true;
                    sprint = false;
                }
                    if (IsRun == true)
                    {
                        cc.SprintBar.value += Time.deltaTime;
                        if (cc.SprintBar.value >= cc.SprintBar.maxValue && !GetInput)
                        {
                            IsRun = false;
                         
                    }
                    }
                var moveDir = UpdateDir();
                if (IsMoveVilasty == true)
                {
                    Grigidbody.velocity = new Vector3(moveDir.x * savespeed, Grigidbody.velocity.y, moveDir.z * savespeed);
                }
                if (moveDir != Vector3.zero&& IsMoveCameraDiraction)
                {
                    Quaternion desiredRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                    Grigidbody.transform.rotation = Quaternion.Slerp(Grigidbody.transform.rotation, desiredRotation, 14 * Time.deltaTime);
                }
                #region animator
                if (GetInput && angle != 0 && IsSprint == true && cc.SprintBar.value > 0)
                {
                    animator.SetFloat(action, Mathf.MoveTowards(animator.GetFloat(action), 1, Time.deltaTime * fastRunSpeed));
                }
                else if (angle == 0 || cc.SprintBar.value <= 0)
                {
                    animator.SetFloat(action, Mathf.MoveTowards(animator.GetFloat(action), 0, Time.deltaTime * fastRunSpeed));

                }
                else if (!GetInput|| cc.SprintBar.value == 0)
                {
                    animator.SetFloat(action, Mathf.MoveTowards(animator.GetFloat(action), 0, Time.deltaTime * fastRunSpeed));
                }
                #endregion
                if (x != 0 || z != 0)
                {
                    NotIdel = true;
                }
                else
                {
                    NotIdel = false;
                }
            }
            if (IsLockJump != true)
            {
                animatorControler.SetBool(Jump, IsJumping);
            }

        }
        public static Vector3 UpdateDir()
        {
            if(Camera.main!=null)
            {
                var cameraPos = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
                var inputPos = cameraPos * new Vector3(x_move, 0, z_move);
                var moveDir = inputPos.normalized;
                var right = Camera.main.transform.right;
                return moveDir;
            }

            return Vector3.zero;
        }
        public void GoCursor(bool visible,bool lockCursor)
        {

            Cursor.visible = visible;
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
        public void UpdateAxis( ref float x,ref float z  ,MoveInput move)
        {
            if (move.MoveAxis == input3.ComputerAndConsul)
            {
                 x = Axis.x;
                 z = Axis.z;
                 z_anim = z;
                 x_anim = x;

            }
            else
            {
                x = move.MobileJoystick.Horizontal();
                z = move.MobileJoystick.Vertical();
                z_anim = z;
                x_anim = x;
              ///  Cursor.visible = true;
               /// Cursor.lockState = CursorLockMode.None;

            }
            if (AxisCtrl == true)
            {
             
                if (move.MoveAxis == input3.ComputerAndConsul)
                {
                    UpdateAnimation(x_anim, z_anim);
                }
                else
                {
                    if (move.MobileJoystick.Horizontal() > 0)
                    {
                        x_anim = 1;

                    }
                    else if (move.MobileJoystick.Horizontal() < 0)
                    {
                        x_anim = -1;
                    }
                    if (move.MobileJoystick.Vertical() > 0)
                    {
                        z_anim = 1;
                    }
                    else if (move.MobileJoystick.Vertical() < 0)
                    {
                        z_anim = -1;
                    }
                    if (move.MobileJoystick.Vertical() == 0)
                    {
                        z_anim = 0;
                    }
                    if (move.MobileJoystick.Horizontal() == 0)
                    {
                        x_anim = 0;
                    }
                        UpdateAnimation(x_anim, z_anim);
                }


            }
        }
        
        private void UpdateAnimation(float x,float z)
        {
            var Z = Mathf.MoveTowards(animatorControler.GetFloat(yAxismovment), z, Time.deltaTime);
            animatorControler.SetFloat(yAxismovment, Z);
            var X = Mathf.MoveTowards(animatorControler.GetFloat(movment), x, Time.deltaTime);
            animatorControler.SetFloat(movment, X);
            Go_Velocity = Velocity(x,z) * 3;
            if (Go_Velocity > 1)
            {
                Go_Velocity = 1;
            }
            animatorControler.SetFloat(InputMove, Go_Velocity);
        }
        public bool MobileInputCrouch;
        bool couchSpam;
        public void CrouchPosAction(GoInput CrouchActive, float speed)
        {
            if (!IsLockMove||!IsLockCrouch)
            {
                var GetDown = Inputsystem.GetKeyDown(CrouchActive.Kaybord.ToString(), CrouchActive.Joystick.ToString(),MobileInputCrouch);
                var Target = GoSystemsController.bones[10].transform;
                var WallTargetChick = ChackUp(Target);
                animatorControler.SetBool(crouch, IsCrouch); 
                CapsuleCollider Capsule = GoSystemsController.Charcter.GetComponent<CapsuleCollider>();
                if (GetDown && !RagdallActivate)
                {
                    if (!couchSpam)
                    {

                        if (Crouch == 0 && !LockCrouch)//inter Crouch
                        {
                            //couchSpam = true;
                            CameraPositionCrouch = true;
                            IsCrouch = true;
                            Crouch = 1;
                            IsSprint = false;
                            savespeed = speed;
                            S_Crouch = Capsule.center;
                            Capsule.height /= CrouchColiderSizeOffset;  
                            Capsule.center -= new Vector3(0, Capsule.height / 2, 0);

                            OnExit = new UnityEngine.Events.UnityEvent();
                            OnExit.AddListener(LockBackCrouch);

                        }
                        else if (Crouch == 1 && LockCrouch && !WallTargetChick)//Exit crouch
                        {
                            Crouch = 3; couchSpam = true;
                            IsCrouch = false;
                            Capsule.height = Capsule.height * CrouchColiderSizeOffset;
                            Capsule.center = S_Crouch;
                            OnExit = new UnityEngine.Events.UnityEvent();
                            OnExit.AddListener(LockBackCrouch);

                        }

                    }

                }

                if (!IsGround && IsCrouch && LockCrouch)
                {
                    IsSprint = true;
                    IsCrouch = false;
                    LockCrouch = false;
                    Capsule.center = S_Crouch;
                    Capsule.height = Capsule.height * 2;
                    Crouch = 0;
                }


            }
        }
        private void LockBackCrouch()
        {
            if (LockCrouch == true)
            {
                IsCrouch = false;
                   LockCrouch = false;
                IsSprint = true;
                Crouch = 0;

            }
            else
            {
                LockCrouch = true;

            }


            couchSpam = false;
            //OnStarted = new UnityEngine.Events.UnityEvent();
            OnExit = new UnityEngine.Events.UnityEvent();
            //OnUpdate = new UnityEngine.Events.UnityEvent();

        }
        public void LockAllInputs(bool Lock)
        {
            if (Lock)
            {

                IsActivte = false;
                IsLockMove = true;
                IsLockJump = true;
                IsRagdall = false;
                IsLockJump = true;
                IsFoods = false;
                IsFoodSound = false;
                IsLags = false;

            }
            else
            {

                IsActivte = true;
                IsLockMove = false;
                IsLockJump = false;
                IsRagdall = true;
                IsFoodSound = true;
                IsLockJump = false;
                IsFoods = true;
                IsLags = true;
            }
        }
        private bool ChackUp(Transform Target)
        {
            var ray = new Ray(Target.position, Vector3.up);
            bool T = Physics.Raycast(ray, 2);
            return T;
        }
        public static void Layers(string[] NameLayers)
        {
            for (int i = 8; i <= 31; i++)
            {
                var layerN = LayerMask.LayerToName(i);
                if (layerN.Length > 0)
                    NameLayers.SetValue(layerN, i);
            }
        }
        public bool IsGraunded(LayerMask layer)
        {
            var fixposCast = new Vector3(GoSystemsController.Charcter.transform.position.x, GoSystemsController.Charcter.transform.position.y + 0.5f, GoSystemsController.Charcter.transform.position.z);
            Ray ray = new Ray(fixposCast, Vector3.down);
            IsGround = Physics.SphereCast(ray, 0.1f, out hit, GrawndDistens, layer);
            animatorControler.SetBool(Grawnd, IsGround);
            Debug.DrawLine(fixposCast, new Vector3(fixposCast.x, fixposCast.y - 0.4f, fixposCast.z));
            IsGround = IsGround;
            return IsGround;

        }
        public bool tesgtinput;
        public void test()
        {
            Debug.Log(tesgtinput);
            Inputsystem.GetValue = tesgtinput;
        }
        public bool MobileInputJump;
        public void JumpBehaviour(float MaxJumpTime,float JumpHeight,float jumpForce, LayerMask layer,GoInput input)
        {
            var getinput = Inputsystem.GetKeyDown(input.Kaybord.ToString(), input.Joystick.ToString(), MobileInputJump);
            if (getinput && !IsLockJump)
            {
                IsFoods = true;
                IsLags = true;
                IsJumping = true;
                if (IsGraunded(layer))
                {
                    animatorControler.SetBool(Jump, IsJumping);
                    if (IsJumping)
                    {
                        var timeToApox = MaxJumpTime / 2;
                        var force = jumpForce * 2;
                        Gravity = (-2 * MaxJumpTime) / Mathf.Pow(timeToApox, 2);
                        initialJumpVelocity = (2 * JumpHeight / timeToApox);
                        Grigidbody.AddForce(Grigidbody.velocity.x, Grigidbody.velocity.y + initialJumpVelocity * force, Grigidbody.velocity.z);
                        IsGround = false;
                        Physics.gravity = new Vector3(0, Gravity, 0);
                    }
                }
            }
        }
        public void UnLockStopWall()
        {
            Debug.Log("Stop Wall");
            IsLockMove = false;
            animatorControler.SetBool("StopWall", false);

        }
        public void Dircection(Transform MyPositin, Vector3 PointDirection, float speed = 5, bool Move = true, bool lookAt = false)
        {
            Vector3 direction = PointDirection - MyPositin.position;
            var pos = new Vector3(direction.x, direction.y, direction.z);
            Debug.DrawRay(MyPositin.position, direction, Color.red);
            direction.Normalize();
            if (Move == true)
            {
                MyPositin.Translate(direction * Time.deltaTime * speed, Space.World);
            }
            if (lookAt == true)
            {
                MyPositin.LookAt(PointDirection);
            }
        }
        public void FixRotation(Transform sorce, Transform target, float time,Vector3 offset)
        {
            Vector3 relativePos = target.position - sorce.position;
            relativePos = new Vector3(relativePos.x + offset.x, relativePos.y + offset.y, relativePos.z + offset.z);
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            Quaternion current = sorce.localRotation;
            sorce.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime
                * time);
        }
        public bool ChackInToByObject(GameObject Go_Object)
        {
            foreach (Collider collider in GoTrigger.xcol)
            {
                if (collider.transform.gameObject == Go_Object)
                {
                    IntoObject = true;
                    Debug.Log("we find it");
                }
            }
            return IntoObject;
        }
        public bool ChackInToByTag(string yourObjectTag)
        {
            foreach (Collider collider in GoTrigger.xcol)
            {
                if (collider.transform.tag == yourObjectTag)
                {
                    IntoTag = true;
                }
            }
            return IntoTag;
        }
        public Vector3 FixPosition(Vector3 target, Vector3 source, float OffsetX = 0, float OffsetY = 0, float OffsetZ = 0)
        {
            var FixPos = new Vector3(target.x + OffsetX, target.y + OffsetY, target.z + OffsetZ);
            source = Vector3.Lerp(source, FixPos, 8 * Time.deltaTime);
            return source;
        }
        public void FixPosFood(float offSet,Vector3 offsetOragenFoodposition, ref LayerMask layer)
        {
            if (IsFoods)
            {
                if (animatorControler)
                {
                    animatorControler.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                    animatorControler.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                    animatorControler.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                    animatorControler.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
                     RaycastHit FHit;
                     RaycastHit Lhit;
                  
                     Ray ray = new Ray(animatorControler.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up+offsetOragenFoodposition, Vector3.down);
                    if (Physics.Raycast(ray, out Lhit, 3f, layer))
                    {
                            Vector3 footPodition = Lhit.point;
                            footPodition.y += offSet;
                            animatorControler.SetIKPosition(AvatarIKGoal.LeftFoot, footPodition);
                            animatorControler.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(animatorControler.transform.forward, hit.normal));
                            var v = animatorControler.GetIKHintPosition(AvatarIKHint.LeftKnee);  
                    }
                    ray = new Ray(animatorControler.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up+ offsetOragenFoodposition, Vector3.down);
                    if (Physics.Raycast(ray, out FHit, 3f, layer))
                    {
                            Vector3 footPodition = FHit.point;
                            footPodition.y += offSet;
                            animatorControler.SetIKPosition(AvatarIKGoal.RightFoot, footPodition);
                            animatorControler.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(animatorControler.transform.forward, hit.normal));
                        
                    }
                }
            }
        }
        public void FixPosLegs(Vector3 offsetL, Vector3 offsetR)
        {
            if (IsLags)
            {
                animatorControler.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 1);
                animatorControler.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 1);
                Transform orginalPosition = animatorControler.transform;
                Vector3 L = FixLegIk(orginalPosition, offsetL);
                Vector3 R = FixLegIk(orginalPosition, offsetR);
                animatorControler.SetIKHintPosition(AvatarIKHint.LeftKnee, L);
                animatorControler.SetIKHintPosition(AvatarIKHint.RightKnee, R);
            }
        }
        private Vector3 FixLegIk(Transform orginalPosition, Vector3 offset)
        {
            var point = new Vector3(orginalPosition.position.x + offset.x, orginalPosition.position.y + offset.y, orginalPosition.position.z + offset.z);
            var fixDir = point - orginalPosition.position;
            fixDir = fixDir.normalized;
            var relativeAngle = Vector3.Angle(orginalPosition.forward, fixDir);
            var localRelativePosition = orginalPosition.TransformPoint(-fixDir);
            return localRelativePosition;

        }
        private  float Velocity(float x,float z)
        {
            if (x > 0 && z > 0)
            {
                velocity = x + z;
                if (velocity > 1)
                {
                    velocity = 1;
                    return velocity;
                }
                else
                {
                    return velocity;
                }
            }
            else if (x < 0 && z < 0)
            {
                velocity= x + z;
                if (-velocity > 1)
                {
                    velocity = 1;
                    return velocity;
                }
                else
                {
                    return -velocity/2;
                }
               
            }
            else if (x > 0 && z < 0)
            {
                if (x == 1)
                {
                    velocity = x;
                    return velocity;
                }
                else
                {
                    return 1;
                }
            }
            else if (x < 0 && z > 0)
            {
                if (z == 1)
                {
                    velocity = z;
                    return velocity;
                }
                else
                {
                    return 1;
                }
            }
            else if (x == 0 && z > 0)
            {
               
                return z;
            }
            else if (x > 0 && z == 0)
            {

                return x;
            }
            else if (x < 0 && z == 0)
            {

                return -x;
            }
            else if (z < 0 && x == 0)
            {

                return -z;
            }
            else
            {
                return 0;
            }
        }

#if UNITY_EDITOR
        [InitializeOnLoad]
        public class Startup
        {
            static Startup()
            {
                // GetGoSystems();
            }
        }
#endif
    }


    [ExecuteInEditMode]
    public class DrowMithods
    {
        public static void GoDrow(Vector3 Position, Vector3 Size,string Color= "#FF8E28")
        {
            Color color;
            ColorUtility.TryParseHtmlString(Color, out color);
            color = new Color(color.r, color.g, color.b, 0.5f);
            Gizmos.color = color;
            Gizmos.DrawCube(Position, Size);

        }

    }
  
}
