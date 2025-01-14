
using UnityEngine;
using GoSystem.Control;
namespace GoSystem {
    [GBehaviourAttributeAttribute("Camera controller", false)]
    public class GoCameraSystem : GoSystemsBehaviour
    {
         public CameraMobileInput InputCameraSystem;
        [System.Serializable]
        public enum PlayerState
        {
            Walking,
            Crouch,
            Ragdall
        }
        [Space(5)]
        public PlayerState cameraPlayerState;
        public float CameraSensitivity = 10;
        public float DistenceWalking = 3, DistenceCrouch = 1, DistenceRagDall =4;
        public Vector2 maxrotatecamera = new Vector2(-50, 50);
        public float smoothtime = 0.2f;
        public Transform target;
        public LayerMask TriggerLayer;
        public Vector3 OffSetCameraWithCrouch;
        public Vector3 offSetCameraWalking;
        private float _DistenceCamera, Distencecollition;
        [HideInInspector] public float DistenceCamera {
            get
            {
                return _DistenceCamera;
            }
        }
        private float distence; float x, y;
        private  GoSystems Gs;private string mosuex = "MouseX", mosuey = "MouseY", player = "Player";
        private bool Collintion, GetTargetByCrouch = false;
        [HideInInspector] public bool lockcode;public Vector3 _offsetMainCamera;
        private Vector3 currentRotation, smoothVelocity = Vector3.zero, cameraPositionBehaviour;
        private Transform _target;
        private RaycastHit hit;
        private GoCharacterController cc;
        private GoSystemsController gsc = new GoSystemsController();
        private void Start()
        {
            _DistenceCamera = DistenceWalking;
           

            if (target == null)
            {
                if (Control.GoSystemsController.bones.Count > 1)
                   target = Control.GoSystemsController.bones[10].transform;
                Gs = GoSystems.getSystem(target.gameObject);
                
                _target = target;
                Gs.IsCameraInput = true;
            }
            else
            {
                if (target.gameObject.active == false)
                {
                    target = GameObject.FindObjectOfType<GoCharacterController>().transform.Find("targetCam");
                    Gs = GoSystems.getSystem(target.gameObject);
                    _target = target;
                    Gs.IsCameraInput = true;
                }
                else
                {
                    Gs = GoSystems.getSystem(target.gameObject);
                    _target = target;
                    Gs.IsCameraInput = true;
                }
              
            }
        }
        private void Update()
        {
            switch (cameraPlayerState)
            {
                case PlayerState.Walking:
                    _DistenceCamera = Transition(_DistenceCamera, DistenceWalking, 7f);
                    break;
                case PlayerState.Ragdall:
                    _target.position = GoSystemsController.bones[10].position;
                    _DistenceCamera = Transition(_DistenceCamera, DistenceRagDall, 7f);
                    break;
                case PlayerState.Crouch:
                    GetTargetCrouch();
                    break;

         
            }
                PositionCameraBehaviour();
                TargetBehaviour();
        }
        private void collitionBehaviour()
        {
            Ray ray = new Ray(_target.position, -transform.forward);
            Physics.SphereCast(ray, 0.1f, out hit, _DistenceCamera);
            Distencecollition = Vector3.Distance(_target.position, hit.point);
            if (hit.transform != null)
            {

                if (gsc.checkLayes(hit.transform.gameObject, TriggerLayer))
                {
                   
                    _DistenceCamera = Transition(_DistenceCamera, Distencecollition, 7f);
                }
               
            }
            else
            {
                if (Gs.IsCrouch)
                {
                    cameraPlayerState = PlayerState.Crouch;
                }
                else
                if (Gs.RagdallActivate)
                {
                    cameraPlayerState = PlayerState.Ragdall;
                }
                else if (!Gs.IsCrouch || !Gs.RagdallActivate)
                {
                    cameraPlayerState = PlayerState.Walking;
                }
            }
        }
        public float Transition(float startValue, float endValue, float SpeedTransition)
        {

            var transitionValue = startValue;
            var transitionDuration = SpeedTransition;
            if (startValue < endValue)
            {
                transitionValue = startValue + (endValue - startValue) / 1 * Time.deltaTime * transitionDuration;
            }
            else
            {
                transitionValue = startValue - (startValue - endValue) / 1 * Time.deltaTime * transitionDuration;
            }
            return transitionValue;
        }
        private void PositionCameraBehaviour()
        {
            if (Gs.IsCameraInput)
            {
                if (!lockcode)
                {
                    if (InputCameraSystem.MoveAxis == input3.ComputerAndConsul)
                    {
                        x += Input.GetAxis(mosuex) * CameraSensitivity;
                        y -= Input.GetAxis(mosuey) * CameraSensitivity;
                        MovePosition();
                    }
                    else
                    {
                        x = InputCameraSystem.TouchSpace.newCameraAxis.x * InputCameraSystem.panSpeed;
                        y = InputCameraSystem.TouchSpace.newCameraAxis.y * InputCameraSystem.panSpeed;
                        MovePosition();
                    }
                }
            }
        }
        private void MovePosition()
        {
            y = Mathf.Clamp(y, maxrotatecamera.x, maxrotatecamera.y);
            var mousePos = new Vector3(y, x);
            currentRotation = Vector3.SmoothDamp(currentRotation, mousePos, ref smoothVelocity, smoothtime);
            transform.localEulerAngles = currentRotation;
        }
        private void TargetBehaviour()
        {
            if (_target != null&&!lockcode)
            {
                cameraPositionBehaviour = _target.position - transform.forward * _DistenceCamera + _target.right * _offsetMainCamera.x;
                cameraPositionBehaviour.y +=   _offsetMainCamera.y;
              //  cameraPositionBehaviour.z += _target.position.z * _offsetMainCamera.z;
                transform.localPosition = cameraPositionBehaviour;
         
                if (Gs.RagdallActivate != true)
                {
                    cameraPlayerState = PlayerState.Crouch;
                    if (GetTargetByCrouch == true) return;
                    _target = target;
                }
                else
                {
                    cameraPlayerState = PlayerState.Ragdall;
                }
                collitionBehaviour();
            }
            _offsetMainCamera = offSetCameraWalking;
        }
        private void GetTargetCrouch()
        {
            if (Gs.IsCrouch == true)
            {
                _offsetMainCamera = Vector3.Lerp(_offsetMainCamera, OffSetCameraWithCrouch, 5 * Time.deltaTime);


                    _DistenceCamera = Transition(_DistenceCamera, DistenceCrouch, 7f);
                GetTargetByCrouch = true;
             _target = GoSystemsController.bones[10];
            }
            if (Gs.IsCrouch == false && GetTargetByCrouch == true)
            {
                _DistenceCamera = Mathf.Lerp(_DistenceCamera, DistenceWalking, 5 * Time.deltaTime);
                _offsetMainCamera = Vector3.Lerp(_offsetMainCamera, offSetCameraWalking, 5 * Time.deltaTime);
                GoSystems.OnExit.AddListener(GetTarget);
            }
        }
        private void GetTarget()
        {
            GetTargetByCrouch = false;
            _target = target;
        }

    }
    
}